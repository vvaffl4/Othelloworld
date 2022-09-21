using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Othelloworld.Data.Models;
using System.Collections.Generic;
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
		public async Task OnGetAsync(string id)
		{
			string username = id; //(string)Request.RouteValues["id"];

			Account = await _userManager.FindByNameAsync(username);
		}
	}
}
