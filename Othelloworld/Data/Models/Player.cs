using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Othelloworld.Data.Models
{
	public class Player
	{
		[Key]
		[ForeignKey("Account_ID")]
		public string Username { get; set; }
		[JsonIgnore]
		public Account Account { get; set; }
		public string Country { get; set; }
		public int AmountWon { get; set; }
		public int AmountLost { get; set; }
		public int AmountDraw { get; set; }

		public IEnumerable<PlayerInGame> PlayerInGame { get; set; }
	}
}
