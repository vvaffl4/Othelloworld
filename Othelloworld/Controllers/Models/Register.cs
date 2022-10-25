using Duende.IdentityServer.Models;
using System.ComponentModel.DataAnnotations;

namespace Othelloworld.Controllers.Models
{
	public class RegisterModel
	{
		[Required(ErrorMessage = "This field is required")]
		[MinLength(3, ErrorMessage = "Your username must be longer than 2 characters.")]
		[MaxLength(16, ErrorMessage = "Your username can't be longer than 16 characters.")]
		[RegularExpression("^(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$",
			ErrorMessage = "This is not a valid username.")]
		public string Username { get; set; }

		[Required(ErrorMessage = "This field is required")]
		[RegularExpression("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])",
			ErrorMessage = "This is not a valid email.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "This field is required")]
		[MinLength(12, ErrorMessage = "Your password must be longer than 11 characters.")]
		[MaxLength(128, ErrorMessage = "Your password can't be longer than 128 characters.")]
		[RegularExpression(
			"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*\\W).*$",
			ErrorMessage = "Your password should at least have one lowercase, one highercase character, one number and one symbol.")]
		public string Password { get; set; }

		[RegularExpression("^[A-Z]{2}$", ErrorMessage = "Incorrect Country Code")]
		public string Country { get; set; }
	}
}
