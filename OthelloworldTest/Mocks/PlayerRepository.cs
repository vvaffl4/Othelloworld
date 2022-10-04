using Moq;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloworldTest.Mocks
{
	public static class PlayerRepository
	{
		public static Player PlayerWithGame = new Player
		{
			Username = "PlayerWithGame",
			PlayerInGame = new PlayerInGame[]
			{
				new PlayerInGame
				{
					GameToken = "PlayerWithGameToken"
				}
			}
		};
		public static Player PlayerWithoutGame = new Player
		{
			Username = "PlayerWithoutGame"
		};
		public static Player UserManagerPlayer = new Player
		{
			Username = "UserManager",
			PlayerInGame = new PlayerInGame[]
			{
				new PlayerInGame
				{
					GameToken = "UserManagerGameToken"
				}
			}
		};

		public static Mock<IPlayerRepository> New()
		{
			var playerRepositoryMock = new Mock<IPlayerRepository>();

			playerRepositoryMock.Setup(x => x.GetPlayer(It.Is<string>(PlayerWithGame.Username, StringComparer.Ordinal)))
				.Returns(PlayerWithGame);

			playerRepositoryMock.Setup(x => x.GetPlayer(It.Is<string>(PlayerWithoutGame.Username, StringComparer.Ordinal)))
				.Returns(PlayerWithoutGame);

			playerRepositoryMock.Setup(x => x.GetPlayer(It.Is<string>(UserManagerPlayer.Username, StringComparer.Ordinal)))
				.Returns(UserManagerPlayer);

			return playerRepositoryMock;
		}
	}
}
