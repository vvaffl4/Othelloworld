
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Othelloworld.Data;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using System.Threading.Tasks;

//namespace Othelloworld.Controllers
//{
//	[Route("[controller]")]
//	[ApiController]
//	public class LoginController : ControllerBase
//	{
//		private readonly AccountRepository _repository;
//		private readonly ILogger<AccountController> _logger;

//		public LoginController(AccountRepository repo, ILogger<AccountController> logger)
//		{
//			_repository = repo;
//			_logger = logger;
//		}

//		/// <summary>
//		/// Login in account
//		/// </summary>
//		/// <param name="loginInfo"></param>
//		/// <returns></returns>
//		/// <remarks>
//		/// Sample request:
//		///
//		///     POST /Login
//		///     {
//		///        "identifier": "gello1",
//		///        "password": "password"
//		///     }
//		///
//		/// </remarks>
//		[HttpPost]
//		[ActionName("Login")]
//		[Consumes("application/json")]
//		[Produces("application/json")]
//		[ProducesResponseType(StatusCodes.Status200OK)]
//		[ProducesResponseType(StatusCodes.Status400BadRequest)]
//		public ActionResult<Account> Login([FromBody] LoginInfo loginInfo)
//		{
//			return _repository.LoginAccount(loginInfo.identifier, loginInfo.password);
//		}
//	}
//}
