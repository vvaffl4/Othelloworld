using Othelloworld.Data.Models;
using System.Collections.Generic;

namespace Othelloworld.Services
{
	public class Result
	{
		public bool valid;
		public (int x, int y)[][] paths;
	}

	public class ColorCount
	{
		public int black;
		public int white;
		public int none;
	}

	public interface IGameService
	{
		Game CreateNewGame(string userName, string gameName, string gameDescription);
		(int none, int white, int black) CountColors(Game game);
		Result CheckIfValid(IEnumerable<IEnumerable<Color>> board, (int x, int y) position, (int x, int y)[] directions, Color turn);
		Game PutStone(Game game, (int x, int y) position);
		Game Pass(Game game);

	}
}
