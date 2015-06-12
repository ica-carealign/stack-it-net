using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Amazon.EC2;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using AwsInstance = Amazon.EC2.Model.Instance;
using Instance = Ica.StackIt.Core.Entities.Instance;
using SecurityGroup = Amazon.EC2.Model.SecurityGroup;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.UnorderedQueueName)]
	public class UpdateInstance : CommandBase
	{
		private readonly IRepository<Instance> _instanceRepository;

		public UpdateInstance(IRepository<AwsProfile> profileRepository, IAwsClientFactory awsClientFactory, IRepository<Instance> instanceRepository)
			: base(profileRepository, awsClientFactory)
		{
			_instanceRepository = instanceRepository;
		}

		[DisableConcurrentExecution(60)]
		public void Execute(Guid profileId, string instanceId)
		{
			IAwsClient awsClient;
			if (!TryInitializeClient(profileId, out awsClient))
			{
				return;
			}

			AwsInstance awsInstance = awsClient.InstanceService.GetInstance(instanceId);
			if(awsInstance == null)
			{
				// Don't do any work if the instance isn't even in Amazon anymore
				return;
			}

			IEnumerable<SecurityGroup> awsSecurityGroups = awsClient.InstanceService.GetSecurityGroups(awsInstance);
			List<Core.Entities.SecurityGroup> securityGroups = awsSecurityGroups.Select(x => new Core.Entities.SecurityGroup
			{
				Name = x.GroupName,
				ResourceId = x.GroupId
			}).ToList();

			Instance dbInstance = _instanceRepository.FindAll().FirstOrDefault(x => x.ResourceId == instanceId);
			Instance instanceToPersist = dbInstance ?? new Instance();
			SaveMechanic saveMechanic = dbInstance == null ? SaveMechanic.Add : SaveMechanic.Update;

			Map(awsInstance, instanceToPersist, saveMechanic);
			instanceToPersist.OwnerProfileId = profileId;
			instanceToPersist.SecurityGroups = securityGroups;
			Persist(_instanceRepository, instanceToPersist, saveMechanic);
		}

		internal static void Persist(IRepository<Instance> instanceRepository, Instance instanceToPersist, SaveMechanic saveMechanic)
		{
			switch(saveMechanic)
			{
				case SaveMechanic.Add:
					instanceRepository.Add(instanceToPersist);
					break;
				case SaveMechanic.Update:
					instanceRepository.Update(instanceToPersist);
					break;
				default:
					throw new InvalidOperationException("Unknown save mechanic. This should never happen.");
			}
		}

		internal static void Map(AwsInstance src, Instance dest, SaveMechanic mechanic)
		{
			var nameTag = src.Tags.FirstOrDefault(x => x.Key.Equals("name", StringComparison.InvariantCultureIgnoreCase));
			string name = nameTag == null ? string.Format("Unknown-{0}", src.InstanceId) : nameTag.Value;

			List<IPAddress> privateIpAddresses = src.PrivateIpAddress == null ? new List<IPAddress>() : new List<IPAddress> {IPAddress.Parse(src.PrivateIpAddress)};
			List<IPAddress> publicIpAddresses = src.PublicIpAddress == null ? new List<IPAddress>() : new List<IPAddress> {IPAddress.Parse(src.PublicIpAddress)};
			List<string> volumeResourceIds = src.BlockDeviceMappings.Select(x => x.Ebs.VolumeId).ToList();

			dest.AvailabilityZone = src.Placement.AvailabilityZone;
			dest.InstanceType = src.InstanceType.Value;
			dest.KeyName = src.KeyName;
			dest.LaunchTime = src.LaunchTime;
			dest.MonitoringState = src.Monitoring.State != MonitoringState.Disabled;
			dest.Name = name;
			dest.PrivateAddresses = privateIpAddresses.Select(x => x.ToString()).ToList();
			dest.PublicAddresses = publicIpAddresses.Select(x => x.ToString()).ToList();
			dest.VolumeResourceIds = volumeResourceIds;
			dest.ResourceId = src.InstanceId;
			dest.State = src.State.Name;
			dest.StorageType = src.RootDeviceType.Value;
			dest.SubnetId = src.SubnetId;
			dest.VirtualizationType = src.VirtualizationType.Value;
			dest.VpcId = src.VpcId;
			dest.NeedsRefreshing = false;

			if(mechanic == SaveMechanic.Add)
			{
				dest.Id = Guid.NewGuid();
			}
		}
	}
}