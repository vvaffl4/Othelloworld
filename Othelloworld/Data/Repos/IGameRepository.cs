using Othelloworld.Data.Models;
using Othelloworld.Util;
using System.Threading.Tasks;

namespace Othelloworld.Data.Repos
{
	public interface IGameRepository : IRepository<Game>
	{
		public Game GetGame(string token);
		public void CreateGame(Game game);
		public void UpdateGame(Game game);
		public void DeleteGame(Game game);

		public Task<PagedList<Game>> GetGames(int pageNumber, int pageSize);
	}
}
