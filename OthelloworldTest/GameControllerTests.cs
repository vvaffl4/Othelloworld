using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Othelloworld.Controllers;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using Othelloworld.Services;
using Othelloworld.Util;
using System.Diagnostics;
using Assert = NUnit.Framework.Assert;

namespace OthelloworldTest
{

	[TestFixture]
	public class GameControllerTest
	{
		public Mock<IGameRepository> _gameRepositoryMock;
		private Mock<IGameService> _gameServiceMock;
		private Mock<IPlayerRepository> _playerRepositoryMock;
		private Mock<IAccountService> _accountServiceMock;
		private Mock<UserManager<Account>> _userManagerMock;
		private Mock<ILogger<GameController>> _loggerMock;

		private IEnumerable<Account> _mockAccounts = Enumerable.Range(0, 10)
			.Select(i => new Account { Id = Guid.NewGuid().ToString(), UserName = $"UserName{i}" });

		private IEnumerable<Game> _mockGames = Enumerable.Range(0, 10)
			.Select(i => new Game { Token = Guid.NewGuid().ToString(), Name = $"Game{i}" });

		[OneTimeSetUp]
		public void StartTest()
		{
			_gameRepositoryMock = new Mock<IGameRepository>();

			_gameRepositoryMock.Setup(x => x.GetGames(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new PagedList<Game>(
					_mockGames.ToList(),
					It.IsAny<int>(),
					It.IsAny<int>(),
					It.IsAny<int>()
				));

			_gameServiceMock = new Mock<IGameService>();
			_playerRepositoryMock = new Mock<IPlayerRepository>();
			_accountServiceMock = new Mock<IAccountService>();
			_loggerMock = new Mock<ILogger<GameController>>();

			_userManagerMock = MockUserManager(
				_mockAccounts.ToList()
			);

			Trace.Listeners.Add(new ConsoleTraceListener());
		}

		public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
		{
			var store = new Mock<IUserStore<TUser>>();
			var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
			mgr.Object.UserValidators.Add(new UserValidator<TUser>());
			mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

			mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
			mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
			mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

			return mgr;
		}

		[OneTimeTearDown]
		public void EndTest()
		{
			Trace.Flush();
		}

		public GameController CreateMockedGameController() =>
			new GameController(
				_gameRepositoryMock.Object,
				_gameServiceMock.Object,
				_playerRepositoryMock.Object,
				_accountServiceMock.Object,
				_userManagerMock.Object,
				_loggerMock.Object);

		[Test]
		public async Task ZetMogelijk_StartSituatieZet23Zwart_ReturnTrueAsync()
		{
			var controller = CreateMockedGameController();
			var games = await controller.GetGamesAsync(1, 3);

			// Assert
			_mockGames.Select((game, index) => new {game, index})
				.ToList()
				.ForEach(indexedGame =>
				Assert.AreEqual(indexedGame.game.Name, games.Value[indexedGame.index].Name)
			);
		}
	}
}
