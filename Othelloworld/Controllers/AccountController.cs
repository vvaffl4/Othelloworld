using Duende.IdentityServer.Extensions;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Othelloworld.Data;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using Othelloworld.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Othelloworld.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly UserManager<Account> _userManager;
		private readonly SignInManager<Account> _signInManager;
		private readonly IPlayerRepository _playerRepository;

		public AccountController(
			IConfiguration configuration,
			UserManager<Account> userManager,
			SignInManager<Account> signInManager,
			IPlayerRepository playerRepository)
		{
			_configuration = configuration;
			_userManager = userManager;
			_signInManager = signInManager;
			_playerRepository = playerRepository;
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if (!ModelState.IsValid) return BadRequest("Supplied bad model");

			var hasher = new PasswordHasher<Account>();
			var user = new Account
			{
				Id = Guid.NewGuid().ToString(),
				UserName = model.Username,
				NormalizedUserName = model.Username,
				Email = model.Email,
				NormalizedEmail = model.Email,
				EmailConfirmed = true,
				PasswordHash = hasher.HashPassword(null, model.Password),
				SecurityStamp = String.Empty
			};

			var accountResult = await _userManager.CreateAsync(user, model.Password);

			if (!accountResult.Succeeded) return BadRequest(accountResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

			var roleResult = await _userManager.AddToRoleAsync(user, "user");

			if (!roleResult.Succeeded) return BadRequest(roleResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

			_playerRepository.CreatePlayer(new Player
			{
				Username = model.Username,
				AmountWon = 0,
				AmountDraw = 0,
				AmountLost = 0,
				Country = model.Country
			});

			await _signInManager.SignInAsync(user, false);

			return Created("Register", new { UserName = model.Username, Email = model.Email });
		}

		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (!ModelState.IsValid) return BadRequest("Supplied bad model");
			if (model.Email.IsNullOrEmpty() || model.Password.IsNullOrEmpty()) BadRequest("Email or Password is incorrect!");

			var account = await _userManager.FindByEmailAsync(model.Email);
			var result = await _signInManager.PasswordSignInAsync(account.UserName, model.Password, false, false);

			if (!result.Succeeded) return BadRequest(result.ToString());

			var roles = await _userManager.GetRolesAsync(account);

			var claims = new List<Claim>
			{
				new Claim("id", account.Id),
				new Claim("username", account.UserName)
			};

			// Add roles as multiple claims
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var token = JwtHelper.GetJwtToken(
				account.Id,
				_configuration.GetValue<string>("SigningKey"),
				_configuration.GetValue<string>("Issuer"),
				_configuration.GetValue<string>("Audience"),
				TimeSpan.FromMinutes(_configuration.GetValue<int>("TokenTimeoutMinutes")),
				claims);

			return Ok(new { 
				token = new JwtSecurityTokenHandler().WriteToken(token),
				username = account.UserName,
				expires = token.ValidTo
			});
		}
		//		/// <summary>
		//		/// Create a new account
		//		/// </summary>
		//		/// <param name="account"></param>
		//		/// <returns></returns>
		//		/// <remarks>
		//		/// 
		//		/// </remarks>
		//		/// <response code="201">Returns newly created account</response>
		//		/// <response code="400">If the item is null</response>
		//		[HttpPost]
		//		[Consumes("application/json")]
		//		[Produces("application/json")]
		//		[ProducesResponseType(StatusCodes.Status201Created)]
		//		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		//		public ActionResult<Account> Create(
		//				[FromBody] Account account)
		//		{
		//			_repository.CreateAccount(account);

		//			return Created("Account/Create", account);
		//		}

		//		/// <summary>
		//		///     Get weather forecast for the next 5 days
		//		/// </summary>
		//		/// <path name="username"></path>
		//		/// <returns>JSON array of weatherforecast information.</returns>
		//		/// <remarks>
		//		/// Sample request:
		//		///
		//		///     POST /Todo
		//		///     {
		//		///        "id": 1,
		//		///        "name": "Item #1",
		//		///        "isComplete": true
		//		///     }
		//		///
		//		/// </remarks>
		//		/// <response code="200">Returns found item</response>
		//		/// <response code="400">If the item is null</response>
		//		[HttpGet("{username: }")]
		//		[Produces("application/json")]
		//		[ProducesResponseType(StatusCodes.Status200OK)]
		//		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		//		public ActionResult<Account> Get(string username)
		//		{
		//			return _repository
		//				.FindByCondition(account => account.Username == username)
		//				.FirstOrDefault();
		//		}

		//	}

		public class LoginModel
		{
			public string Email { get; set; }
			public string Password { get; set; }
		}

		public class RegisterModel
		{
			public string Username { get; set; }
			public string Email { get; set; }
			public string Password { get; set; }
			public string Country { get; set; }
		}
	}
}