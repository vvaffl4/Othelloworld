using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Othelloworld.Data.Models
{
	public class Turn
	{
		public int Number { get; set; }
		public int X { get; set; }
		public float Y { get; set; }
		public Color Color { get; set; }

		[JsonIgnore]
		[ForeignKey("Game_Token_ID")]
		public string GameToken { get; set; }
		public Game Game { get; set; }
	}
}
