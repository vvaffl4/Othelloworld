using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
//using Microsoft.IdentityModel.JsonWebTokens;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;

namespace Othelloworld.Util
{
	public class JwtHelper
	{
		public JwtSecurityToken GetJwtToken(
				string id,
				SigningCredentials signingCredentials,
				EncryptingCredentials publicEncryptionCredentials,
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

			//var jwtTokenHandler = new JsonWebTokenHandler();

			var jwtTokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = issuer,
				Audience = audience,
				IssuedAt = DateTime.UtcNow,
				Expires = DateTime.UtcNow.Add(expiration),
				Claims = claims.ToDictionary(claim => claim.Type, claim => (object)claim.Value),
				SigningCredentials = signingCredentials,
				EncryptingCredentials = publicEncryptionCredentials
			};

			var tokenHandler = new JwtSecurityTokenHandler();

			return tokenHandler.CreateJwtSecurityToken(jwtTokenDescriptor);

			//return jwtTokenHandler.CreateToken(jwtTokenDescriptor);

			//return new JwtSecurityToken(
			//		issuer: issuer,
			//		audience: audience,
			//		expires: DateTime.UtcNow.Add(expiration),
			//		claims: claims,
			//		signingCredentials: creds
			//);
		}

		public ClaimsPrincipal ValidateToken(
			string token,
			SymmetricSecurityKey signingKey,
			SecurityKey privateEncryptionKey,
			string issuer,
			string audience)
		{
			//var jwtTokenHandler = new JsonWebTokenHandler();
			var tokenHandler = new JwtSecurityTokenHandler();

			SecurityToken resultToken;
			var claims = tokenHandler.ValidateToken(
				token,
				new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					RequireExpirationTime = true,
					ValidIssuer = issuer,
					ValidAudience = audience,
					IssuerSigningKey = signingKey,
					TokenDecryptionKey = privateEncryptionKey
				},
				out resultToken);

			return claims;
			//return jwtTokenHandler.ValidateToken(
			//		token,
			//		new TokenValidationParameters
			//		{
			//			ValidateIssuer = true,
			//			ValidateAudience = true,
			//			RequireExpirationTime = true,
			//			ValidIssuer = issuer,
			//			ValidAudience = audience,
			//			IssuerSigningKey = signingKey,
			//			TokenDecryptionKey = privateEncryptionKey
			//		});
			//var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
		}
	}
}
