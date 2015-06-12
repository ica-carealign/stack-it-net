using System;

using FluentAssertions;

using Ica.StackIt.Application.Parser;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.Parser
{
	internal class ArnParserTests
	{
		private ArnParser Parser { get; set; }

		[SetUp]
		public void SetUp()
		{
			Parser = new ArnParser();
		}

		// These examples came from http://docs.aws.amazon.com/general/latest/gr/aws-arns-and-namespaces.html
		[TestCase("arn:aws:iam::123456789012:root")]
		[TestCase("arn:aws:iam::123456789012:user/Bob")]
		[TestCase("arn:aws:iam::123456789012:user/division_abc/subdivision_xyz/Bob")]
		[TestCase("arn:aws:iam::123456789012:group/Developers")]
		[TestCase("arn:aws:iam::123456789012:group/division_abc/subdivision_xyz/product_A/Developers")]
		[TestCase("arn:aws:iam::123456789012:role/S3Access")]
		[TestCase("arn:aws:iam::123456789012:role/application_abc/component_xyz/S3Access")]
		[TestCase("arn:aws:iam::123456789012:policy/UsersManageOwnCredentials")]
		[TestCase("arn:aws:iam::123456789012:policy/division_abc/subdivision_xyz/UsersManageOwnCredentials")]
		[TestCase("arn:aws:iam::123456789012:instance-profile/Webserver")]
		[TestCase("arn:aws:sts::123456789012:federated-user/Bob")]
		[TestCase("arn:aws:sts::123456789012:assumed-role/Accounting-Role/Mary")]
		[TestCase("arn:aws:iam::123456789012:mfa/BobJonesMFA")]
		[TestCase("arn:aws:iam::123456789012:server-certificate/ProdServerCert")]
		[TestCase("arn:aws:iam::123456789012:server-certificate/division_abc/subdivision_xyz/ProdServerCert")]
		public void ParseAccount_Ok(string arn)
		{
			// Arrange
			const string expected = "123456789012";

			// Act
			string actual = Parser.GetAccountNumber(arn);

			// Assert
			actual.Should().Be(expected);
		}

		[TestCase("Not an ARN lol")]
		[TestCase("arn:aws:sqs:us-east-1:123456789012:queue1")]
		[TestCase("arn:aws:sns:us-east-1:123456789012:my_corporate_topic:02034b43-fefa-4e07-a5eb-3be56f8c54ce")]
		[TestCase("arn:aws:redshift:us-east-1:123456789012:securitygroup:my-public-group")]
		[TestCase("arn:aws:cloudformation:us-east-1:123456789012:stack/MyProductionStack/abc9dbf0-43c2-11e3-a6e8-50fa526be49c")]
		[TestCase("arn:aws:ec2:us-east-1::image/ami-1a2b3c4d")]
		public void ParseAccount_Fail(string arn)
		{
			// Arrange

			// Act
			Action act = () => Parser.GetAccountNumber(arn);

			// Assert
			act.ShouldThrow<ArgumentException>().Where(ex => ex.Message.Contains("Not a valid Identity and Access Management ARN"));
		}
	}
}