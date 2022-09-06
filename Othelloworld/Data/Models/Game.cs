using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Othelloworld.Data.Models
{
	public enum Status
	{
		Staging = 0,
		Playing = 1
	}

	public class Game
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public string Token { get; set; }

		[Required]
		public string Name { get; set; }

		public virtual ICollection<PlayerInGame> Players { get; set; }
		public Color PlayerTurn { get; set; }
		public string Description { get; set; }

		[NotMapped]
		public int[][] Board
		{
			get
			{
				return InternalBoard
					.Split(';')
					.Select(row => Array.ConvertAll(row.Split(','), int.Parse))
					.ToArray();
			}
			set
			{
				int[][] _board = value;
				InternalBoard = String.Join(';', _board.Select(item => String.Join(',', item)));
			}
		}

		[JsonIgnore]
		[Required]
		public string InternalBoard { get; set; }
	}
}
