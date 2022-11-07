using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Othelloworld.Data.Models
{
	public enum GameStatus
	{
		Staging = 0,
		Playing = 1,
		Finished = 2
	}

	public class Game
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column(TypeName = "varchar(64)")]
		public string Token { get; set; }

		[Required]
		[Column(TypeName = "varchar(64)")]
		public string Name { get; set; }

		public GameStatus Status { get; set; }
		public virtual ICollection<PlayerInGame> Players { get; set; }

		public ICollection<Turn> Turns { get; set; }

		public Color PlayerTurn { get; set; }

		[Column(TypeName = "varchar(400)")]
		public string Description { get; set; }

		[NotMapped]
		public IEnumerable<IEnumerable<Color>> Board
		{
			get
			{
				return InternalBoard
					.Split(';')
					.Select(row => Array.ConvertAll(row.Split(','), int.Parse)
						.Cast<Color>());
			}
			set
			{
				IEnumerable<IEnumerable<Color>> _board = value;
				InternalBoard = String.Join(';', _board.Select(item => String.Join(',', item.Select(item => (int)item))));
			}
		}

		[JsonIgnore]
		[NotMapped]
		public Color[][] BoardArray
		{
			get
			{
				return InternalBoard
					.Split(';')
					.Select(row => Array.ConvertAll(row.Split(','), int.Parse)
						.Cast<Color>()
						.ToArray())
					.ToArray();

			}
			set
			{
				Color[][] _board = value;
				InternalBoard = String.Join(';', _board.Select(item => String.Join(',', item)));
			}
		}

		[JsonIgnore]
		[Required]
		[Column(TypeName = "varchar(400)")]
		public string InternalBoard { get; set; }
	}
}
