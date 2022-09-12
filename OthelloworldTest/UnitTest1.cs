using NUnit.Framework;
using Othelloworld.Data.Models;
using Othelloworld.Services;
using System.Diagnostics;
using Assert = NUnit.Framework.Assert;

namespace OthelloworldTest
{
	[SetUpFixture]
	public class SetupTrace
	{
		[OneTimeSetUp]
		public void StartTest()
		{
			Trace.Listeners.Add(new ConsoleTraceListener());
		}

		[OneTimeTearDown]
		public void EndTest()
		{
			Trace.Flush();
		}
	}

	[TestFixture]
	public class SpelTest
	{
		readonly GameService gameService = new();
		// geen Color = 0
		// Wit = 1
		// Zwart = 2
		private Color w = Color.white;
		private Color b = Color.black;

		public Game GetNewGame(Color playerTurn)
		{
			Game game = new();
			game.Board = new IEnumerable<Color>[]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			game.PlayerTurn = playerTurn;

			return game;
		}

		public static void Print2DArray<T>(T[][] matrix)
		{
			for (int i = 0; i < matrix.Length; i++)
			{
				for (int j = 0; j < matrix.Length; j++)
				{
					Console.Write(matrix[i][j] + "\t");
				}
				Console.WriteLine();
			}
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void ZetMogelijk__BuitenBord_Exception()
		{
			Game game = GetNewGame(Color.white);
			// Arrange
			//     0 1 2 3 4 5 6 7
			//                     v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0
			//                     1 <

			// Act
			Exception ex = Assert.Throws<Exception>(delegate {
				gameService.PutStone(game, ( 8, 8 ));
			});
			Assert.That(ex.Message, Is.EqualTo("Zet (8,8) ligt buiten het bord!"));

			// Assert
		}

		[Test]
		public void ZetMogelijk_StartSituatieZet23Zwart_ReturnTrue()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			//     0 1 2 3 4 5 6 7
			//           v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 2 0 0 0 0  <
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 2, 3 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsTrue(result.valid);
		}

		[Test]
		public void zetmogelijk_startsituatiezet23wit_returnfalse()
		{
			// arrange
			Game game = GetNewGame(Color.white);
			//     0 1 2 3 4 5 6 7
			//           v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 1 0 0 0 0 <
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// act
			var result = gameService.CheckIfValid(game.Board, ( 2, 3 ), GameService.AllDirections, (int)Color.white - 1);
			// Assert
			Assert.IsFalse(result.valid);
		}


		[Test]
		public void ZetMogelijk_ZetAanDeRandBoven_ReturnTrue()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//           v
			// 0   0 0 0 2 0 0 0 0  <
			// 1   0 0 0 1 0 0 0 0
			// 2   0 0 0 1 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 3, 0 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsTrue(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandBoven_ReturnFalse()
		{
			// Arrange
			var game = GetNewGame(Color.white); 
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//           v
			// 0   0 0 0 1 0 0 0 0  <
			// 1   0 0 0 1 0 0 0 0
			// 2   0 0 0 1 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 3, 0 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsFalse(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandBovenEnTotBenedenReedsGevuld_ReturnTrue()
		{
			// Arrange
			var game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, w, w, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, b, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//           v
			// 0   0 0 0 2 0 0 0 0  <
			// 1   0 0 0 1 0 0 0 0
			// 2   0 0 0 1 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 1 1 0 0 0
			// 5   0 0 0 1 0 0 0 0
			// 6   0 0 0 1 0 0 0 0
			// 7   0 0 0 2 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 3, 0 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsTrue(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandBovenEnTotBenedenReedsGevuld_ReturnFalse()
		{
			// Arrange
			var game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, w, w, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//           v
			// 0   0 0 0 2 0 0 0 0  <
			// 1   0 0 0 1 0 0 0 0
			// 2   0 0 0 1 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 1 1 0 0 0
			// 5   0 0 0 1 0 0 0 0
			// 6   0 0 0 1 0 0 0 0
			// 7   0 0 0 1 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 3, 0 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsFalse(result.valid);
		}






		[Test]
		public void ZetMogelijk_ZetAanDeRandRechts_ReturnTrue()
		{
			// Arrange
			var game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, b, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, w, w, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 2 0 0 0 0  
			// 1   0 0 0 1 0 0 0 0
			// 2   0 0 0 1 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 1 1 2 <
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 7, 4 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsTrue(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandRechts_ReturnFalse()
		{
			// Arrange
			var game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, b, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, w, w, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 1 0 0 0 0  
			// 1   0 0 0 1 0 0 0 0
			// 2   0 0 0 1 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 1 1 1 <
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 7, 4 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsFalse(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandRechtsEnTotLinksReedsGevuld_ReturnTrue()
		{
			// Arrange
			var game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{b, w, w, w, w, w, w, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0 
			// 4   2 1 1 1 1 1 1 2 <
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 7, 4 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsTrue(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandRechtsEnTotLinksReedsGevuld_ReturnFalse()
		{
			// Arrange
			var game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{b, w, w, w, w, w, w, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0 
			// 4   2 1 1 1 1 1 1 1 <
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 7, 4 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsFalse(result.valid);
		}


		[Test]
		public void ZetMogelijk_StartSituatieZet22Wit_ReturnFalse()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			//     0 1 2 3 4 5 6 7
			//         v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 1 0 0 0 0 0 <
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 2, 2 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsFalse(result.valid);
		}

		[Test]
		public void ZetMogelijk_StartSituatieZet22Zwart_ReturnFalse()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			//     0 1 2 3 4 5 6 7
			//         v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 2 0 0 0 0 0 <
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 2, 2 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsFalse(result.valid);
		}


		[Test]
		public void ZetMogelijk_ZetAanDeRandRechtsBoven_ReturnTrue()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, b, 0},
				new Color[8]{0, 0, 0, 0, 0, b, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, w, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 1  <
			// 1   0 0 0 0 0 0 2 0
			// 2   0 0 0 0 0 2 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 1 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 7, 0 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsTrue(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandRechtsBoven_ReturnFalse()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, b, 0},
				new Color[8]{0, 0, 0, 0, 0, b, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, w, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 2  <
			// 1   0 0 0 0 0 0 2 0
			// 2   0 0 0 0 0 2 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 1 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 7, 0 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsFalse(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandRechtsOnder_ReturnTrue()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, b, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, w, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, w, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 2 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 1 0 0
			// 6   0 0 0 0 0 0 1 0
			// 7   0 0 0 0 0 0 0 2 <

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 7, 7 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsTrue(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandRechtsOnder_ReturnFalse()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, b, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, w, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, w, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 0  <
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 2 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 1 0 0
			// 6   0 0 0 0 0 0 1 0
			// 7   0 0 0 0 0 0 0 1

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 7, 7 ), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsFalse(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandLinksBoven_ReturnTrue()
		{
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, w, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, w, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, b, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//     v
			// 0   2 0 0 0 0 0 0 0  <
			// 1   0 1 0 0 0 0 0 0
			// 2   0 0 1 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 2 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0 

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 0, 0), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsTrue(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandLinksBoven_ReturnFalse()
		{
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, w, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, w, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, b, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//     v
			// 0   1 0 0 0 0 0 0 0  <
			// 1   0 1 0 0 0 0 0 0
			// 2   0 0 1 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 2 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 0, 0), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsFalse(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandLinksOnder_ReturnTrue()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, w, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, b, 0, 0, 0, 0, 0},
				new Color[8]{0, b, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//     v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 1 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 2 0 0 0 0 0
			// 6   0 2 0 0 0 0 0 0
			// 7   1 0 0 0 0 0 0 0 <

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 0, 7), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsTrue(result.valid);
		}

		[Test]
		public void ZetMogelijk_ZetAanDeRandLinksOnder_ReturnFalse()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, w, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, b, 0, 0, 0, 0, 0},
				new Color[8]{0, b, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 0  <
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 1 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 2 0 0 0 0 0
			// 6   0 2 0 0 0 0 0 0
			// 7   2 0 0 0 0 0 0 0

			// Act
			var result = gameService.CheckIfValid(game.Board, ( 0, 7), GameService.AllDirections, game.PlayerTurn);
			// Assert
			Assert.IsFalse(result.valid);
		}

		//---------------------------------------------------------------------------
		[Test]
		[ExpectedException(typeof(Exception))]
		public void DoeZet_BuitenBord_Exception()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			//     0 1 2 3 4 5 6 7
			//                     v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0
			//                     1 <

			// Act
			Exception ex = Assert.Throws<Exception>(delegate { gameService.PutStone(game, ( 8, 8)); });
			Assert.That(ex.Message, Is.EqualTo("Zet (8,8) ligt buiten het bord!"));

			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[3][3]);
			Assert.AreEqual(Color.white, game.BoardArray[4][4]);
			Assert.AreEqual(Color.black, game.BoardArray[3][4]);
			Assert.AreEqual(Color.black, game.BoardArray[4][3]);

			Assert.AreEqual(Color.white, (Color)game.PlayerTurn);
		}

		[Test]
		public void DoeZet_StartSituatieZet23Zwart_ZetCorrectUitgevoerd()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			//     0 1 2 3 4 5 6 7
			//           v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 2 0 0 0 0  <
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var resultGame = gameService.PutStone(game, ( 3, 2 ));

			// Assert
			Assert.AreEqual(Color.black, resultGame.BoardArray[2][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[3][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][3]);

			Assert.AreEqual(Color.white, resultGame.PlayerTurn);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void DoeZet_StartSituatieZet23Wit_Exception()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			//     0 1 2 3 4 5 6 7
			//           v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 1 0 0 0 0 <
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			Exception ex = Assert.Throws<Exception>(delegate { gameService.PutStone(game, ( 3, 2)); });
			Assert.That(ex.Message, Is.EqualTo("Zet (3,2) is niet mogelijk!"));

			//Exception ex = Assert.Throws<Exception>(delegate { spel.DoeZet(2, 3); });
			//Assert.That(ex.Message, Is.EqualTo("Zet (2,3) is niet mogelijk!"));

			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[3][3]);
			Assert.AreEqual(Color.white, game.BoardArray[4][4]);
			Assert.AreEqual(Color.black, game.BoardArray[3][4]);
			Assert.AreEqual(Color.black, game.BoardArray[4][3]);

			Assert.AreEqual(Color.none, game.BoardArray[2][3]);

			Assert.AreEqual(Color.white, (Color)game.PlayerTurn);
		}


		[Test]
		public void DoeZet_ZetAanDeRandBoven_ZetCorrectUitgevoerd()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//           v
			// 0   0 0 0 2 0 0 0 0  <
			// 1   0 0 0 1 0 0 0 0
			// 2   0 0 0 1 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var resultGame = gameService.PutStone(game, ( 3, 0));

			// Assert
			Assert.AreEqual(Color.black, resultGame.BoardArray[0][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[1][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[2][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[3][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][3]);

			Assert.AreEqual(Color.white, resultGame.PlayerTurn);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void DoeZet_ZetAanDeRandBoven_Exception()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//           v
			// 0   0 0 0 1 0 0 0 0  <
			// 1   0 0 0 1 0 0 0 0
			// 2   0 0 0 1 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			Exception ex = Assert.Throws<Exception>(delegate { 
				gameService.PutStone(game, ( 3, 0 )); 
			});
			Assert.That(ex.Message, Is.EqualTo("Zet (3,0) is niet mogelijk!"));

			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[3][3]);
			Assert.AreEqual(Color.white, game.BoardArray[4][4]);
			Assert.AreEqual(Color.black, game.BoardArray[3][4]);
			Assert.AreEqual(Color.black, game.BoardArray[4][3]);

			Assert.AreEqual(Color.white, game.BoardArray[1][3]);
			Assert.AreEqual(Color.white, game.BoardArray[2][3]);

			Assert.AreEqual(Color.none, game.BoardArray[0][3]);

		}

		[Test]
		public void DoeZet_ZetAanDeRandBovenEnTotBenedenReedsGevuld_ZetCorrectUitgevoerd()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, w, w, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, b, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//           v
			// 0   0 0 0 2 0 0 0 0  <
			// 1   0 0 0 1 0 0 0 0
			// 2   0 0 0 1 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 1 1 0 0 0
			// 5   0 0 0 1 0 0 0 0
			// 6   0 0 0 1 0 0 0 0
			// 7   0 0 0 2 0 0 0 0

			// Act
			var resultGame = gameService.PutStone(game, ( 3, 0 ));
			
			// Assert
			Assert.AreEqual(Color.black, resultGame.BoardArray[0][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[1][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[2][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[3][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[5][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[6][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[7][3]);
		}

			[Test]
		[ExpectedException(typeof(Exception))]
		public void DoeZet_ZetAanDeRandBovenEnTotBenedenReedsGevuld_Exception()
			{
				// Arrange
				Game game = GetNewGame(Color.black);
				game.Board = new Color[8][]
				{
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, w, b, 0, 0, 0},
					new Color[8]{0, 0, 0, w, w, 0, 0, 0},
					new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, w, 0, 0, 0, 0}
				};
			//     0 1 2 3 4 5 6 7
			//           v
			// 0   0 0 0 2 0 0 0 0  <
			// 1   0 0 0 1 0 0 0 0
			// 2   0 0 0 1 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 1 1 0 0 0
			// 5   0 0 0 1 0 0 0 0
			// 6   0 0 0 1 0 0 0 0
			// 7   0 0 0 1 0 0 0 0

			// Act
			Exception ex = Assert.Throws<Exception>(delegate {
				gameService.PutStone(game, ( 3, 0 ));
			});
			Assert.That(ex.Message, Is.EqualTo("Zet (3,0) is niet mogelijk!"));
			
			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[3][3]);
			Assert.AreEqual(Color.white, game.BoardArray[4][4]);
			Assert.AreEqual(Color.black, game.BoardArray[3][4]);
			Assert.AreEqual(Color.white, game.BoardArray[4][3]);

			Assert.AreEqual(Color.white, game.BoardArray[1][3]);
			Assert.AreEqual(Color.white, game.BoardArray[2][3]);
			Assert.AreEqual(Color.none, game.BoardArray[0][3]);
		}

		[Test]
		public void DoeZet_ZetAanDeRandRechts_ZetCorrectUitgevoerd()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, w, b, 0, 0, 0},
					new Color[8]{0, 0, 0, b, w, w, w, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 1 1 2 <
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var resultGame = gameService.PutStone(game, ( 7, 4 ));
			
			// Assert
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][4]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][5]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][6]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][7]);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void DoeZet_ZetAanDeRandRechts_Exception()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
					new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, w, b, 0, 0, 0},
					new Color[8]{0, 0, 0, b, w, w, w, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 1 0 0 0 0  
			// 1   0 0 0 1 0 0 0 0
			// 2   0 0 0 1 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 1 1 1 <
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			Exception ex = Assert.Throws<Exception>(delegate {
				gameService.PutStone(game, ( 7, 4 ));
			});
			Assert.That(ex.Message, Is.EqualTo("Zet (7,4) is niet mogelijk!"));

			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[3][3]);
			Assert.AreEqual(Color.white, game.BoardArray[4][4]);
			Assert.AreEqual(Color.black, game.BoardArray[3][4]);
			Assert.AreEqual(Color.black, game.BoardArray[4][3]);

			Assert.AreEqual(Color.white, game.BoardArray[4][5]);
			Assert.AreEqual(Color.white, game.BoardArray[4][6]);
			Assert.AreEqual(Color.none, game.BoardArray[4][7]);
		}

		[Test]
		public void DoeZet_ZetAanDeRandRechtsEnTotLinksReedsGevuld_ZetCorrectUitgevoerd()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, w, b, 0, 0, 0},
					new Color[8]{b, w, w, w, w, w, w, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0 
			// 4   2 1 1 1 1 1 1 2 <
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var resultGame = gameService.PutStone(game, ( 7, 4 ));
			
			// Assert
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][0]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][1]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][2]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][4]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][5]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][6]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][7]);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void DoeZet_ZetAanDeRandRechtsEnTotLinksReedsGevuld_Exception()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, w, b, 0, 0, 0},
					new Color[8]{b, w, w, w, w, w, w, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
					new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 0  

			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   2 1 1 1 1 1 1 1 <
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			Exception ex = Assert.Throws<Exception>(delegate {
				gameService.PutStone(game, ( 7, 4 ));
			});
			Assert.That(ex.Message, Is.EqualTo("Zet (7,4) is niet mogelijk!"));

			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[3][3]);
			Assert.AreEqual(Color.white, game.BoardArray[4][4]);
			Assert.AreEqual(Color.black, game.BoardArray[3][4]);
			Assert.AreEqual(Color.white, game.BoardArray[4][3]);

			Assert.AreEqual(Color.black, game.BoardArray[4][0]);
			Assert.AreEqual(Color.white, game.BoardArray[4][1]);
			Assert.AreEqual(Color.white, game.BoardArray[4][2]);

			Assert.AreEqual(Color.white, game.BoardArray[4][5]);
			Assert.AreEqual(Color.white, game.BoardArray[4][6]);
			Assert.AreEqual(Color.none, game.BoardArray[4][7]);
		}

		[ExpectedException(typeof(Exception))]
		public void DoeZet_StartSituatieZet22Wit_Exception()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			//     0 1 2 3 4 5 6 7
			//         v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 1 0 0 0 0 0 <
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			Exception ex = Assert.Throws<Exception>(delegate {
				gameService.PutStone(game, ( 2,2 ));
			});
			Assert.That(ex.Message, Is.EqualTo("Zet (2,2) is niet mogelijk!"));

			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[3][3]);
			Assert.AreEqual(Color.white, game.BoardArray[4][4]);
			Assert.AreEqual(Color.black, game.BoardArray[3][4]);
			Assert.AreEqual(Color.black, game.BoardArray[4][3]);

			Assert.AreEqual(Color.none, game.BoardArray[2][2]);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void DoeZet_StartSituatieZet22Zwart_Exception()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			//     0 1 2 3 4 5 6 7
			//         v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 2 0 0 0 0 0 <
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			Exception ex = Assert.Throws<Exception>(delegate {
				gameService.PutStone(game, ( 2, 2 ));
			});
			Assert.That(ex.Message, Is.EqualTo("Zet (2,2) is niet mogelijk!"));

			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[3][3]);
			Assert.AreEqual(Color.white, game.BoardArray[4][4]);
			Assert.AreEqual(Color.black, game.BoardArray[3][4]);
			Assert.AreEqual(Color.black, game.BoardArray[4][3]);

			Assert.AreEqual(Color.none, game.BoardArray[2][2]);
		}


		[Test]
		public void DoeZet_ZetAanDeRandRechtsBoven_ZetCorrectUitgevoerd()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, b, 0},
				new Color[8]{0, 0, 0, 0, 0, b, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, w, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 1  <
			// 1   0 0 0 0 0 0 2 0
			// 2   0 0 0 0 0 2 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 1 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var resultGame = gameService.PutStone(game, ( 7, 0 ));
			// Assert
			Assert.AreEqual(Color.white, resultGame.BoardArray[5][2]);
			Assert.AreEqual(Color.white, resultGame.BoardArray[4][3]);
			Assert.AreEqual(Color.white, resultGame.BoardArray[3][4]);
			Assert.AreEqual(Color.white, resultGame.BoardArray[2][5]);
			Assert.AreEqual(Color.white, resultGame.BoardArray[1][6]);
			Assert.AreEqual(Color.white, resultGame.BoardArray[0][7]);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void DoeZet_ZetAanDeRandRechtsBoven_Exception()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, b, 0},
				new Color[8]{0, 0, 0, 0, 0, b, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, w, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 2  <
			// 1   0 0 0 0 0 0 2 0
			// 2   0 0 0 0 0 2 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 1 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			Exception ex = Assert.Throws<Exception>(delegate {
				gameService.PutStone(game, ( 7, 0 ));
			});
			Assert.That(ex.Message, Is.EqualTo("Zet (7,0) is niet mogelijk!"));

			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[3][3]);
			Assert.AreEqual(Color.white, game.BoardArray[4][4]);
			Assert.AreEqual(Color.black, game.BoardArray[3][4]);
			Assert.AreEqual(Color.black, game.BoardArray[4][3]);

			Assert.AreEqual(Color.black, game.BoardArray[1][6]);
			Assert.AreEqual(Color.black, game.BoardArray[2][5]);

			Assert.AreEqual(Color.white, game.BoardArray[5][2]);

			Assert.AreEqual(Color.none, game.BoardArray[0][7]);
		}

		[Test]
		public void DoeZet_ZetAanDeRandRechtsOnder_ZetCorrectUitgevoerd()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, b, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, w, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, w, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 2 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 1 0 0
			// 6   0 0 0 0 0 0 1 0
			// 7   0 0 0 0 0 0 0 2 <

			// Act
			var resultGame = gameService.PutStone(game, ( 7, 7 ));

			// Assert
			Assert.AreEqual(Color.black, resultGame.BoardArray[2][2]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[3][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][4]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[5][5]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[6][6]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[7][7]);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void DoeZet_ZetAanDeRandRechtsOnder_Exception()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, b, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, w, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, w, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//                   v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 2 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 1 0 0
			// 6   0 0 0 0 0 0 1 0
			// 7   0 0 0 0 0 0 0 1 <

			// Act
			Exception ex = Assert.Throws<Exception>(delegate {
				gameService.PutStone(game, ( 7, 7 ));
			});
			Assert.That(ex.Message, Is.EqualTo("Zet (7,7) is niet mogelijk!"));

			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[3][3]);
			Assert.AreEqual(Color.white, game.BoardArray[4][4]);
			Assert.AreEqual(Color.black, game.BoardArray[3][4]);
			Assert.AreEqual(Color.black, game.BoardArray[4][3]);

			Assert.AreEqual(Color.black, game.BoardArray[2][2]);
			Assert.AreEqual(Color.white, game.BoardArray[5][5]);
			Assert.AreEqual(Color.white, game.BoardArray[6][6]);

			Assert.AreEqual(Color.none, game.BoardArray[7][7]);
		}

		[Test]
		public void DoeZet_ZetAanDeRandLinksBoven_ZetCorrectUitgevoerd()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, w, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, w, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, b, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//     v
			// 0   2 0 0 0 0 0 0 0  <
			// 1   0 1 0 0 0 0 0 0
			// 2   0 0 1 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 2 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0 

			// Act
			var resultGame = gameService.PutStone(game, ( 0, 0 ));

			// Assert
			Assert.AreEqual(Color.black, resultGame.BoardArray[0][0]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[1][1]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[2][2]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[3][3]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[4][4]);
			Assert.AreEqual(Color.black, resultGame.BoardArray[5][5]);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void DoeZet_ZetAanDeRandLinksBoven_Exception()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, w, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, w, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, b, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//     v
			// 0   1 0 0 0 0 0 0 0  <
			// 1   0 1 0 0 0 0 0 0
			// 2   0 0 1 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 2 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0          

			// Act
			Exception ex = Assert.Throws<Exception>(delegate {
				gameService.PutStone(game, ( 0, 0 ));
			});
			Assert.That(ex.Message, Is.EqualTo("Zet (0,0) is niet mogelijk!"));

			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[3][3]);
			Assert.AreEqual(Color.white, game.BoardArray[4][4]);
			Assert.AreEqual(Color.black, game.BoardArray[3][4]);
			Assert.AreEqual(Color.black, game.BoardArray[4][3]);

			Assert.AreEqual(Color.white, game.BoardArray[1][1]);
			Assert.AreEqual(Color.white, game.BoardArray[2][2]);

			Assert.AreEqual(Color.black, game.BoardArray[5][5]);

			Assert.AreEqual(Color.none, game.BoardArray[0][0]);
		}

		[Test]
		public void DoeZet_ZetAanDeRandLinksOnder_ZetCorrectUitgevoerd()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, w, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, b, 0, 0, 0, 0, 0},
				new Color[8]{0, b, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//     v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 1 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 2 0 0 0 0 0
			// 6   0 2 0 0 0 0 0 0
			// 7   1 0 0 0 0 0 0 0 <

			// Act
			var resultGame = gameService.PutStone(game, ( 0, 7 ));

			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[7][0]);
			Assert.AreEqual(Color.white, game.BoardArray[6][1]);
			Assert.AreEqual(Color.white, game.BoardArray[5][2]);
			Assert.AreEqual(Color.white, game.BoardArray[4][3]);
			Assert.AreEqual(Color.white, game.BoardArray[3][4]);
			Assert.AreEqual(Color.white, game.BoardArray[2][5]);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void DoeZet_ZetAanDeRandLinksOnder_Exception()
		{
			// Arrange
			Game game = GetNewGame(Color.black);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, w, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, b, 0, 0, 0, 0, 0},
				new Color[8]{0, b, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};
			//     0 1 2 3 4 5 6 7
			//     v
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 1 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 2 0 0 0 0 0
			// 6   0 2 0 0 0 0 0 0
			// 7   2 0 0 0 0 0 0 0 <

			// Act
			Exception ex = Assert.Throws<Exception>(delegate {
				gameService.PutStone(game, ( 0, 7 ));
			});
			Assert.That(ex.Message, Is.EqualTo("Zet (0,7) is niet mogelijk!"));

			// Assert
			Assert.AreEqual(Color.white, game.BoardArray[3][3]);
			Assert.AreEqual(Color.white, game.BoardArray[4][4]);
			Assert.AreEqual(Color.black, game.BoardArray[3][4]);
			Assert.AreEqual(Color.black, game.BoardArray[4][3]);

			Assert.AreEqual(Color.white, game.BoardArray[2][5]);
			Assert.AreEqual(Color.black, game.BoardArray[5][2]);
			Assert.AreEqual(Color.black, game.BoardArray[6][1]);

			Assert.AreEqual(Color.none, game.BoardArray[7][7]);

			Assert.AreEqual(Color.none, game.BoardArray[7][0]);
		}

		[Test]
		public void Pas_ZwartAanZetGeenZetMogelijk_ReturnTrueEnWisselBeurt()
		{
			// Arrange  (zowel wit als zwart kunnen niet meer)
			Game game = GetNewGame(Color.black); 
			game.Board = new Color[8][]
			{
				new Color[8]{w, w, w, w, w, w, w, w},
				new Color[8]{w, w, w, w, w, w, w, w},
				new Color[8]{w, w, w, w, w, w, w, w},
				new Color[8]{w, w, w, w, w, w, w, 0},
				new Color[8]{w, w, w, w, w, w, 0, 0},
				new Color[8]{w, w, w, w, w, w, 0, b},
				new Color[8]{w, w, w, w, w, w, w, 0},
				new Color[8]{w, w, w, w, w, w, w, w}
			};

			//     0 1 2 3 4 5 6 7
			//     v
			// 0   1 1 1 1 1 1 1 1  
			// 1   1 1 1 1 1 1 1 1
			// 2   1 1 1 1 1 1 1 1
			// 3   1 1 1 1 1 1 1 0
			// 4   1 1 1 1 1 1 0 0
			// 5   1 1 1 1 1 1 0 2
			// 6   1 1 1 1 1 1 1 0
			// 7   1 1 1 1 1 1 1 1

			// Act
			var resultGame = gameService.Pass(game);

			// Assert
			Assert.AreEqual(Color.white, resultGame.PlayerTurn);
		}

		[Test]
		public void Pas_WitAanZetGeenZetMogelijk_ReturnTrueEnWisselBeurt()
		{
			// Arrange  (zowel wit als zwart kunnen niet meer)
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{w, w, w, w, w, w, w, w},
				new Color[8]{w, w, w, w, w, w, w, w},
				new Color[8]{w, w, w, w, w, w, w, w},
				new Color[8]{w, w, w, w, w, w, w, 0},
				new Color[8]{w, w, w, w, w, w, 0, 0},
				new Color[8]{w, w, w, w, w, w, 0, b},
				new Color[8]{w, w, w, w, w, w, w, 0},
				new Color[8]{w, w, w, w, w, w, w, w}
			};

			//     0 1 2 3 4 5 6 7
			//     v
			// 0   1 1 1 1 1 1 1 1  
			// 1   1 1 1 1 1 1 1 1
			// 2   1 1 1 1 1 1 1 1
			// 3   1 1 1 1 1 1 1 0
			// 4   1 1 1 1 1 1 0 0
			// 5   1 1 1 1 1 1 0 2
			// 6   1 1 1 1 1 1 1 0
			// 7   1 1 1 1 1 1 1 1

			// Act
			var resultGame = gameService.Pass(game);
			
			// Assert
			Assert.AreEqual(Color.black, resultGame.PlayerTurn);
		}

		[Test]
		public void Afgelopen_GeenZetMogelijk_ReturnTrue()
		{
			// Arrange  (zowel wit als zwart kunnen niet meer)
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{w, w, w, w, w, w, w, w},
				new Color[8]{w, w, w, w, w, w, w, w},
				new Color[8]{w, w, w, w, w, w, w, w},
				new Color[8]{w, w, w, w, w, w, w, 0},
				new Color[8]{w, w, w, w, w, w, 0, 0},
				new Color[8]{w, w, w, w, w, w, 0, b},
				new Color[8]{w, w, w, w, w, w, w, 0},
				new Color[8]{w, w, w, w, w, w, w, w}
			};

			//     0 1 2 3 4 5 6 7
			//     v
			// 0   1 1 1 1 1 1 1 1  
			// 1   1 1 1 1 1 1 1 1
			// 2   1 1 1 1 1 1 1 1
			// 3   1 1 1 1 1 1 1 0
			// 4   1 1 1 1 1 1 0 0
			// 5   1 1 1 1 1 1 0 2
			// 6   1 1 1 1 1 1 1 0
			// 7   1 1 1 1 1 1 1 1

			// Act
			var result = gameService.Finished(game);
			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void Afgelopen_GeenZetMogelijkAllesBezet_ReturnTrue()
		{
			// Arrange  (zowel wit als zwart kunnen niet meer)
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{w, w, w, w, w, w, w, w},
				new Color[8]{w, w, w, w, w, w, w, w},
				new Color[8]{w, w, w, w, w, w, w, w},
				new Color[8]{w, w, w, w, w, w, w, b},
				new Color[8]{w, w, w, w, w, w, b, b},
				new Color[8]{w, w, w, w, w, w, b, b},
				new Color[8]{w, w, w, w, w, w, w, b},
				new Color[8]{w, w, w, w, w, w, w, w}
			};

			//     0 1 2 3 4 5 6 7
			//     v
			// 0   1 1 1 1 1 1 1 1  
			// 1   1 1 1 1 1 1 1 1
			// 2   1 1 1 1 1 1 1 1
			// 3   1 1 1 1 1 1 1 2
			// 4   1 1 1 1 1 1 2 2
			// 5   1 1 1 1 1 1 2 2
			// 6   1 1 1 1 1 1 1 2
			// 7   1 1 1 1 1 1 1 1
			
			// Act
			var result = gameService.Finished(game);
			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void Afgelopen_WelZetMogelijk_ReturnFalse()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			//     0 1 2 3 4 5 6 7
			//                     
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0
			
			// Act
			var result = gameService.Finished(game);
			// Assert
			Assert.IsFalse(result);
		}



		[Test]
		public void OverwegendeColor_Gelijk_ReturnColorGeen()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			//     0 1 2 3 4 5 6 7
			//                     
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 0 0 0 0 0
			// 3   0 0 0 1 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0
			
			// Act
			var colors = gameService.CountColors(game);

			// Assert
			Assert.IsTrue(colors.white == colors.black);
			Assert.AreEqual(64, colors.none + colors.white + colors.black);
		}

		[Test]
		public void OverwegendeColor_Zwart_ReturnColorZwart()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, b, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};

			//     0 1 2 3 4 5 6 7
			//                     
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 2 0 0 0 0
			// 3   0 0 0 2 2 0 0 0
			// 4   0 0 0 2 1 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0
			

			// Act
			var colors = gameService.CountColors(game);

			// Assert
			Assert.IsTrue(colors.black > colors.white);
			Assert.AreEqual(64, colors.none + colors.white + colors.black);
		}

		[Test]
		public void OverwegendeColor_Wit_ReturnColorWit()
		{
			// Arrange
			Game game = GetNewGame(Color.white);
			game.Board = new Color[8][]
			{
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, w, b, 0, 0, 0},
				new Color[8]{0, 0, 0, b, w, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0},
				new Color[8]{0, 0, 0, 0, 0, 0, 0, 0}
			};


			//     0 1 2 3 4 5 6 7
			//                     
			// 0   0 0 0 0 0 0 0 0  
			// 1   0 0 0 0 0 0 0 0
			// 2   0 0 0 1 0 0 0 0
			// 3   0 0 0 1 1 0 0 0
			// 4   0 0 0 1 2 0 0 0
			// 5   0 0 0 0 0 0 0 0
			// 6   0 0 0 0 0 0 0 0
			// 7   0 0 0 0 0 0 0 0

			// Act
			var colors = gameService.CountColors(game);

			// Assert
			Assert.IsTrue(colors.black < colors.white);
			Assert.AreEqual(64, colors.none + colors.white + colors.black);
		}
	}
}