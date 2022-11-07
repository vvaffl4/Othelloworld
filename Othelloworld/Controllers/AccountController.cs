using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Othelloworld.Controllers.Models;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using Othelloworld.Services;
using Othelloworld.Util;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Othelloworld.Controllers
{
	[Authorize(Policy = JwtBearerDefaults.AuthenticationScheme, Roles = "user, mod, admin")]
	[ApiController]
	[Route("[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly HttpClient _httpClient;
		private readonly UserManager<Account> _userManager;
		private readonly SignInManager<Account> _signInManager;
		private readonly IAccountService _accountService;
		private readonly IMailService _mailService;
		private readonly IPlayerRepository _playerRepository;
		private readonly Credentials _credentials;
		private readonly ILogger<AccountController> _logger;

		public AccountController(
			HttpClient httpClient,
			UserManager<Account> userManager,
			SignInManager<Account> signInManager,
			IAccountService accountService,
			IMailService mailService,
			IPlayerRepository playerRepository,
			Credentials credentials,
			ILogger<AccountController> logger)
		{
			_httpClient = httpClient;
			_userManager = userManager;
			_signInManager = signInManager;
			_accountService = accountService;
			_mailService = mailService;
			_playerRepository = playerRepository;
			_credentials = credentials;
			_logger = logger;
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if (model == null) return BadRequest("Invalid body");
			if (!ModelState.IsValid) return BadRequest("Supplied bad model");
			if (!(await _accountService.VerifyCaptchaTokenAsync(model.captchaToken))) return BadRequest("Captcha is invalid");

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

		[HttpPut("changepassword")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
		{
			if (model == null) return BadRequest("Invalid body");
			if (!ModelState.IsValid) return BadRequest("Invalid model state");
			if (!(await _accountService.VerifyCaptchaTokenAsync(model.captchaToken))) return BadRequest("Captcha is invalid");

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

			var player = _playerRepository.GetPlayer(account.UserName);

			if (player == null) return BadRequest("Player doesn't exist");

			var roles = await _userManager.GetRolesAsync(account);

			var token = _accountService.CreateJwtToken(
				account.Id,
				account.UserName,
				roles.Select(role => new Claim(ClaimTypes.Role, role))
			);

			return Ok(new
			{
				token = new JwtSecurityTokenHandler().WriteToken(token),
				player = player,
				expires = token.ValidTo
			});
		}

		[AllowAnonymous]
		[HttpPost("forgotpassword")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
		{
			if (model == null) return BadRequest("Invalid body");
			if (!ModelState.IsValid) return BadRequest("Invalid model state");
			if (!(await _accountService.VerifyCaptchaTokenAsync(model.CaptchaToken))) return BadRequest("Captcha is invalid");

			var account = await _userManager.FindByEmailAsync(model.Email);

			if (account == null) return Unauthorized(new { errors = new { email = new string[] { "Email is invalid" } } });

			var urlEncodedEmail = HttpUtility.UrlEncode(model.Email);
			var resetToken = await _userManager.GeneratePasswordResetTokenAsync(account);
			var urlEncodedResetToken = HttpUtility.UrlEncode(resetToken);
			var host = HttpContext.Request.Host.Value;

			_mailService.SendMail(
				"passwordrecovery@othelloworld.hbo-ict.org",
				"Recover your password!",
				$"Blablabla, password, blablabla recover, blablabla, click this link: {host}/recoverpassword/{urlEncodedEmail}/{urlEncodedResetToken}");

			return Ok();
		}

		[AllowAnonymous]
		[HttpPost("recoverpassword")]
		public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordModel model)
		{
			if (model == null) return BadRequest("Invalid body");
			if (!ModelState.IsValid) return BadRequest("Invalid model state");
			if (!(await _accountService.VerifyCaptchaTokenAsync(model.captchaToken))) return BadRequest("Captcha is invalid");

			var account = await _userManager.FindByEmailAsync(model.Email);

			if (account == null) return Unauthorized();

			var result = await _userManager.ResetPasswordAsync(account, model.ResetToken, model.NewPassword);

			if (!result.Succeeded) return BadRequest("Password recovery information is incorrect");

			var roles = await _userManager.GetRolesAsync(account);

			if (!result.Succeeded) return BadRequest("User does not have a valid role");

			var player = _playerRepository.GetPlayer(account.UserName);

			if (player == null) return BadRequest("Player doesn't exist");

			var token = _accountService.CreateJwtToken(
				account.Id,
				account.UserName,
				roles.Select(role => new Claim(ClaimTypes.Role, role))
			);

			return Ok(new
			{
				token = new JwtSecurityTokenHandler().WriteToken(token),
				player = player,
				expires = token.ValidTo
			});
		}

		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(value => value.Errors));
			if (model.Email.IsNullOrEmpty() || model.Password.IsNullOrEmpty()) return BadRequest(new { errors = new { password = new string[] { "Login info is invalid" } } });

			var account = await _userManager.FindByEmailAsync(model.Email);

			if (account == null) return BadRequest(new { errors = new { password = new string[] { "Login info is invalid" } } });

			var result = await _signInManager.PasswordSignInAsync(account.UserName, model.Password, false, false);

			if (!result.Succeeded) return BadRequest(new { errors = new { password = new string[] { "Login info is invalid" }}});

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