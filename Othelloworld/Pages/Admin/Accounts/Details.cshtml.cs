using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Othelloworld.Data.Models;
using System.Collections.Generic;
using System.Diagnostics;
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
		public async Task OnGetAsync(string username)
		{
			Account = await _userManager.FindByNameAsync(username);
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var result = await _userManager.UpdateAsync(Account);

			if(result.Succeeded)
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
