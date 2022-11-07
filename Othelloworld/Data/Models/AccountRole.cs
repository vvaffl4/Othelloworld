using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Othelloworld.Data.Models
{
	public class AccountRole: IdentityRole<string>
	{
		[Column(TypeName = "varchar(64)")]
		public override string Id { get; set; }

		[Column(TypeName = "varchar(16)")]
		public virtual string Name { get; set; }

		[Column(TypeName = "varchar(16)")]
		public virtual string NormalizedName { get; set; }
	}
}
