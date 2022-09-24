using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Othelloworld.Data.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Othelloworld.Pages.Admin
{
	public class AccountsModel : PageModel
	{
		private readonly UserManager<Account> _userManager;

		public AccountsModel(UserManager<Account> userManager)
		{
			_userManager = userManager;
		}


		[BindProperty]
		public IEnumerable<Account> Accounts { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			if (ModelState.IsValid)
			{
				if (User.IsAuthenticated() && User.IsInRole("admin"))
				{
					Accounts = await _userManager.GetUsersInRoleAsync("user");

					return Page();
				}
			}

			return Redirect("/");
		}
	}
}
