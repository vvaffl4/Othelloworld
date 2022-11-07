using Microsoft.AspNetCore.Identity;
using Othelloworld.Data.Models;
using System;
using System.Threading.Tasks;

namespace Othelloworld.Data.Repos
{
	public class AccountRepository : IAccountRepository
	{
		private OthelloDbContext _context;
		private UserManager<Account> _userManager;

		public AccountRepository(
			OthelloDbContext context,
			UserManager<Account> userManager) 
		{ 
			_context = context;
			_userManager = userManager;
		}

		public async Task<bool> ReassignRoleAsync(Account account, string currentRole, string newRole)
		{
			using (var identitydbContextTransaction = _context.Database.BeginTransaction())
			{
				try
				{
					var removeResult = await _userManager.RemoveFromRoleAsync(account, currentRole);
					if (removeResult.Succeeded)
					{
						var addResult = await _userManager.AddToRoleAsync(account, newRole);

						if (addResult.Succeeded)
						{
							identitydbContextTransaction.Commit();
							return true;
						}
					}
				}
				catch (Exception)
				{
					identitydbContextTransaction.Rollback();
					throw;
				}
			}

			return false;
		}
	}
}
