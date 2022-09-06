using Othelloworld.Data.Models;
using Othelloworld.Util;
using System.Threading.Tasks;

namespace Othelloworld.Data.Repos
{
	public interface IPlayerRepository : IRepository<Player>
	{
		public Player GetPlayer(string username);
		public void CreatePlayer(Player player);
		public void UpdatePlayer(Player player);
		public void DeletePlayer(Player player);
	}
}
