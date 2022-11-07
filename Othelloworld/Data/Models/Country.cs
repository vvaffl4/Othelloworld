using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Othelloworld.Data.Models
{
	public class Country
	{
		[Key]
		[Column(TypeName = "varchar(2)")]
		public string Code { get; set; }
		[Column(TypeName = "varchar(2)")]
		public string ContinentCode { get; set; }
		[Column(TypeName = "varchar(64)")]
		public string Name { get; set; }
		[Column(TypeName = "varchar(3)")]
		public string Iso3 { get; set; }
		[Column(TypeName = "varchar(3)")]
		public string Number { get; set; }
		[Column(TypeName = "varchar(128)")]
		public string FullName { get; set; }
	}
}
