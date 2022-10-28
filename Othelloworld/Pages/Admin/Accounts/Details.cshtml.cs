using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Othelloworld.Data.Models;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Othelloworld.Pages.Admin.Accounts
{
	public class IndexModel : PageModel
	{
		private readonly UserManager<Account> _userManager;

		public IndexModel(UserManager<Account> userManager)
		{
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
			var OldRole = (await _userManager.GetRolesAsync(Account)).First();

			var result = await _userManager.UpdateAsync(Account);
			var roleRemoveResult = await _userManager.RemoveFromRoleAsync(Account, OldRole);
			var roleAddResult = await _userManager.AddToRoleAsync(Account, NewRole);

			if(result.Succeeded )
			{
				return Page();
			} else
			{
				Debug.WriteLine(result.Errors.ToString());
			}
			return Page();
		}
	}
}
