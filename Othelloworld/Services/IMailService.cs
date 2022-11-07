namespace Othelloworld.Services
{
	public interface IMailService
	{
		public bool SendMail(string from, string subject, string body);
	}
}
