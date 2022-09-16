using Microsoft.EntityFrameworkCore;
using Othelloworld.Data.Models;
using System.Linq;

namespace Othelloworld.Data.Repos
{
	public class PlayerRepository : Repository<Player>, IPlayerRepository
	{
		public PlayerRepository(OthelloDbContext context) : base(context) { }

		public void CreatePlayer(Player player) => 
			Create(player);

		public void DeletePlayer(Player player) =>
			Delete(player);

		public Player GetPlayer(string username) =>
			FindByCondition(player => player.Username == username)
				.Include(player => player.PlayerInGame.Where(pig => pig.Game.Status == GameStatus.Playing).Take(1))
				.FirstOrDefault();

		public void UpdatePlayer(Player player)
		{
			throw new System.NotImplementedException();
		}
	}
}
