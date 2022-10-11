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

namespace Othelloworld.Services
{
	public class AccountService : IAccountService
	{
		private IConfiguration _configuration;
		private JwtHelper _jwtHelper;
		public AccountService(
			IConfiguration configuration, 
			JwtHelper jwtHelper)
		{
			_configuration = configuration;
			_jwtHelper = jwtHelper;
		}

		public string CreateJwtToken(
			string id, 
			string username,
			IEnumerable<Claim> claims
			//string signingKey,
			//string credentialKey,
			//string issuer,
			//string audience,
			//int timeoutMinutes
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
				_configuration.GetValue<string>("SigningKey"),
				_configuration.GetValue<string>("EncryptionKey"),
				_configuration.GetValue<string>("Issuer"),
				_configuration.GetValue<string>("Audience"),
				TimeSpan.FromMinutes(_configuration.GetValue<int>("TokenTimeoutMinutes")),
				tokenClaims);
		}

		public string GetAccountId(string authorization)
		{
			var tokenString = authorization
				.Replace("bearer ", "", true, CultureInfo.CurrentCulture);

			var result = _jwtHelper.ValidateToken(
				tokenString,
				_configuration.GetValue<string>("SigningKey"),
				_configuration.GetValue<string>("EncryptionKey"),
				_configuration.GetValue<string>("Issuer"),
				_configuration.GetValue<string>("Audience"));

			return result.ClaimsIdentity.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
		}
	}
}
