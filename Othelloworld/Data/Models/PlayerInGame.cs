using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Othelloworld.Data.Models
{
	public enum Color: uint
	{
		none = 0,
		white = 1,
		black = 2
	}

	public class PlayerInGame
	{
		[ForeignKey("Player_Username_ID")]
		public string Username { get; set; }

		public Player Player { get; set; }
		
		public bool IsHost { get; set; }
		public Color Color { get; set; }

		[ForeignKey("Game_Token_ID")]
		public string GameToken { get; set; }
		public Game Game { get; set; }
	}
}
