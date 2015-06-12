using System.Collections.Generic;
using System.Linq;

using AwsContrib.EnvelopeCrypto;

using FluentAssertions;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Core.Entities;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.AWS
{
	internal class CredentialsServiceTests
	{
		private Mock<ICryptoProvider> CryptoProviderMock { get; set; }
		private CredentialService CredentialService { get; set; }

		[SetUp]
		public void SetUp()
		{
			CryptoProviderMock = new Mock<ICryptoProvider>();
			CredentialService = new CredentialService(CryptoProviderMock.Object);
		}

		[Test]
		public void GetCredentialsByName()
		{
			// Arrange
			CryptoProviderMock.Setup(x => x.Decrypt("qwerty", "abc")).Returns("abc");

			// Act
			var credential = CredentialService.GetCredentials(new AwsProfile {Name = "default", AccessKeyId = "123", EncryptedSecretAccessKey = "abc", EncryptedKey = "qwerty"});

			// Assert
			var actualCredentials = credential.GetCredentials();
			actualCredentials.AccessKey.Should().Be("123");
			actualCredentials.SecretKey.Should().Be("abc");
		}
	}
}