using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Othelloworld.Data.Models
{
	public enum Color: uint
	{
		none = 0,
		white = 1,
		black = 2
	}

	public enum GameResult: uint
	{
		undecided = 0,
		won = 1,
		lost = 2,
		draw = 3
	}

	public class PlayerInGame
	{
		[JsonIgnore]
		[ForeignKey("Player_Username_ID")]
		[Column(TypeName = "varchar(16)")]
		public string Username { get; set; }

		public Player Player { get; set; }

		public bool IsHost { get; set; }

		public GameResult Result { get; set; }
		public Color Color { get; set; }

		public bool ConfirmResults { get; set; }

		[JsonIgnore]
		[ForeignKey("Game_Token_ID")]
		[Column(TypeName = "varchar(64)")]
		public string GameToken { get; set; }
		public Game Game { get; set; }
	}
}
