using Microsoft.AspNetCore.Http;
using Othelloworld.Data.Models;
using Othelloworld.Util;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Othelloworld.Services
{
	public class AccountService : IAccountService
	{
		public JwtSecurityToken CreateJwtToken(
			string id, 
			string username,
			IEnumerable<Claim> claims,
			string signingKey,
			string issuer,
			string audience,
			int timeoutMinutes)
		{
			var tokenClaims = new List<Claim>
			{
				new Claim("id", id),
				new Claim("username", username)
			};

			tokenClaims.AddRange(claims);

			return JwtHelper.GetJwtToken(
				id,
				signingKey,
				issuer,
				audience,
				TimeSpan.FromMinutes(timeoutMinutes),
				tokenClaims);
		}

		public string GetAccountId(string authorization)
		{
			var tokenString = authorization
				.Replace("bearer ", "", true, CultureInfo.CurrentCulture);

			var token = JwtHelper.ReadJwtToken(tokenString);

			return token.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
		}
	}
}
