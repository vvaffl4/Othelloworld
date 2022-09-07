using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Othelloworld.Data;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using Othelloworld.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
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
		private readonly IPlayerRepository _playerRepository;
		private readonly UserManager<Account> _userManager;
		private readonly ILogger<GameController> _logger;

		public GameController(
			IGameRepository gameRepository,
			IPlayerRepository playerRepository,
			UserManager<Account> userManager,
			ILogger<GameController> logger)
		{
			_gameRepository = gameRepository;
			_playerRepository = playerRepository;
			_userManager = userManager;
			_logger = logger;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		[HttpGet("/search")]
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
		[HttpGet]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<PagedList<Game>>> GetGames([FromQuery] int pageNumber, int pageSize)
		{
			//var authorization = Request.Headers["Authorization"]
			//	.FirstOrDefault()
			//	.Replace("bearer ", "", true, CultureInfo.CurrentCulture);

			//var token = JwtHelper.ReadJwtToken(authorization);

			//var id = token.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;

			//var account = await _userManager.FindByIdAsync(id);

			//Debug.WriteLine($"Account: {account.UserName}");

			return await _gameRepository.GetGames(pageNumber, pageSize);
		}

		/// <summary>
		/// Create a game
		/// </summary>
		/// <param name="game"></param>
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

			var authorization = Request.Headers["Authorization"]
				.FirstOrDefault()
				.Replace("bearer ", "", true, CultureInfo.CurrentCulture);

			var token = JwtHelper.ReadJwtToken(authorization);

			var id = token.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;

			var account = await _userManager.FindByIdAsync(id);

			var gameToken = Guid.NewGuid().ToString();
			var game = new Game
			{
				Token = gameToken,
				Name = model.Name,
				Description = model.Description,
				PlayerTurn = Color.white,
				Players = new PlayerInGame[]
				{
					new PlayerInGame
					{
						GameToken = gameToken,
						Username = account.UserName,
						IsHost = true
					}
				},
				Board = new int[8][]
				{
					new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
					new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
					new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
					new int[8] { 0, 0, 0, 1, 2, 0, 0, 0 },
					new int[8] { 0, 0, 0, 2, 1, 0, 0, 0 },
					new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
					new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
					new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 }
				}
			};

			_gameRepository.CreateGame(game);
			return Created("CreateGame", game);
		}

		/// <summary>
		/// Gets active game
		/// </summary>
		/// <returns></returns>
		[HttpGet("{token}")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<Game> GetGame(string token)
		{
			if (token.Length < 1 || token == null) return BadRequest("Invalid token");
			var game = _gameRepository.GetGame(token);
			if (game == null) return NotFound("Game " + token + " does not exist");
			return Ok(game);
		}

		private class Result
		{
			public bool valid;
			public int[][][] paths;
		}

		static U Reduce<T, R, U>(T list, Func<U, R, U> func, U acc) where T : IEnumerable<R>
		{
			foreach (var i in list)
				acc = func(acc, i);

			return acc;
		}

		private Result checkIfValid(int[][] board, int[] position, int turn)
		{
			var directions = new int[8][] {
				new int[2]{-1, -1}, new int[2]{0, -1}, new int[2]{1, -1},
				new int[2]{-1,  0},                    new int[2]{1,  0},
				new int[2]{-1,  1}, new int[2]{0,  1}, new int[2]{1,  1}
			};

			if (board[position[1]][position[0]] != (int)Color.none)
			{
				return new Result
				{
					valid = false
				};
			}

			return Reduce<int[][], int[], Result>(directions, (result, direction) =>
			{
				var hasOpponent = false;
				var path = Array.Empty<int[]>();

				for (int index = 1,
					xIndex = index * direction[0] + position[0],
					yIndex = index * direction[1] + position[1];

					xIndex > -1 && yIndex > -1 && xIndex < 8 && yIndex < 8;

					++index,
					xIndex = index * direction[0] + position[0],
					yIndex = index * direction[1] + position[1])
				{
					if (board[yIndex][xIndex] == turn + 1 && hasOpponent)
					{
						return new Result
						{
							valid = true,
							paths = result.paths.Concat(new int[][][] { path }).ToArray()

						};
					}
					else if (board[yIndex][xIndex] == 1 - turn + 1)
					{
						hasOpponent = true;
						path = path.Concat(new int[][] { new int[] { xIndex, yIndex } }).ToArray();
					}
					else
					{
						break;
					}
				}

				return result;
			},
				new Result
				{
					valid = false,
					paths = new int[][][] {
						new int[][] { position }
					}
				});
		}

		[HttpPut("{gameToken}/{accountToken}")]
		[Consumes("application/json")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Game>> PutStoneAsync(string gameToken, string accountToken, [FromBody] int[] position)
		{
			var authorization = Request.Headers["Authorization"]
				.FirstOrDefault()
				.Replace("bearer ", "", true, CultureInfo.CurrentCulture);

			var token = JwtHelper.ReadJwtToken(authorization);

			var id = token.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;

			var account = await await _userManager.FindByIdAsync(id)
				.ContinueWith(async task =>
				{
					var account = await task;
					account.Player = _playerRepository.GetPlayer(account.UserName);
					account.Player.PlayerInGame.Game = _gameRepository.GetGame(account.Player.PlayerInGame.GameToken);
					return account;
				});
				
			var game = account.Player.PlayerInGame.Game;

			if (game.PlayerTurn != account.Player.PlayerInGame.Color) return BadRequest("Not your turn");
			
			var result = checkIfValid(game.Board, position, (int)game.PlayerTurn - 1);

			if (result.valid)
			{
				var board = game.Board;

				result.paths.ToList()
					.ForEach(path => path.ToList()
						.ForEach(location =>
							board[location[1]][location[0]] = (int)game.PlayerTurn
						)
					);

				game.Board = board;
				if (game.PlayerTurn == Color.white)
				{
					game.PlayerTurn = Color.black;
				}
				else
				{
					game.PlayerTurn = Color.white;
				}

				_gameRepository.UpdateGame(game);
				return Ok(game);
			}

			return BadRequest("Not a valid action.");
		}

		public class CreateGameModel
		{
			public string Name { get; set; }
			public string Description { get; set; }
		}
	}
}
