using System.Collections.Generic;
using System;
using Othelloworld.Data.Models;
using System.Linq;
using System.Drawing;
using Color = Othelloworld.Data.Models.Color;
using Duende.IdentityServer.Extensions;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace Othelloworld.Services
{
	public class GameService : IGameService
	{
		private static U Reduce<T, R, U>(T list, Func<U, R, int, U> func, U acc) where T : IEnumerable<R>
		{
			var index = 0;

			foreach (var i in list) { 
				acc = func(acc, i, index);

				index += 1;
			}

			return acc;
		}
		private static U Reduce<T, R, U>(T list, Func<U, R, U> func, U acc) where T : IEnumerable<R>
		{
			foreach (var i in list)
			{
				acc = func(acc, i);
			}

			return acc;
		}

		public static readonly (int x, int y)[] AllDirections = new (int x, int y)[] {
				(-1, -1), (0, -1), (1, -1),
				(-1,  0),          (1,  0),
				(-1,  1), (0,  1), (1,  1)
			};

		public Game CreateNewGame(string userName, string gameName, string gameDescription)
		{
			var gameToken = Guid.NewGuid().ToString();
			return new Game
			{
				Token = gameToken,
				Name = gameName,
				Description = gameDescription,
				PlayerTurn = Color.white,
				Players = new PlayerInGame[]
				{
					new PlayerInGame
					{
						GameToken = gameToken,
						Username = userName,
						IsHost = true
					}
				},
				Board = new Color[8][]
				{
					new Color[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
					new Color[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
					new Color[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
					new Color[8] { 0, 0, 0, Color.white, Color.black, 0, 0, 0 },
					new Color[8] { 0, 0, 0, Color.black, Color.white, 0, 0, 0 },
					new Color[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
					new Color[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
					new Color[8] { 0, 0, 0, 0, 0, 0, 0, 0 }
				}
			};
		}

		public Result CheckIfValid(IEnumerable<IEnumerable<Color>> board, (int x, int y) position, (int x, int y)[] directions, Color turn)
		{
			if (position.y < 0 
				|| position.y > 7
				|| position.x < 0
				|| position.x > 7)
			{
				throw new Exception($"Zet ({position.x},{position.y}) ligt buiten het bord!");
			}

			if (board.ElementAt(position.y).ElementAt(position.x) != Color.none)
			{
				return new Result
				{
					valid = false
				};
			}

			var opponent = turn == Color.white ? Color.black : Color.white;

			return Reduce(directions, (Result result, (int x, int y) direction) =>
			{
				var hasOpponent = false;
				var path = new (int x, int y)[] {};

				for (int index = 1,
					xIndex = index * direction.x + position.x,
					yIndex = index * direction.y + position.y;

					xIndex > -1 && yIndex > -1 && xIndex < 8 && yIndex < 8;

					++index,
					xIndex = index * direction.x + position.x,
					yIndex = index * direction.y + position.y)
				{
					if (board.ElementAt(yIndex).ElementAt(xIndex) == turn && hasOpponent)
					{
						return new Result
						{
							valid = true,
							paths = result.paths.Append( path ).ToArray()
						};
					}
					else if (board.ElementAt(yIndex).ElementAt(xIndex) == opponent)
					{
						hasOpponent = true;
						path = path.Append(( xIndex, yIndex )).ToArray();
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
				paths = new (int x, int y)[][] {
					new (int x, int y)[] { position }
				}
			});
		}

		public Game PutStone(Game game, (int x, int y) position)
		{
			var result = CheckIfValid(game.Board, position, AllDirections, game.PlayerTurn);

			if (result.valid)
			{
				var board = game.BoardArray;

				result.paths.ToList()
					.ForEach(path => path.ToList()
						.ForEach(location =>
							board[location.y][location.x] = game.PlayerTurn
						)
					);

				game.Board = board;

				var isFinished = Finished(game);

				if (isFinished)
				{
					var count = CountColors(game);
				}

				if (game.PlayerTurn == Color.white)
				{
					game.PlayerTurn = Color.black;
				}
				else
				{
					game.PlayerTurn = Color.white;
				}
			} else
			{
				throw new Exception($"Zet ({position.x},{position.y}) is niet mogelijk!");
			}

			return game;
		}

		public Game Pass(Game game)
		{
			var map = CreatePlaceholderMap(game.Board, game.PlayerTurn);

			if (map.IsNullOrEmpty())
			{
				game.PlayerTurn = game.PlayerTurn == Color.white 
					? Color.black 
					: Color.white;
			}

			return game;
		}

		public bool Finished(Game game)
		{
			var whiteMap = CreatePlaceholderMap(game.Board, Color.white);
			var blackMap = CreatePlaceholderMap(game.Board, Color.black);

			if (whiteMap.IsNullOrEmpty() && blackMap.IsNullOrEmpty())
			{
				return true;
			}
			return false;
		}

		public (int none, int white, int black) CountColors(Game game)
		{
			return Reduce(game.Board, ((int none, int white, int black) state, IEnumerable<Color> row) =>
				Reduce(row, ((int none, int white, int black) state, Color cell) =>
					( state.none + (cell == Color.none ? 1 : 0),
						state.white + (cell == Color.white ? 1 : 0),
						state.black + (cell == Color.black ? 1 : 0)
					),
					state)
			, (0, 0, 0));
		}

		public IEnumerable<(int x, int y)> CreatePlaceholderMap (IEnumerable<IEnumerable<Color>> board, Color color) {

			var opponent = color == Color.white ? Color.black : Color.white;
			var placeHolderMap = new Color[8][] {
				new Color[8] {0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8] {0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8] {0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8] {0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8] {0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8] {0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8] {0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8] {0, 0, 0, 0, 0, 0, 0, 0}
			};

			// Every Row
			return Reduce(board, (IEnumerable<(int x, int y)> state, IEnumerable<Color> row, int yIndex) =>
				// Every Column
				Reduce(row, (IEnumerable<(int x, int y)> state, Color cell, int xIndex) =>
					(cell != opponent) 
						? state
						// Every direction
						: Reduce(AllDirections, (IEnumerable<(int x, int y)> state, (int x, int y) direction, int _) =>
							{
								var locX = xIndex + direction.x;
								var locY = yIndex + direction.y;

								if (locX < 0 || locY < 0 || locX > 7 || locY > 7
									|| placeHolderMap[locY][locX] != Color.none
									|| board.ElementAt(locY).ElementAt(locX) != Color.none) return state;

								var result = CheckIfValid(
									board,
									(locX, locY),
									new (int x, int y)[] {
											( xIndex - locX, yIndex - locY )
									},
									color
								);

								if (result.valid)
								{
									placeHolderMap[locY][locX] = (Color)1;
									return state.Append((locX, locY));
								}

								return state;
							}
						, state)
				, state)
			, Array.Empty<(int x, int y)>());
		}
	}
}
