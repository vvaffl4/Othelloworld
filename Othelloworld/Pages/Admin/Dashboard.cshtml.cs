using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Othelloworld.Data.Models;

namespace Othelloworld.Pages.Admin
{
	public class DashboardModel : PageModel
	{
		private UserManager<Account> _userManager;

		public DashboardModel(UserManager<Account> userManager)
		{
			_userManager = userManager;
		}

		public IActionResult OnGet()
		{
			if (ModelState.IsValid)
			{
				if (User.IsAuthenticated() && User.IsInRole("admin"))
				{
					return Page();
				}
			}

			return Redirect("/");
		}
	}
}
