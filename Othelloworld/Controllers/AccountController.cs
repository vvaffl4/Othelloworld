using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using Othelloworld.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Othelloworld.Controllers
{
	[Authorize(Policy = JwtBearerDefaults.AuthenticationScheme, Roles = "user, admin")]
	[ApiController]
	[Route("[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly UserManager<Account> _userManager;
		private readonly SignInManager<Account> _signInManager;
		private readonly IAccountService _accountService;
		private readonly IPlayerRepository _playerRepository;

		public AccountController(
			IConfiguration configuration,
			UserManager<Account> userManager,
			SignInManager<Account> signInManager,
			IAccountService accountService,
			IPlayerRepository playerRepository)
		{
			_configuration = configuration;
			_userManager = userManager;
			_signInManager = signInManager;
			_accountService = accountService;
			_playerRepository = playerRepository;
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if (!ModelState.IsValid) return BadRequest("Supplied bad model");

			var hasher = new PasswordHasher<Account>();
			var accountId = Guid.NewGuid().ToString();
			var account = new Account
			{
				Id = accountId,
				UserName = model.Username,
				NormalizedUserName = model.Username,
				Email = model.Email,
				NormalizedEmail = model.Email,
				EmailConfirmed = true,
				PasswordHash = hasher.HashPassword(null, model.Password),
				SecurityStamp = String.Empty
			};

			var accountResult = await _userManager.CreateAsync(account, model.Password);

			if (!accountResult.Succeeded) return BadRequest(accountResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

			var roleResult = await _userManager.AddToRoleAsync(account, "user");

			if (!roleResult.Succeeded) return BadRequest(roleResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

			_playerRepository.CreatePlayer(new Player
			{
				Username = model.Username,
				AmountWon = 0,
				AmountDraw = 0,
				AmountLost = 0,
				Country = model.Country
			});

			await _signInManager.SignInAsync(account, false);

			var roles = await _userManager.GetRolesAsync(account);


			var token = _accountService.CreateJwtToken(
				accountId,
				model.Username,
				roles.Select(role => new Claim(ClaimTypes.Role, role)),
				_configuration.GetValue<string>("SigningKey"),
				_configuration.GetValue<string>("Issuer"),
				_configuration.GetValue<string>("Audience"),
				_configuration.GetValue<int>("TokenTimeoutMinutes")
			);

			return Created("Register", new
			{
				token = new JwtSecurityTokenHandler().WriteToken(token),
				username = model.Username,
				expires = token.ValidTo
			});
		}

		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(value => value.Errors));
			if (model.Email.IsNullOrEmpty() || model.Password.IsNullOrEmpty()) BadRequest("Email or Password is incorrect!");

			var account = await _userManager.FindByEmailAsync(model.Email);
			var result = await _signInManager.PasswordSignInAsync(account.UserName, model.Password, false, false);

			if (!result.Succeeded) return BadRequest(result.ToString());

			var roles = await _userManager.GetRolesAsync(account);

			if (!result.Succeeded) return BadRequest("User does not have a valid role");

			var token = _accountService.CreateJwtToken(
				account.Id,
				account.UserName,
				roles.Select(role => new Claim(ClaimTypes.Role, role)),
				_configuration.GetValue<string>("SigningKey"),
				_configuration.GetValue<string>("Issuer"),
				_configuration.GetValue<string>("Audience"),
				_configuration.GetValue<int>("TokenTimeoutMinutes")
			);

			return Ok(new { 
				token = new JwtSecurityTokenHandler().WriteToken(token),
				username = account.UserName,
				expires = token.ValidTo
			});
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			if (!ModelState.IsValid) return BadRequest("Supplied bad model");

			var id = _accountService.GetAccountId(Request.Headers["Authorization"]);
			var claims = await _userManager.GetClaimsAsync(new Account { Id = id });
			HttpContext.User = new ClaimsPrincipal (
				new ClaimsIdentity(claims) 
			);

			await _signInManager.SignOutAsync();

			return Ok();
		}

		public class LoginModel
		{
			[Required(ErrorMessage = "This field is required")]
			[RegularExpression("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])",
				ErrorMessage = "This is not a valid email")]
			public string Email { get; set; }

			[Required(ErrorMessage = "This field is required")]
			//[MinLength(12, ErrorMessage = "Your password must be longer than 11 characters.")]
			[MaxLength(128, ErrorMessage = "Your password can't be longer than 128 characters.")]
			public string Password { get; set; }
		}

		public class RegisterModel
		{
			[Required(ErrorMessage = "This field is required")]
			[MinLength(3, ErrorMessage = "Your password must be longer than 2 characters.")]
			[MaxLength(16, ErrorMessage = "Your password can't be longer than 16 characters.")]
			[RegularExpression("^(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$",
				ErrorMessage = "This is not a valid username.")]
			public string Username { get; set; }

			[Required(ErrorMessage = "This field is required")]
			[RegularExpression("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])",
				ErrorMessage = "This is not a valid email.")]
			public string Email { get; set; }

			[Required(ErrorMessage = "This field is required")]
			[MinLength(12, ErrorMessage = "Your password must be longer than 11 characters.")]
			[MaxLength(128, ErrorMessage = "Your password can't be longer than 128 characters.")]
			[RegularExpression(
				"^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&])$",
				ErrorMessage = "Your password should at least have one lowercase, one highercase character, one number and one symbol.")]
			public string Password { get; set; }
			public string Country { get; set; }
		}
	}
}