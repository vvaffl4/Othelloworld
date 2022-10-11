using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Othelloworld.Services
{
	public interface IAccountService
	{
		string GetAccountId(string authorization);

		JwtSecurityToken CreateJwtToken(
			string id, 
			string username,
			IEnumerable<Claim> claims);
	}
}
