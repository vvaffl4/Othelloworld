using Moq;
using Othelloworld.Data.Models;
using Othelloworld.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloworldTest.Mocks
{
	public static class GameService
	{
		public static Mock<IGameService> New()
		{
			var gameServiceMock = new Mock<IGameService>();

			gameServiceMock.Setup(x =>
				x.CreateNewGame(
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>()
				))
					.Returns((string username, string gameName, string gameDescription) =>
						new Game
						{
							Name = gameName,
							Description = gameDescription,
							Players = new PlayerInGame[]
							{
								new PlayerInGame
								{
									Username = username,
									Color = Color.white
								}
							}
						});

			return gameServiceMock;
		}
	}
}
