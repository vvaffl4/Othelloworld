using Microsoft.AspNetCore.Identity;
using Moq;
using Othelloworld.Data.Models;

namespace OthelloworldTest.Mocks
{
	public static class UserManager
	{
		public static Account Account = new Account
		{
			Id = Guid.NewGuid().ToString(),
			UserName = "UserManager",
			Email = "UserManager@mail.com"
		};

		public static Mock<UserManager<TUser>> New<TUser>(List<TUser> ls) where TUser : IdentityUser
		{
			var store = new Mock<IUserStore<TUser>>();
			var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
			userManager.Object.UserValidators.Add(new UserValidator<TUser>());
			userManager.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

			userManager.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
			userManager.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Success)
				.Callback<TUser, string>((x, y) => ls.Add(x));
			userManager.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

			userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync(() => Account as TUser);

			return userManager;
		}
	}


}
