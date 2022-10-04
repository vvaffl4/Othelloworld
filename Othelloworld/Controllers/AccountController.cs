using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Othelloworld.Controllers.Models;
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

	}
}