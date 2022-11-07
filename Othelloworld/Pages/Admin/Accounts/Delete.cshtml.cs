using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using Othelloworld.Services;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Othelloworld.Pages.Admin.Accounts
{
	public class DeleteModel : PageModel
	{
		private readonly IAccountRepository _accountRepository;
		private readonly IGameRepository _gameRepository;
		private readonly IMailService _mailService;
		private readonly UserManager<Account> _userManager;

		public DeleteModel(
			IAccountRepository accountRepository,
			IGameRepository gameRepository,
			IMailService mailService,
			UserManager<Account> userManager)
		{
			_accountRepository = accountRepository;
			_gameRepository = gameRepository;
			_mailService = mailService;
			_userManager = userManager;
		}

		[BindProperty]
		public Account Account { get; set; }
		
		public async Task OnGetAsync(string username)
		{
			Account = await _userManager.FindByNameAsync(username);
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			var account = await _userManager.FindByIdAsync(Account.Id);

			var games = await _gameRepository.GetGameHistory(account.UserName);

			if (games.Any())
			{
				_gameRepository.DeleteGames(games);
			}

			var result = await _userManager.DeleteAsync(account);

			if (!result.Succeeded) return Page();

			_mailService.SendMail(
				"admin@othelloworld.hbo-ict.org",
			"Your Othelloworld Account has been Removed!",
				$"Blablabla, account, blablabla removed, blablabla, better luck next time ;P");

			return RedirectToPage("/Admin/Accounts");
		}
	}
}
