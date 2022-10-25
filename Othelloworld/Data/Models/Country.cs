using System.ComponentModel.DataAnnotations;

namespace Othelloworld.Data.Models
{
	public class Country
	{
		[Key]
		public string Code { get; set; }
		public string ContinentCode { get; set; }
		public string Name { get; set; }
		public string Iso3 { get; set; }
		public string Number { get; set; }
		public string FullName { get; set; }
	}
}
