using Duende.IdentityServer.Models;
using System.ComponentModel.DataAnnotations;

namespace Othelloworld.Controllers.Models
{
	public class LoginModel
	{
		[Required(ErrorMessage = "This field is required.")]
		//[RegularExpression("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])",
		//	ErrorMessage = "This is not a valid email")]
		[EmailAddress(ErrorMessage = "This is not a valid email.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "This field is required")]
		[MinLength(12, ErrorMessage = "This field is invalid.")]
		[MaxLength(128, ErrorMessage = "Your password can't be longer than 128 characters.")]
		public string Password { get; set; }
	}
}
