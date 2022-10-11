using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using System.Threading.Tasks;

namespace Othelloworld.Controllers
{
	[Authorize(Roles = "user, admin")]
	[ApiController]
	[Route("[controller]")]
	public class PlayerController : Controller
	{
		private readonly IPlayerRepository _playerRepository;

		public PlayerController(IPlayerRepository playerRepository)
		{
			_playerRepository = playerRepository;
		}
		[HttpGet("{username}")]
		public async Task<ActionResult<Player>> GetAsync(string username)
		{
			if (!ModelState.IsValid || username.IsNullOrEmpty()) return BadRequest("Provided params are incorrect");

			Player player = await _playerRepository.FindByCondition(player => player.Username == username)
				.FirstAsync();

			return Ok(player);
		}
	}
}
