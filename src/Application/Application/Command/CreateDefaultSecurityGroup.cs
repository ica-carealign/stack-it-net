using System;

using Amazon.EC2;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.OrderedQueueName)]
	public class CreateDefaultSecurityGroup : CommandBase
	{
		private const string _unauthorizedMarker = "not authorized";

		public CreateDefaultSecurityGroup(
			IRepository<AwsProfile> profileRepository,
			IAwsClientFactory clientFactory)
			: base(profileRepository, clientFactory)
		{
		}

		public void Execute(Guid profileId, string vpcId)
		{
			IAwsClient client;
			AwsProfile profile;
			if (!TryInitialize(profileId, out client, out profile))
			{
				return;
			}

			var creator = new DefaultSecurityGroupCreator(client.SecurityGroupService);
			string groupId;
			try
			{
				groupId = creator.TryCreateDefaultSecurityGroup(vpcId);
			}
			catch (AmazonEC2Exception e)
			{
				// TODO: Once we create a billing profile type, we won't need to do this anymore
				// because the billing profile type will not need to create any kind of security group.
				if (e.Message.Contains(_unauthorizedMarker))
				{
					return;
				}
				throw;
			}
			profile.DefaultSecurityGroupId = groupId;
			ProfileRepository.Update(profile);
		}
	}
}