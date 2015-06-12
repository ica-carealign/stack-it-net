using System;
using System.Collections.Generic;
using System.Linq;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using InstanceResource = Amazon.EC2.Model.Instance;
using InstanceEntity = Ica.StackIt.Core.Entities.Instance;
using SecurityGroupEntity = Ica.StackIt.Core.Entities.SecurityGroup;
using SecurityGroupResource = Amazon.EC2.Model.SecurityGroup;
using Tag = Amazon.EC2.Model.Tag;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.UnorderedQueueName)]
	public class UpdateAllInstances : CommandBase
	{
		private readonly IRepository<InstanceEntity> _instanceRepository;

		public UpdateAllInstances(
			IAwsClientFactory awsClientFactory,
			IRepository<InstanceEntity> instanceRepository,
			IRepository<AwsProfile> profileRepository
			) : base(profileRepository, awsClientFactory)
		{
			_instanceRepository = instanceRepository;
		}

		/// <summary>
		///     Refresh all instance entities with the current instance data from AWS
		/// </summary>
		/// <param name="profileId"></param>
		[DisableConcurrentExecution(60)]
		public void Execute(Guid profileId)
		{
			IAwsClient awsClient;
			if (!TryInitializeClient(profileId, out awsClient))
			{
				return;
			}

			List<InstanceResource> awsInstances = awsClient.InstanceService.GetAllInstances().ToList();

			Dictionary<string, List<SecurityGroupResource>> securityGroupsFor = awsClient.SecurityGroupService.GetSecurityGroupMap(awsInstances);

			foreach (InstanceResource awsInstance in awsInstances)
			{
				List<SecurityGroupEntity> securityGroups = securityGroupsFor[awsInstance.InstanceId]
					.Select(x => new SecurityGroupEntity {Name = x.GroupName, ResourceId = x.GroupId}).ToList();

				string instanceId = awsInstance.InstanceId;

				InstanceEntity dbInstance = _instanceRepository.FindAll()
				                                               .FirstOrDefault(x => StackItIdTagsMatch(x, awsInstance) || x.ResourceId == instanceId);
				InstanceEntity instanceToPersist = dbInstance ?? new InstanceEntity();

				SaveMechanic saveMechanic = dbInstance == null ? SaveMechanic.Add : SaveMechanic.Update;

				UpdateInstance.Map(awsInstance, instanceToPersist, saveMechanic);
				instanceToPersist.OwnerProfileId = profileId;
				instanceToPersist.SecurityGroups = securityGroups;
				UpdateInstance.Persist(_instanceRepository, instanceToPersist, saveMechanic);
			}
		}

		private static bool StackItIdTagsMatch(InstanceEntity instanceEntity, InstanceResource instanceResource)
		{
			const string tagName = "StackItId";
			var instanceEntityTag = instanceEntity.Tags.FirstOrDefault(x => x.Name == tagName);
			if (instanceEntityTag == null)
			{
				return false;
			}

			var instanceResourceTag = instanceResource.Tags.FirstOrDefault(x => x.Key == tagName);
			if (instanceResourceTag == null)
			{
				return false;
			}

			return instanceEntityTag.Value == instanceResourceTag.Value;
		}
	}
}