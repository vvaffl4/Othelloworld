using Microsoft.AspNetCore.Http;

namespace Othelloworld.Services
{
	public interface IAccountService
	{
		string GetAccountId(string authorization);
	}
}
