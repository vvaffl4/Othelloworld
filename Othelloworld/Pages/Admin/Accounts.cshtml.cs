using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Othelloworld.Data.Models;
using Othelloworld.Pages.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		public IEnumerable<ListAccount> Accounts { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			if (ModelState.IsValid)
			{
				if (User.IsAuthenticated())
				{
					if (User.IsInRole("admin"))
					{
						var admins = await _userManager.GetUsersInRoleAsync("admin");
						var mods = await _userManager.GetUsersInRoleAsync("mod");
						var users = await _userManager.GetUsersInRoleAsync("user");

						Accounts = admins.Select(admin => new ListAccount { Id = admin.Id, UserName = admin.UserName, Email = admin.Email, Role = "Administrator" })
							.Concat(mods.Select(mod => new ListAccount { Id = mod.Id, UserName = mod.UserName, Email = mod.Email, Role = "Moderator" }))
							.Concat(users.Select(user => new ListAccount { Id = user.Id, UserName = user.UserName, Email = user.Email, Role = "User" }));

						return Page();
					}

					if (User.IsInRole("mod"))
					{
						var users = await _userManager.GetUsersInRoleAsync("user");

						Accounts = users.Select(user => new ListAccount { Id = user.Id, UserName = user.UserName, Email = user.Email, Role = "User" });

						return Page();
					}
				}
			}

			return Redirect("/");
		}
	}
}
