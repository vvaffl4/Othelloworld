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
using System.Linq.Expressions;
using Assert = NUnit.Framework.Assert;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Othelloworld.Controllers.Models;

#nullable disable

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
		private Mock<HttpRequest> _requestMock;
		private Mock<ILogger<GameController>> _loggerMock;

		private IEnumerable<Account> _mockAccounts = Enumerable.Range(0, 10)
			.Select(i => new Account { Id = Guid.NewGuid().ToString(), UserName = $"UserName{i}" })
			.ToList();

		[OneTimeSetUp]
		public void StartTest()
		{
			_gameRepositoryMock = Mocks.GameRepository.New();
			_gameServiceMock = Mocks.GameService.New();

			_playerRepositoryMock = Mocks.PlayerRepository.New();
			_accountServiceMock = Mocks.AccountService.New();

			_loggerMock = new Mock<ILogger<GameController>>();

			_userManagerMock = Mocks.UserManager.New(
				_mockAccounts.ToList()
			);

			//only do this if you want to use request object in your tests
			//var returnUrl = new Uri("http://www.example.com");
			//var httpContext = new Mock<HttpContext>();
			_requestMock = new Mock<HttpRequest>();

			//httpContext.Setup(x => x.Request).Returns(request.Object);
			_requestMock.Setup(x => x.Headers).Returns(() =>
			{
				var headerDictionary = new HeaderDictionary();
				headerDictionary.Append("Authorization", "Bearer abc");
				return headerDictionary;
			});

			Trace.Listeners.Add(new ConsoleTraceListener());
		}

		[OneTimeTearDown]
		public void EndTest()
		{
			Trace.Flush();
		}

		public GameController CreateMockedGameController()
		{
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Headers["Authorization"] = "Bearer token";
			
			return new GameController(
				_gameRepositoryMock.Object,
				_gameServiceMock.Object,
				_playerRepositoryMock.Object,
				_accountServiceMock.Object,
				_userManagerMock.Object,
				_loggerMock.Object)
			{
				 ControllerContext = new ControllerContext
				 {
					 HttpContext = httpContext
				 }
			};
		}

		public IEnumerable<ValidationResult> ValidateModelState<Model>(Model model)
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

			Validator.TryValidateObject(model, context, results, true);

			return results;
		}


		//[Test]
		//public void SearchGamesFromGameControllerReturnsGameWithNameGame4()
		//{
		//	var gameName = "Game1";
		//	var controller = CreateMockedGameController();
		//	var games = controller.SearchGames(gameName);

		//	// Assert
		//	Assert.AreEqual(
		//		Mocks.GameRepository.Games.First(game => game.Name == gameName)
		//			.Token,
		//		games.Value.First().Token
		//	);
		//}

		//[Test]
		//public async Task GetGamesFromGameControllerReturnsFirstPageWith3Games()
		//{
		//	var controller = CreateMockedGameController();
		//	var result = await controller.GetGamesAsync(1, 3);

		//	// Assert
		//	Mocks.GameRepository.Games.GetRange(0, 3)
		//		.Select((game, index) => new {game, index})
		//		.ToList()
		//		.ForEach(indexedGame =>
		//			Assert.AreEqual(indexedGame.game.Name, result.Value[indexedGame.index].Name)
		//		);
		//}

		//[Test]
		//public async Task GetGamesFromGameControllerReturnsSecondPageWith4Games()
		//{
		//	var controller = CreateMockedGameController();
		//	var result = await controller.GetGamesAsync(2, 4);

		//	// Assert
		//	Mocks.GameRepository.Games.GetRange(4, 4)
		//		.Select((game, index) => new { game, index })
		//		.ToList()
		//		.ForEach(indexedGame =>
		//			Assert.AreEqual(indexedGame.game.Name, result.Value[indexedGame.index].Name)
		//		);
		//}

		//[Test]
		//public async Task GetGamesFromGameControllerReturnsThirdPageWith2Games()
		//{
		//	var controller = CreateMockedGameController();
		//	var result = await controller.GetGamesAsync(3, 2);

		//	// Assert
		//	Mocks.GameRepository.Games.GetRange(4, 2)
		//		.Select((game, index) => new { game, index })
		//		.ToList()
		//		.ForEach(indexedGame =>
		//			Assert.AreEqual(indexedGame.game.Name, result.Value.Items(indexedGame.index].Name)
		//		);
		//}

		//[Test]
		//public async Task GetGamesFromGameControllerReturnsSecondPageWith5Games()
		//{
		//	var controller = CreateMockedGameController();
		//	var result = await controller.GetGamesAsync(2, 5);

		//	// Assert
		//	Mocks.GameRepository.Games.GetRange(5, 5)
		//		.Select((game, index) => new { game, index })
		//		.ToList()
		//		.ForEach(indexedGame =>
		//			Assert.AreEqual(indexedGame.game.Name, result.Value[indexedGame.index].Name)
		//		);
		//}

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
			var createGameModelMock = new CreateGameModel { Name = "" };

			var controller = CreateMockedGameController();
			var validationResults = ValidateModelState(createGameModelMock);

			if (validationResults.Any()) { 
				controller.ModelState.AddModelError("Required", "Invalid model state for game");
			}

			var result = await controller.CreateGameAsync(createGameModelMock);
			var actual = result.Result as BadRequestObjectResult;

			Assert.AreEqual(400, actual.StatusCode);
			Assert.AreEqual("Invalid model state for game", actual.Value);
		}

		[Test]
		public async Task CreateNewGameWithIdFromGameControllerReturnsBadRequest()
		{
			var createGameModelMock = new CreateGameModel { Name = "Game11", Description = "Description11" };

			var controller = CreateMockedGameController();

			var result = await controller.CreateGameAsync(createGameModelMock);
			var actual = result.Result as CreatedResult;
			var game = actual.Value as Game;

			Assert.AreEqual(201, actual.StatusCode);
			Assert.AreEqual(createGameModelMock.Name, game.Name);
			Assert.AreEqual(createGameModelMock.Description, game.Description);
			Assert.AreEqual(Mocks.UserManager.Account.UserName, game.Players.First().Username);
		}

		[Test]
		public async Task GetGameWithId()
		{
			var controller = CreateMockedGameController();

			var result = await controller.GetGameAsync();
			var actual = result.Result as OkObjectResult;
			var game = actual.Value as Game;

			Assert.AreEqual(Mocks.GameRepository.UserManagerGame.Token, game.Token);
			Assert.AreEqual(Mocks.GameRepository.UserManagerGame.Name, game.Name);
		}
	}
}
