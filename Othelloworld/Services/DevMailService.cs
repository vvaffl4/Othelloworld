using Microsoft.Extensions.Logging;

namespace Othelloworld.Services
{
	public class DevMailService : IMailService
	{
		private readonly ILogger<DevMailService> _logger;

		public DevMailService(ILogger<DevMailService> logger)
		{
			_logger = logger;
		}

		public bool SendMail(string from, string subject, string body)
		{
			_logger.LogInformation($"From email: {from}, with subject: {subject}, and body: {body}");

			return true;
		}
	}
}
