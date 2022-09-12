using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Othelloworld.Data.Models
{
	public class Player
	{
		[Key]
		[ForeignKey("Account_ID")]
		public string Username { get; set; }
		public Account Account { get; set; }
		public string country { get; set; }
		public int amountWon { get; set; }
		public int amountLost { get; set; }
		public int amountDraw { get; set; }

		public PlayerInGame PlayerInGame { get; set; }
	}
}
