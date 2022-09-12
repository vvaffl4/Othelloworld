using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Othelloworld.Data;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using Othelloworld.Services;
using Othelloworld.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Othelloworld.Controllers
{
	[Authorize(Roles = "user, admin")]
	[ApiController]
	[Route("[controller]")]
	public class GameController : Controller
	{
		private readonly IGameRepository _gameRepository;
		private readonly IGameService _gameService;
		private readonly IPlayerRepository _playerRepository;
		private readonly IAccountService _accountService;
		private readonly UserManager<Account> _userManager;
		private readonly ILogger<GameController> _logger;

		public GameController(
			IGameRepository gameRepository,
			IGameService gameService,
			IPlayerRepository playerRepository,
			IAccountService accountService,
			UserManager<Account> userManager,
			ILogger<GameController> logger)
		{
			_gameRepository = gameRepository;
			_gameService = gameService;
			_playerRepository = playerRepository;
			_accountService = accountService;
			_userManager = userManager;
			_logger = logger;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		[HttpGet("search")]
		public ActionResult<IEnumerable<Game>> SearchGames(string search) =>
			_gameRepository.FindByCondition(g =>
				g.Name.Contains(search)
				|| g.Description.Contains(search)).ToList();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pageNumber"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet("pages")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<PagedList<Game>>> GetGames([FromQuery] int pageNumber, int pageSize)
		{
			return await _gameRepository.GetGames(pageNumber, pageSize);
		}

		/// <summary>
		/// Create a game
		/// </summary>
		/// <param name="model"></param>
		/// <returns>Game</returns>
		[HttpPost]
		[Consumes("application/json")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Game>> CreateGameAsync([FromBody] CreateGameModel model)
		{
			if (model == null) return BadRequest("Game is null");
			if (model.Name == null) return BadRequest("Invalid model state for game");

			var id = _accountService.GetAccountId(Request.Headers["Authorization"]);

			var account = await _userManager.FindByIdAsync(id);

			if (account == null) return BadRequest("Token is invalid");

			var game = _gameService.CreateNewGame(account.UserName, model.Name, model.Description);

			_gameRepository.CreateGame(game);
			return Created("CreateGame", game);
		}

		/// <summary>
		/// Gets active game
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Game>> GetGameAsync()
		{
			var id = _accountService.GetAccountId(Request.Headers["Authorization"]);
			var account = await _userManager.FindByIdAsync(id);

			if (account == null) return BadRequest("Token is invalid");

			account.Player = _playerRepository.GetPlayer(account.UserName);

			if (account.Player.PlayerInGame == null) return BadRequest("Player doesn't have a game");

			var game = _gameRepository.GetGame(account.Player.PlayerInGame.GameToken);

			return Ok(game);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		[HttpPut]
		[Consumes("application/json")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Game>> PutStoneAsync([FromBody] int[] position)
		{
			if (position.Length != 2) return BadRequest("Position is invalid");
			var pos = (position[0], position[1]);
			var id = _accountService.GetAccountId(Request.Headers["Authorization"]);

			var account = await _userManager.FindByIdAsync(id);
			account.Player = _playerRepository.GetPlayer(account.UserName);

			if (account.Player.PlayerInGame == null) return BadRequest("Player doesn't have a game");
							
			var game = _gameRepository.GetGame(account.Player.PlayerInGame.GameToken);

			if (game.PlayerTurn != account.Player.PlayerInGame.Color) return BadRequest("Not your turn");

			var gameService = new GameService();
			try
			{
				var result = gameService.PutStone(game, pos);

				_gameRepository.UpdateGame(result);

				return Ok(result);
			} catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		public class CreateGameModel
		{
			public string Name { get; set; }
			public string Description { get; set; }
		}
	}
}
