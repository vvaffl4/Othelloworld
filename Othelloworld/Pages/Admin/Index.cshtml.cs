using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Othelloworld.Data.Models;
using Othelloworld.Services;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Othelloworld.Pages
{
	public class IndexModel : PageModel
	{
		private SignInManager<Account> _signInManager;

		[BindProperty]
		public string Username { get; set; }

		[BindProperty]
		public string Password { get; set; }

		public IndexModel (SignInManager<Account> signInManager)
		{
			_signInManager = signInManager;
		}

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var result = await _signInManager.PasswordSignInAsync(Username, Password, false, false);

			if (!result.Succeeded) return Page();

			Debug.WriteLine("Logged in");

			return Page();
		}
	}
}
