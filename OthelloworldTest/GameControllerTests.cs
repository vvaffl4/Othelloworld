using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Othelloworld.Controllers;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using Othelloworld.Services;
using Othelloworld.Util;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
			.Select(i => new Account { Id = Guid.NewGuid().ToString(), UserName = $"UserName{i}" })
			.ToList();

		private IEnumerable<Game> _mockGames = Enumerable.Range(0, 10)
			.Select(i => new Game { Token = Guid.NewGuid().ToString(), Name = $"Game{i}", Description = $"Description{i}" })
			.ToList();

		[OneTimeSetUp]
		public void StartTest()
		{
			_gameRepositoryMock = new Mock<IGameRepository>();

			_gameRepositoryMock.Setup(x =>
				x.FindByCondition(
					It.IsAny<Expression<Func<Game, bool>>>()
			))
				.Returns<Expression<Func<Game, bool>>>(x => 
					_mockGames.AsQueryable().Where(x).AsNoTracking()
				);

			_gameRepositoryMock.Setup(x => 
				x.GetGames(
					It.IsAny<int>(), 
					It.IsAny<int>()
			))
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
			mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Success)
				.Callback<TUser, string>((x, y) => ls.Add(x));
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

		public bool CheckIfValidModelState<Model>(Model model)
		{
			// Set some properties here
			var context = new ValidationContext(model, null, null);
			var results = new List<ValidationResult>();

			TypeDescriptor.AddProviderTransparent(
				new AssociatedMetadataTypeTypeDescriptionProvider(
					typeof(Model),
					typeof(Model)),
				typeof(Model)
			);

			return Validator.TryValidateObject(model, context, results, true);
		}


		[Test]
		public void SearchGamesFromGameControllerReturnsGameWithNameGame4()
		{
			var gameName = "Game1";
			var controller = CreateMockedGameController();
			var games = controller.SearchGames(gameName);

			// Assert
			Assert.AreEqual(
				_mockGames.First(game => game.Name == gameName)
					.Token,
				games.Value.First().Token
			);
		}

		[Test]
		public async Task GetGamesFromGameControllerReturns10Games()
		{
			var controller = CreateMockedGameController();
			var result = await controller.GetGamesAsync(1, 3);

			// Assert
			_mockGames.Select((game, index) => new {game, index})
				.ToList()
				.ForEach(indexedGame =>
				Assert.AreEqual(indexedGame.game.Name, result.Value[indexedGame.index].Name)
			);
		}

		[Test]
		public async Task CreateNewGameWithNoModelFromGameControllerReturnsBadRequest()
		{
			var controller = CreateMockedGameController();
			var result = await controller.CreateGameAsync(null);
			var actual = result.Result as BadRequestObjectResult;

			Assert.AreEqual(400, actual.StatusCode);
			Assert.AreEqual("Game is null", actual.Value);
		}

		[Test]
		public async Task CreateNewGameWithWrongModelFromGameControllerReturnsBadRequest()
		{
			var createGameModelMock = new GameController.CreateGameModel { Name = "" };

			var controller = CreateMockedGameController();

			if (CheckIfValidModelState(createGameModelMock)) { 
				controller.ModelState.AddModelError("Required", "Invalid model state for game");
			}

			var result = await controller.CreateGameAsync(createGameModelMock);
			var actual = result.Result as BadRequestObjectResult;

			Assert.AreEqual(400, actual.StatusCode);
			Assert.AreEqual("Invalid model state for game", actual.Value);
		}
	}
}
