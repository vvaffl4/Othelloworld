using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;

namespace Othelloworld.Util
{
	public static class JwtHelper
	{
		public static JwtSecurityToken GetJwtToken(
				string id,
				string signingKey,
				string issuer,
				string audience,
				TimeSpan expiration,
				List<Claim> additionalClaims)
		{
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, id),
        // this guarantees the token is unique
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			if (additionalClaims is object)
			{
				var claimList = new List<Claim>(claims);
				claimList.AddRange(additionalClaims);
				claims = claimList.ToArray();
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			return new JwtSecurityToken(
					issuer: issuer,
					audience: audience,
					expires: DateTime.UtcNow.Add(expiration),
					claims: claims,
					signingCredentials: creds
			);
		}

		public static JwtSecurityToken ReadJwtToken(
				string tokenString)
		{
			var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);

			return token;
		}
	}
}
