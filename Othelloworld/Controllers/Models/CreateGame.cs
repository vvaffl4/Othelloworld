using System.ComponentModel.DataAnnotations;

namespace Othelloworld.Controllers.Models
{
	public class CreateGameModel
	{
		[Required(ErrorMessage = "A game name is required")]
		[MinLength(3, ErrorMessage = "A game name must be longer than 2 characters.")]
		[MaxLength(64, ErrorMessage = "A game name can't be longer than 64 characters.")]
		[RegularExpression("^[A-Za-z0-9 _]*[A-Za-z0-9][A-Za-z0-9 _]*$",
			ErrorMessage = "This game name is invalid.")]
		public string Name { get; set; }

		[MaxLength(400, ErrorMessage = "A description can't be longer than 400 characters.")]
		[RegularExpression("^(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$",
			ErrorMessage = "This description is invalid.")]
		public string Description { get; set; }
	}
}
