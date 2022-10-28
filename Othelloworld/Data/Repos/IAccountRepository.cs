using Othelloworld.Data.Models;
using System.Threading.Tasks;

namespace Othelloworld.Data.Repos
{
	public interface IAccountRepository
	{
		public Task<bool> ReassignRoleAsync(Account account, string currentRole, string newRole);
	}
}
