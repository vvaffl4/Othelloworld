using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Othelloworld.Data.Models
{
	//[Index(nameof(Username), IsUnique = true), Index(nameof(Email), IsUnique = true)]
	public class Account: IdentityUser
	{
		[PersonalData]
		[Column(TypeName = "varchar(64)")]
		public override string Id { get; set; }

		[ProtectedPersonalData]
		[Column(TypeName = "varchar(16)")]
		public override string UserName { get; set; }

		[Column(TypeName = "varchar(16)")]
		public override string NormalizedUserName { get; set; }

		[ProtectedPersonalData]
		[Column(TypeName = "varchar(128)")]
		public override string Email { get; set; }

		[Column(TypeName = "varchar(128)")]
		public override string NormalizedEmail { get; set; }
		public Player Player { get; set; }
	}
}
