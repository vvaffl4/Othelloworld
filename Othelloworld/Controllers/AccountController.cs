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

		private string _privateKey = "-----BEGIN RSA PRIVATE KEY-----\r\nMIIBOgIBAAJBAIlam3rj1P7MQu/O11E5u/6h8ndr+FdsRk6813uLwADZFNP6mP5v\r\n81sY2/uk5FyWJReWIJD3IE4upyUHRIRD1nUCAwEAAQJATwb1zCgH5a4KmUVt90r7\r\nkk8FXZaepVYjwau/Y1MN3q4tpfibzqafELG42kBBFwzmwRX/nE4qELhl0NvlblQT\r\nhQIhANn5OxfKSsbsiyPd00tCoS6nGXYstnt9xotga//UZ/aTAiEAoVDg09qBLr6s\r\nnPP/IX6BPgLZmtleGX5wCNu9Pwls29cCIQDIj+jttPclHlXQxLU8lKxWju6ArBek\r\nfVCIwknddgXK/QIgeHkebxlQQMjFwLG4aBtCCj22pZ6QWBnFMdhpjRpM4iECIHry\r\n8y4yJHZqv8zpisBVuWP092u6xUWatdA9hAKxAv7u\r\n-----END RSA PRIVATE KEY-----";
		private string _publicKey = "-----BEGIN PUBLIC KEY-----\r\nMFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAIlam3rj1P7MQu/O11E5u/6h8ndr+Fds\r\nRk6813uLwADZFNP6mP5v81sY2/uk5FyWJReWIJD3IE4upyUHRIRD1nUCAwEAAQ==\r\n-----END PUBLIC KEY-----";
		//		private readonly AccountRepository _repository;
		//		private readonly ILogger<AccountController> _logger;

		//		public AccountController(AccountRepository repo, ILogger<AccountController> logger)
		//		{
		//			_repository = repo;
		//			_logger = logger;
		//		}

		public AccountController(
			IConfiguration configuration,
			UserManager<Account> userManager,
			SignInManager<Account> signInManager)
		{
			_configuration = configuration;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if (!ModelState.IsValid) return BadRequest("Supplied bad model");

			var hasher = new PasswordHasher<Account>();
			var user = new Account
			{
				UserName = model.Username,
				Email = model.Email,
				PasswordHash = hasher.HashPassword(null, model.Password)
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (!result.Succeeded) return BadRequest("Was unable to register");

			await _signInManager.SignInAsync(user, false);

			return Created("Register", new { UserName = model.Username, Email = model.Email });
		}

		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (!ModelState.IsValid) return BadRequest("Supplied bad model");

			var account = await _userManager.FindByEmailAsync(model.Email);
			var result = await _signInManager.PasswordSignInAsync(account.UserName, model.Password, false, false);

			Debug.WriteLine($"LoginModel: {model.Email}, {model.Password}");
			Debug.WriteLine($"Account: {account.UserName}");
			Debug.WriteLine($"Result: {result}");

			if (!result.Succeeded) return BadRequest(result.ToString());

			var roles = await _userManager.GetRolesAsync(account);

			var claims = new List<Claim>();
			claims.Add(new Claim("id", account.Id));
			claims.Add(new Claim("username", account.UserName));

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
		}
	}
}