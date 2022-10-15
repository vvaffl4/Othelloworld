using Microsoft.AspNetCore.Http;
using Othelloworld.Data.Models;
using Othelloworld.Util;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Net;
using Microsoft.IdentityModel.Tokens;

namespace Othelloworld.Services
{
	public class AccountService : IAccountService
	{
		private Credentials _credentials;
		private JwtHelper _jwtHelper;
		public AccountService(
			Credentials credentials, 
			JwtHelper jwtHelper)
		{
			_credentials = credentials;
			_jwtHelper = jwtHelper;
		}

		public JwtSecurityToken CreateJwtToken(
			string id, 
			string username,
			IEnumerable<Claim> claims
			)
		{
			var tokenClaims = new List<Claim>
			{
				new Claim("id", id),
				new Claim("username", username)
			};

			tokenClaims.AddRange(claims);

			return _jwtHelper.GetJwtToken(
				id,
				_credentials.SigningCredentials,
				_credentials.PublicEncryptionCredentials,
				_credentials.Issuer,
				_credentials.Audience,
				_credentials.TokenTimeSpan,
				tokenClaims);
		}

		public string GetAccountId(string authorization)
		{
			var tokenString = authorization
				.Replace("bearer ", "", true, CultureInfo.CurrentCulture);

			var result = _jwtHelper.ValidateToken(
				tokenString,
				_credentials.SigningKey,
				_credentials.PrivateEncryptionKey,
				_credentials.Issuer,
				_credentials.Audience);

			return result.Claims.First(claim => claim.Type == "id").Value;
		}
	}
}
