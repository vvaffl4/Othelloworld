using Microsoft.EntityFrameworkCore;
using Othelloworld.Data.Models;
using System.Collections.Generic;
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
				.Include(player => player.PlayerInGame.Where(pig => 
					pig.Game.Status == GameStatus.Playing
					|| pig.Game.Status == GameStatus.Staging
					|| !pig.ConfirmResults
				).Take(1))
				.AsNoTracking()
				.FirstOrDefault();

		public void UpdatePlayer(Player player) =>
			Update(player);

		public IEnumerable<Country> GetCountries() =>
			_context.Countries;
	}
}
