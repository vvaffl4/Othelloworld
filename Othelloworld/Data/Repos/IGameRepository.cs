using Othelloworld.Data.Models;
using Othelloworld.Util;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Othelloworld.Data.Repos
{
	public interface IGameRepository : IRepository<Game>
	{
		public OthelloDbContext Context { get; }
		public Game GetGame(string token);
		public void CreateGame(Game game);
		public void UpdateGame(Game game);
		public void DeleteGame(Game game);
		public void DeleteGames(IEnumerable<Game> games);

		public Task<PagedList<Game>> SearchGames(string value, int pageNumber, int pageSize);

		public Task<PagedList<Game>> GetGames(int pageNumber, int pageSize);

		public Task<IEnumerable<Game>> GetGameHistory(string username);
	}
}
