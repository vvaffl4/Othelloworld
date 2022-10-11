using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Linq;

namespace Othelloworld.Util
{
	public class JwtHelper
	{
		public string GetJwtToken(
				string id,
				string signingKey,
				string encryptionKey,
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

			var symmetricEncryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionKey));
			var encryptionCreds = new EncryptingCredentials(key, SecurityAlgorithms.HmacSha256);

			var jwtTokenHandler = new JsonWebTokenHandler();

			return jwtTokenHandler.CreateToken(new SecurityTokenDescriptor
			{
				Issuer = issuer,
				Audience = audience,
				Expires = DateTime.UtcNow.Add(expiration),
				Claims = claims.ToDictionary(claim => claim.Type, claim => (object)claim.Value),
				SigningCredentials = creds,
				EncryptingCredentials = encryptionCreds
			});

			//return new JwtSecurityToken(
			//		issuer: issuer,
			//		audience: audience,
			//		expires: DateTime.UtcNow.Add(expiration),
			//		claims: claims,
			//		signingCredentials: creds
			//);
		}

		public TokenValidationResult ValidateToken(
			string token,
			string signingKey,
			string encryptionKey,
			string issuer,
			string audience)
		{
			var jwtTokenHandler = new JsonWebTokenHandler();

			var symmetricSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
			var symmetricEncryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionKey));

			return jwtTokenHandler.ValidateToken(
					token,
					new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						RequireExpirationTime = true,
						ValidIssuer = issuer,
						ValidAudience = audience,
						// public key for signing
						IssuerSigningKey = symmetricSigningKey,

						// private key for encryption
						TokenDecryptionKey = symmetricEncryptionKey
					});
			//var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
		}
	}
}
