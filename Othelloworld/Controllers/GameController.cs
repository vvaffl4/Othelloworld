using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Othelloworld.Controllers.Models;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using Othelloworld.Services;
using Othelloworld.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Othelloworld.Controllers
{
	[Authorize(Policy = JwtBearerDefaults.AuthenticationScheme, Roles = "user, mod, admin")]
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
		public async Task<ActionResult<PagedList<Game>>> GetGamesAsync([FromQuery] int pageNumber, int pageSize)
		{
			return await _gameRepository.GetGames(pageNumber, pageSize);
		}

		[HttpGet("history/{username}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<IEnumerable<Game>>> GetGameHistoryAsync(string username)
		{
			var games = await _gameRepository.GetGameHistory(username);

			return Ok(games);
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
			string id;
			try
			{
				id = _accountService.GetAccountId(Request.Headers["Authorization"]);
			} catch (Exception exception)
			{
				// Log Exception
				_logger.LogWarning(exception, "Authentication failed: Validating JWE Token");

				// Return as little information as possible
				return Unauthorized();
			}

			var account = await _userManager.FindByIdAsync(id);

			if (account == null) return BadRequest("Token is invalid");

			account.Player = _playerRepository.GetPlayer(account.UserName);

			if (!account.Player.PlayerInGame.Any()) return BadRequest("Player doesn't have a game");

			var game = _gameRepository.GetGame(account.Player.PlayerInGame.First().GameToken);

			return Ok(game);
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
			if (!ModelState.IsValid) return BadRequest("Invalid model state for game");

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

			if (account == null) return BadRequest("Token is invalid");

			var player = _playerRepository.GetPlayer(account.UserName);

			if (player.PlayerInGame.Any()) return BadRequest("Player already has a game");

			var game = _gameService.CreateNewGame(account.UserName, model.Name, model.Description);

			_gameRepository.CreateGame(game);
			return Created("CreateGame", game);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("join")]
		[Consumes("application/json")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Game>> JoinGameAsync([FromBody] JoinGameModel model)
		{
			if (model == null) return BadRequest("Game is null");
			if (model.Token == null) return BadRequest("Invalid model state for game");

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

			if (account == null) return BadRequest("Token is invalid");

			//var game = _gameRepository.GetGame(model.Token);

			//if (game == null) return BadRequest("Game Token is invalid");

			//game.Players.Add(new PlayerInGame
			//{
			//	Username = account.UserName,
			//	GameToken = game.Token,
			//	IsHost = false,
			//	Color = Color.white
			//});
			//game.Status = GameStatus.Playing;

			//_gameRepository.Update(game);

			var game = await _gameRepository.Context.Games
				.Where(game => game.Token == model.Token)
				.Include(game => game.Players)
				.FirstOrDefaultAsync();

			game.Players.Add(new PlayerInGame
			{
				Username = account.UserName,
				GameToken = game.Token,
				IsHost = false,
				Color = Color.black
			});
			game.Status = GameStatus.Playing;

			await _gameRepository.Context.SaveChangesAsync();

			var resultGame = _gameRepository.GetGame(model.Token);

			return Ok(resultGame);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <returns>Game</returns>
		[HttpPut]
		[Consumes("application/json")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Game>> PutStoneAsync([FromBody] int[] position)
		{
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

			if (position.Length != 2) return BadRequest("Position is invalid");
			var pos = (position[0], position[1]);

			var account = await _userManager.FindByIdAsync(id);
			account.Player = _playerRepository.GetPlayer(account.UserName);

			if (account.Player.PlayerInGame.First() == null) return NotFound("Player doesn't have a game");
							
			var game = _gameRepository.GetGame(account.Player.PlayerInGame.First().GameToken);

			if (game.PlayerTurn != account.Player.PlayerInGame.First().Color) return BadRequest("Not your turn");

			var gameService = new GameService();
			try
			{
				var result = gameService.PutStone(game, pos);

				if (gameService.Finished(result))
				{
					result.Status = GameStatus.Finished;
					var (none, white, black) = gameService.CountColors(game);

					if (white > black)
					{
						var winner = result.Players.First(pig => pig.Color == Color.white);
						winner.Result = GameResult.won;
						winner.Player.AmountWon = winner.Player.AmountWon + 1;

						var loser = result.Players.First(pig => pig.Color == Color.black);
						loser.Result = GameResult.lost;
						loser.Player.AmountLost = winner.Player.AmountLost + 1;
					}
					else if (black > white)
					{
						result.Players.First(pig => pig.Color == Color.black).Result = GameResult.won;
						result.Players.First(pig => pig.Color == Color.white).Result = GameResult.lost;
					}
					else
					{
						result.Players.First(pig => pig.Color == Color.black).Result = GameResult.draw;
						result.Players.First(pig => pig.Color == Color.white).Result = GameResult.draw;
					}
				}

				_gameRepository.UpdateGame(result);

				return Ok(result);
			} catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>Game</returns>
		[HttpPut("pass")]
		[Consumes("application/json")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Game>> Pass()
		{
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
			account.Player = _playerRepository.GetPlayer(account.UserName);

			if (account.Player.PlayerInGame.First() == null) return NotFound("Player doesn't have a game");

			var game = _gameRepository.GetGame(account.Player.PlayerInGame.First().GameToken);

			if (game.PlayerTurn != account.Player.PlayerInGame.First().Color) return BadRequest("Not your turn");

			var gameService = new GameService();
			try
			{
				var playerTurn = game.PlayerTurn;
				var result = gameService.Pass(game);

				if (result.PlayerTurn != playerTurn) { 
					_gameRepository.UpdateGame(result);
				} else
				{
					throw new Exception("Player can put a stone");
				}
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("giveup")]
		public async Task<ActionResult<Game>> GiveUp()
		{
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
			var player = _gameRepository.Context.Players
				.Where(player => player.Username == account.UserName)
				.Include(player => player.PlayerInGame)
				.AsNoTracking()
				.FirstOrDefault();

			if (player == null) return NotFound("Player doesn't have a game");

			var game = _gameRepository.Context.Games
				.Where(game => game.Token == player.PlayerInGame.First().GameToken)
				.Include(game => game.Players)
				.FirstOrDefault();

			var lostPlayer = game.Players.FirstOrDefault(pig => pig.Username == account.UserName);
			var wonPlayer = game.Players.FirstOrDefault(pig => pig.Username != account.UserName);

			lostPlayer.Result = GameResult.lost;
			lostPlayer.Player.AmountLost += 1;

			wonPlayer.Result = GameResult.won;
			wonPlayer.Player.AmountWon += 1;

			game.Status = GameStatus.Finished;

			await _gameRepository.Context.SaveChangesAsync();

			return Ok(game);
		}
	}
}
