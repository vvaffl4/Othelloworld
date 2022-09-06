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
		public Player Player { get; set; }
	}
}
