using Moq;
using Othelloworld.Data.Models;
using Othelloworld.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloworldTest.Mocks
{
	public static class AccountService
	{
		public static Account Account = new Account
		{
			Id = Guid.NewGuid().ToString(),
			UserName = "AccountService",
			Email = "AccountService@mail.com"
		};

		public static Mock<IAccountService> New()
		{
			var accountServiceMock = new Mock<IAccountService>();

			accountServiceMock.Setup(x =>
				x.GetAccountId(
					It.IsAny<string>()
			))
				.Returns(Account.Id);

			return accountServiceMock;
		}
	}
}
