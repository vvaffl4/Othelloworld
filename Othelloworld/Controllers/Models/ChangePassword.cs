using Duende.IdentityServer.Models;
using System.ComponentModel.DataAnnotations;

namespace Othelloworld.Controllers.Models
{
	public class ChangePasswordModel
	{
		[Required(ErrorMessage = "This field is required")]
		[MinLength(12, ErrorMessage = "Your password must be longer than 11 characters.")]
		[MaxLength(128, ErrorMessage = "Your password can't be longer than 128 characters.")]
		[RegularExpression(
			"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*\\W).*$",
			ErrorMessage = "Your password should at least have one lowercase, one highercase character, one number and one symbol.")]
		public string CurrentPassword { get; set; }

		[Required(ErrorMessage = "This field is required")]
		[MinLength(12, ErrorMessage = "Your password must be longer than 11 characters.")]
		[MaxLength(128, ErrorMessage = "Your password can't be longer than 128 characters.")]
		[RegularExpression(
			"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*\\W).*$",
			ErrorMessage = "Your password should at least have one lowercase, one highercase character, one number and one symbol.")]
		public string NewPassword { get; set; }

	}
}
