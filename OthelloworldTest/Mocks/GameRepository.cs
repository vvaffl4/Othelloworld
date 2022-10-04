using Moq;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using Othelloworld.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OthelloworldTest.Mocks
{	
	public static class GameRepository
	{
		public static List<Game> Games = Enumerable.Range(0, 10)
			.Select(i => new Game { Token = Guid.NewGuid().ToString(), Name = $"Game{i}", Description = $"Description{i}" })
			.ToList();
		public static Game UserManagerGame = new Game
		{
			Token = "UserManagerGameToken",
			Name = "UserManagerGame"
		};

		public static Mock<IGameRepository> New()
		{
			var gameRepositoryMock = new Mock<IGameRepository>();

			gameRepositoryMock.Setup(x =>
				x.FindByCondition(
					It.IsAny<Expression<Func<Game, bool>>>()
			))
				.Returns<Expression<Func<Game, bool>>>(x =>
					Games.AsQueryable().Where(x)
				);

			gameRepositoryMock.Setup(x => 
				x.GetGame(It.Is<string>("UserManagerGameToken", StringComparer.Ordinal)))
				.Returns(UserManagerGame);

			gameRepositoryMock.Setup(x =>
				x.GetGames(
					It.IsAny<int>(),
					It.IsAny<int>()
			))
				.ReturnsAsync((int pageNumber, int pageSize) =>
					PagedList<Game>.GetPagedList(
						Games.AsQueryable(),
						pageNumber,
						pageSize
				));

			gameRepositoryMock.Setup(x =>
				x.CreateGame(It.IsAny<Game>())
			);

			return gameRepositoryMock;
		}
	}
}
