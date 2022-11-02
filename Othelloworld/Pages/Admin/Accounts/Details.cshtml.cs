using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Othelloworld.Pages.Admin.Accounts
{
	public class IndexModel : PageModel
	{
		private readonly IAccountRepository _accountRepository;
		private readonly UserManager<Account> _userManager;

		public IndexModel(IAccountRepository accountRepository, UserManager<Account> userManager)
		{
			_accountRepository = accountRepository;
			_userManager = userManager;
		}

		[BindProperty]
		public string StatusMessage { get; set; }

		[BindProperty]
		public Account Account { get; set; }

		[BindProperty]
		public string Role { get; set; }

		[BindProperty]
		public string NewRole { get; set; }
		
		public async Task OnGetAsync(string username)
		{
			Account = await _userManager.FindByNameAsync(username);
			Role = (await _userManager.GetRolesAsync(Account)).First();
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			var account = await _userManager.FindByIdAsync(Account.Id);
			var role = (await _userManager.GetRolesAsync(account)).First();

			if (account.UserName != Account.UserName
			 || account.Email != Account.Email)
			{ 
				account.UserName = Account.UserName;
				account.Email = Account.Email;

				var result = await _userManager.UpdateAsync(Account);

				if (result.Succeeded)
				{
					StatusMessage = "User info has successfully been altered";
				}
			}

			if (role != NewRole)
			{ 
				try
				{
					var result = await _accountRepository.ReassignRoleAsync(account, role, NewRole);
					if (result)
					{
						StatusMessage = "User role has successfully been altered";
					}
				}
				catch
				{

				}
			}
			//if(result.Succeeded )
			//{
			//	return Page();
			//} else
			//{
			//	Debug.WriteLine(result.Errors.ToString());
			//}
			return Page();
		}
	}
}
