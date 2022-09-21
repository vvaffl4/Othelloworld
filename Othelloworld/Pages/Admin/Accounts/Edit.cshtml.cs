using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Othelloworld.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Othelloworld.Pages.Admin.Accounts
{
	public class EditModel : PageModel
	{
		private readonly UserManager<Account> _userManager;

		public EditModel(UserManager<Account> userManager)
		{
			_userManager = userManager;
		}

		[BindProperty]
		public string StatusMessage { get; set; }

		[BindProperty]
		public Account Account { get; set; }
		public async Task OnGetAsync()
		{
			string username = (string)Request.RouteValues["username"];

			Account = await _userManager.FindByNameAsync(username);
		}
	}
}
