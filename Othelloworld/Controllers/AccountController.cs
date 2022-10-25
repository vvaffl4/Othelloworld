using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Othelloworld.Controllers.Models;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using Othelloworld.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Othelloworld.Controllers
{
	[Authorize(Policy = JwtBearerDefaults.AuthenticationScheme, Roles = "user, mod, admin")]
	[ApiController]
	[Route("[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<Account> _userManager;
		private readonly SignInManager<Account> _signInManager;
		private readonly IAccountService _accountService;
		private readonly IPlayerRepository _playerRepository;
		private readonly ILogger<AccountController> _logger;

		public AccountController(
			UserManager<Account> userManager,
			SignInManager<Account> signInManager,
			IAccountService accountService,
			IPlayerRepository playerRepository,
			ILogger<AccountController> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_accountService = accountService;
			_playerRepository = playerRepository;
			_logger = logger;
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
				PasswordHash = hasher.HashPassword(null, model.Password)
			};

			var accountResult = await _userManager.CreateAsync(account, model.Password);

			if (!accountResult.Succeeded) return BadRequest(accountResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

			var roleResult = await _userManager.AddToRoleAsync(account, "user");

			if (!roleResult.Succeeded) return BadRequest(roleResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

			var player = new Player
			{
				Username = model.Username,
				AmountWon = 0,
				AmountDraw = 0,
				AmountLost = 0,
				CountryCode = model.Country
			};

			_playerRepository.CreatePlayer(player);

			await _signInManager.SignInAsync(account, false);

			var roles = await _userManager.GetRolesAsync(account);


			var token = _accountService.CreateJwtToken(
				accountId,
				model.Username,
				roles.Select(role => new Claim(ClaimTypes.Role, role))
			);

			return Created("Register", new
			{
				token = new JwtSecurityTokenHandler().WriteToken(token),
				player = player,
				//username = model.Username,
				expires = token.ValidTo
			});
		}

		[HttpPut]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
		{
			if (model == null) return BadRequest("Invalid body");
			if (!ModelState.IsValid) return BadRequest("Invalid model state");

			string id;
			try
			{
				id = _accountService.GetAccountId(Request.Headers["Authorization"]);
			}
			catch (Exception exception)
			{
				// Log Exception
				_logger.LogWarning(exception, "Authentication failed: Validating JWE Token");

				// Return as little information as possible
				return Unauthorized();
			}

			var account = await _userManager.FindByIdAsync(id);

			if (account == null) return Unauthorized();

			var result = await _userManager.ChangePasswordAsync(account, model.CurrentPassword, model.NewPassword);

			if (!result.Succeeded) return BadRequest(result.Errors);

			return Ok();
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

			var player = _playerRepository.GetPlayer(account.UserName);

			if (player == null) return BadRequest("Player doesn't exist");

			var token = _accountService.CreateJwtToken(
				account.Id,
				account.UserName,
				roles.Select(role => new Claim(ClaimTypes.Role, role))
			);

			return Ok(new { 
				token = new JwtSecurityTokenHandler().WriteToken(token),
				player = player,
				//username = account.UserName,
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