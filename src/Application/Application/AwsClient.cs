using System;

using Amazon;
using Amazon.CloudFormation;
using Amazon.EC2;
using Amazon.IdentityManagement;
using Amazon.Route53;
using Amazon.Runtime;
using Amazon.S3;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Configuration;
using Ica.StackIt.Application.Parser;

namespace Ica.StackIt.Application
{
	internal class AwsClient : IAwsClient
	{
		private readonly AWSCredentials _credentials;
		private readonly Lazy<IAmazonCloudFormation> _cloudFormationClient;
		private readonly Lazy<IAmazonEC2> _ec2Client;
		private readonly Lazy<IAmazonRoute53> _route53Client;
		private readonly Lazy<IAmazonS3> _s3Client;
		private readonly Lazy<IAmazonIdentityManagementService> _iamClient;
		private readonly IStackItConfiguration _configuration;

		public AwsClient(AWSCredentials credentials, IStackItConfiguration configuration)
		{
			_credentials = credentials;

			_cloudFormationClient = new Lazy<IAmazonCloudFormation>(() => AWSClientFactory.CreateAmazonCloudFormationClient(_credentials));
			_ec2Client = new Lazy<IAmazonEC2>(() => AWSClientFactory.CreateAmazonEC2Client(_credentials));
			_route53Client = new Lazy<IAmazonRoute53>(() => AWSClientFactory.CreateAmazonRoute53Client(_credentials));
			_s3Client = new Lazy<IAmazonS3>(() => AWSClientFactory.CreateAmazonS3Client(_credentials));
			_iamClient = new Lazy<IAmazonIdentityManagementService>(() => AWSClientFactory.CreateAmazonIdentityManagementServiceClient(_credentials));

			_configuration = configuration;
		}

		public IInstanceService InstanceService
		{
			get { return new InstanceService(_ec2Client.Value); }
		}

		public IStackService StackService
		{
			get { return new StackService(_cloudFormationClient.Value, StorageService, _configuration.CloudOptions.ConfigurationTemplateBucket); }
		}

		public IDnsService DnsService
		{
			get { return new DnsService(_route53Client.Value); }
		}

		public IImageService ImageService
		{
			get { return new ImageService(_ec2Client.Value); }
		}

		public ISecurityGroupService SecurityGroupService
		{
			get { return new SecurityGroupService(_ec2Client.Value); }
		}

		public INetworkService NetworkService
		{
			get { return new NetworkService(_ec2Client.Value); }
		}

		public IStorageService StorageService
		{
			get { return new StorageService(_s3Client.Value, new S3PathParser()); }
		}

		public IIdentityService IdentityService
		{
			get { return new IdentityService(_iamClient.Value, new ArnParser()); }
		}
	}
}