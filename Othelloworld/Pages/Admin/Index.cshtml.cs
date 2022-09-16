using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Othelloworld.Data.Models;
using Othelloworld.Services;
using System.Threading.Tasks;
using System.Diagnostics;
using static Duende.IdentityServer.Models.IdentityResources;
using System.Xml.Linq;

namespace Othelloworld.Pages
{
	public class Movie
	{
		public string Title { get; set; }
		public string ReleaseDate { get; set; }
		public string Genre { get; set; }
		public string Price { get; set; }

	}

	public class IndexModel : PageModel
	{

		public IActionResult OnGet()
		{
			return Page();
		}

		[BindProperty]
		public Movie Movie { get; set; } = default!;


		// To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid || Movie == null)
			{
				return Page();
			}
			
			return RedirectToPage("./Admin");
		}
	}
}
