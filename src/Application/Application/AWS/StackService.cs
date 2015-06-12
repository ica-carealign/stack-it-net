using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amazon.CloudFormation;
using Amazon.CloudFormation.Model;

namespace Ica.StackIt.Application.AWS
{
	public class StackService : IStackService
	{
		private const string _nonExistenceMarker = "does not exist";

		private readonly IAmazonCloudFormation _cloudFormationClient;
		private readonly IStorageService _storageService;
		private readonly string _bucket;

		public StackService(IAmazonCloudFormation cloudFormationClient, IStorageService storageService, string bucket)
		{
			_cloudFormationClient = cloudFormationClient;
			_storageService = storageService;
			_bucket = bucket;
		}

		public void CreateStack(string stackName, string configurationTemplate)
		{
			var key = string.Format("{0}-{1}", stackName, Guid.NewGuid());

			// The configuration template is trimmed of the trailing slash because the slash is necessary
			// creating a lifecycle rule (which was actually the purpose of the convention in the first place)
			// but will result in a blank-named folder when uploading a file to AWS.
			var prefixedBucket = string.Format("{0}/{1}", _bucket, Conventions.ConfigurationTemplateBucketPrefix.TrimEnd(new []{'/'}));

			var url = _storageService.UploadFile(prefixedBucket, key, Encoding.UTF8.GetBytes(configurationTemplate));

			var request = new CreateStackRequest
			{
				StackName = stackName,
				TemplateURL = url,
				DisableRollback = true
			};

			_cloudFormationClient.CreateStack(request);
		}

		public void DeleteStack(string stackName)
		{
			var request = new DeleteStackRequest
			{
				StackName = stackName
			};

			_cloudFormationClient.DeleteStack(request);
		}

		public IEnumerable<Stack> GetAllStacks()
		{
			DescribeStacksResponse response = _cloudFormationClient.DescribeStacks();
			return response.Stacks;
		}

		public Stack GetStack(string stackName)
		{
			var describeStacksRequest = new DescribeStacksRequest {StackName = stackName};
			DescribeStacksResponse response;
			try
			{
				response = _cloudFormationClient.DescribeStacks(describeStacksRequest);
			}
			catch (AmazonCloudFormationException exception)
			{
				if (exception.Message.Contains(_nonExistenceMarker))
				{
					return null;
				}
				throw;
			}

			return response.Stacks.FirstOrDefault();
		}

		public IEnumerable<StackEvent> GetStackEvents(Stack stack)
		{
			var request = new DescribeStackEventsRequest {StackName = stack.StackName};
			DescribeStackEventsResponse response;
			try
			{
				response = _cloudFormationClient.DescribeStackEvents(request);
			}
			catch (AmazonCloudFormationException exception)
			{
				if (exception.Message.Contains(_nonExistenceMarker))
				{
					return Enumerable.Empty<StackEvent>();
				}
				throw;
			}
			
			return response.StackEvents;
		}

		public IEnumerable<StackResource> GetResources(Stack stack)
		{
			var request = new DescribeStackResourcesRequest {StackName = stack.StackName};
			DescribeStackResourcesResponse response;
			try
			{
				response = _cloudFormationClient.DescribeStackResources(request);
			}
			catch (AmazonCloudFormationException exception)
			{
				if (exception.Message.Contains(_nonExistenceMarker))
				{
					return Enumerable.Empty<StackResource>();
				}
				throw;
			}
			return response.StackResources;
		}
	}
}