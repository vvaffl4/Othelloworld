using Microsoft.EntityFrameworkCore;
using Othelloworld.Data.Models;
using Othelloworld.Util;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Generic;

namespace Othelloworld.Data.Repos
{
	public class GameRepository : Repository<Game>, IGameRepository
	{
		public OthelloDbContext Context { get; set; }

		public GameRepository(OthelloDbContext context) : base(context) 
		{
			Context = context;
		}

		public void CreateGame(Game game) => 
			Create(game);
		public void UpdateGame(Game game) => 
			Update(game);
		public void DeleteGame(Game game) => 
			Delete(game);
		public Game GetGame(string token) => 
			FindByCondition(game => game.Token == token)
				.Include(game => game.Turns)
				.Include(game => game.Players)
				.ThenInclude(playerInGame => playerInGame.Player)
				.FirstOrDefault();

		public Task<PagedList<Game>> GetGames(int pageNumber, int pageSize) =>
			Task.FromResult(
				PagedList<Game>.GetPagedList(
					FindAll()
						.Where(game => game.Status == GameStatus.Staging)
						.Include(game => game.Players)
						.ThenInclude(playerInGame => playerInGame.Player)
						.OrderBy(game => game.Name), 
					pageNumber, 
					pageSize));

		public Task<IEnumerable<Game>> GetGameHistory(string username) =>
			Task.FromResult(
				FindAll()
					.Where(game => game.Players.Any(pig => pig.Player.Username == username))
						.Include(game => game.Players)
						.ThenInclude(playerInGame => playerInGame.Player)
						.OrderBy(game => game.Name)
					.AsEnumerable()
			);
	}
}
