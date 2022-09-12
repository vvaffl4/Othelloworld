using Microsoft.AspNetCore.Http;
using Othelloworld.Util;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Othelloworld.Services
{
	public class AccountService : IAccountService
	{
		public string GetAccountId(string authorization)
		{
			var tokenString = authorization
				.Replace("bearer ", "", true, CultureInfo.CurrentCulture);

			var token = JwtHelper.ReadJwtToken(tokenString);

			return token.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
		}
	}
}
