using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Othelloworld.Data.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Duende.IdentityServer.Extensions;

namespace Othelloworld.Pages
{
	public class IndexModel : PageModel
	{
		private readonly SignInManager<Account> _signInManager;
		private readonly UserManager<Account> _userManager;
		private readonly ILogger<IndexModel> _logger;

		public IndexModel(
			SignInManager<Account> signInManager,
			UserManager<Account> userManager,
			ILogger<IndexModel> logger)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
		}

		public class InputModel
		{
			[Display(Name = "Email")]
			[Required]
			[EmailAddress]
			public string Email { get; set; }

			[Display(Name = "Password")]
			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; }

			[Display(Name = "Remember me?")]
			public bool RememberMe { get; set; }
		}

		[BindProperty]
		public InputModel Input { get; set; }
		public string ReturnUrl { get; set; }

		[TempData]
		public string ErrorMessage { get; set; }

		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		public async Task<IActionResult> OnGetAsync(string returnUrl = null)
		{
			if (!string.IsNullOrEmpty(ErrorMessage))
			{
				ModelState.AddModelError(string.Empty, ErrorMessage);
			}

			if (User.IsAuthenticated() && User.IsInRole("admin"))
			{
				return RedirectToPage("./Admin/Dashboard");
			}

			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			return Page();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{
				if (User.IsAuthenticated() 
				&& (User.IsInRole("admin") || User.IsInRole("mod")))
				{
					return RedirectToPage("./Admin/Dashboard");
				}

				var account = await _userManager.FindByEmailAsync(Input.Email);
				var result = await _signInManager.PasswordSignInAsync(account.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

				if (result.Succeeded)
				{
					_logger.LogInformation("User logged in.");
					return RedirectToPage("./Admin/Dashboard");
				}
				if (result.RequiresTwoFactor)
				{
					return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
				}
				if (result.IsLockedOut)
				{
					_logger.LogWarning("User account locked out.");
					return RedirectToPage("./Lockout");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return Page();
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}


		public async Task<IActionResult> OnGetLogoutAsync()
		{
			if (ModelState.IsValid)
			{
				await _signInManager.SignOutAsync();
			}

			return RedirectToPage();
		}
	}
}
