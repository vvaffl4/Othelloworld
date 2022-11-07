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
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Othelloworld.Services
{
	public class AccountService : IAccountService
	{
		private static readonly string GoogleRecaptchaApiUrl = "https://www.google.com/recaptcha/api/siteverify";

		private Credentials _credentials;
		private JwtHelper _jwtHelper;
		private readonly IHttpClientFactory _httpClientFactory;

		public AccountService(
			Credentials credentials,
			JwtHelper jwtHelper,
			IHttpClientFactory httpClientFactory)
		{
			_credentials = credentials;
			_jwtHelper = jwtHelper;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<bool> VerifyCaptchaTokenAsync (string token)
		{
			var httpClient = _httpClientFactory.CreateClient();

			UriBuilder builder = new UriBuilder(GoogleRecaptchaApiUrl);
			builder.Query = $"secret={_credentials.CaptchaSecret}&response={token}";

			var httpResponse = await httpClient.GetAsync(builder.Uri);

			if (httpResponse.IsSuccessStatusCode)
			{
				var recaptchaResponse = await httpResponse.Content.ReadAsAsync<GoogleRecaptchaResponse>();

				return recaptchaResponse.Success;
			}
			return false;

			//using (var client = new WebClient())
			//{
			//	var response = client.DownloadString(QueryHelpers.AddQueryString(, query));
			//	var success = (bool)JObject.Parse(response)["success"];

			//	return success;
			//}
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
	public class GoogleRecaptchaResponse
	{
		public bool Success { get; set; }
	}
}
