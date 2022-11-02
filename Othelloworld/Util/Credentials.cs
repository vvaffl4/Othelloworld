using Microsoft.IdentityModel.Tokens;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Othelloworld.Util
{
	public class Credentials
	{
		private RSA _key;

		public Credentials(
			string issuer,
			string audience,
			string encryptionKey,
			string signingKey,
			string captchaSecret,
			TimeSpan tokenTimeSpan)
		{
			_key = RSA.Create(3072);

			Issuer = issuer;
			Audience = audience;
			CaptchaSecret = captchaSecret;
			TokenTimeSpan = tokenTimeSpan;

			PrivateEncryptionKey = new RsaSecurityKey(_key) { KeyId = encryptionKey };
			PublicEncryptionKey = new RsaSecurityKey(_key.ExportParameters(false)) { KeyId = encryptionKey };
			PublicEncryptionCredentials = new EncryptingCredentials(
				PublicEncryptionKey, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512);

			SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
			SigningCredentials = new SigningCredentials(
				SigningKey,
				SecurityAlgorithms.HmacSha256);
		}

		public RsaSecurityKey PrivateEncryptionKey { get; }
		public RsaSecurityKey PublicEncryptionKey { get; }
		public EncryptingCredentials PublicEncryptionCredentials { get; }

		public SymmetricSecurityKey SigningKey { get; }
		public SigningCredentials SigningCredentials { get; }

		public string Issuer { get; }
		public string Audience { get; }
		public TimeSpan TokenTimeSpan { get; }
		public string CaptchaSecret { get; }
	}
}
