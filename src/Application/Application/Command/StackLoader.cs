using System;
using System.Collections.Generic;
using System.Linq;

using Amazon.CloudFormation.Model;
using Amazon.Route53.Model;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using AwsStack = Amazon.CloudFormation.Model.Stack;
using AwsTag = Amazon.CloudFormation.Model.Tag;
using AwsStackEvent = Amazon.CloudFormation.Model.StackEvent;
using ResourceRecord = Ica.StackIt.Core.Entities.ResourceRecord;
using Stack = Ica.StackIt.Core.Entities.Stack;
using StackEvent = Ica.StackIt.Core.Entities.StackEvent;

namespace Ica.StackIt.Application.Command
{
	public class StackLoader
	{
		private readonly IRepository<Stack> _stackRepository;
		private readonly IRepository<Instance> _instanceRepository;
		private readonly Lazy<List<Instance>> _repositoryInstances;

		public StackLoader(IRepository<Stack> stackRepository, IRepository<Instance> instanceRepository)
		{
			_stackRepository = stackRepository;
			_instanceRepository = instanceRepository;
			_repositoryInstances = new Lazy<List<Instance>>(() => _instanceRepository.FindAll().ToList());
		}

		public void LoadStack(IAwsClient awsClient, AwsStack stack, string hostedZone, Guid profileId)
		{
			Stack dbStack = _stackRepository.FindAll().FirstOrDefault(x => x.Name == stack.StackName);

			IEnumerable<StackResource> stackResources = awsClient.StackService.GetResources(stack).ToList();
			List<ResourceRecord> resourceRecords = GetResourceRecords(awsClient, stackResources, stack, hostedZone);
			List<Guid> instanceIds = GetInstanceIds(stackResources).ToList();

			Stack stackToPersist = dbStack ?? new Stack();
			SaveMechanic saveMechanic = dbStack == null ? SaveMechanic.Add : SaveMechanic.Update;

			Map(stack, stackToPersist);
			stackToPersist.ResourceRecords = resourceRecords;
			if (!stackToPersist.CreatedByApplication)
			{
				stackToPersist.InstanceIds = instanceIds;
			}
			stackToPersist.StackEvents = GetStackEvents(awsClient, stack);
			stackToPersist.OwnerProfileId = profileId;
			Persist(stackToPersist, saveMechanic);
		}

		private void Persist(Stack stackToPersist, SaveMechanic saveMechanic)
		{
			switch (saveMechanic)
			{
				case SaveMechanic.Add:
					_stackRepository.Add(stackToPersist);
					break;
				case SaveMechanic.Update:
					_stackRepository.Update(stackToPersist);
					break;
				default:
					throw new InvalidOperationException("Unknown save mechanic. This should never happen.");
			}
		}

		private static void Map(AwsStack src, Stack dest)
		{
			dest.Description = src.Description;
			dest.Name = src.StackName;
			dest.CreateTime = src.CreationTime;
			dest.ResourceId = src.StackId;
			dest.Status = src.StackStatus;
			dest.NeedsRefreshing = false;
		}

		private static List<StackEvent> GetStackEvents(IAwsClient awsClient, AwsStack awsStack)
		{
			IEnumerable<AwsStackEvent> awsEvents = awsClient.StackService.GetStackEvents(awsStack);
			List<StackEvent> events = awsEvents.Select(x => new StackEvent
			{
				LogicalId = x.LogicalResourceId,
				Reason = x.ResourceStatusReason,
				Status = x.ResourceStatus,
				TimeStamp = x.Timestamp,
				Type = x.ResourceType
			}).ToList();
			return events;
		}

		private IEnumerable<Guid> GetInstanceIds(IEnumerable<StackResource> stackResources)
		{
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (StackResource ec2Instance in stackResources.Where(x => x.ResourceType == ResourceTypeConstants.Ec2Instance))
			{
				StackResource scopedInstance = ec2Instance;
				Instance instance = _repositoryInstances.Value.FirstOrDefault(x => x.ResourceId == scopedInstance.PhysicalResourceId);

				if (instance != null)
				{
					yield return instance.Id;
				}
			}
		}

		private static List<ResourceRecord> GetResourceRecords(IAwsClient awsClient, IEnumerable<StackResource> stackResources, AwsStack awsStack, string hostedZoneName)
		{
			var resourceRecords = new List<ResourceRecord>();
			if (stackResources.Any(x => x.ResourceType == ResourceTypeConstants.RecordSet))
			{
				string zoneName = hostedZoneName.Trim(new[] {'.'});
				HostedZone hostedZone = awsClient.DnsService.GetHostedZoneByName(zoneName);

				IEnumerable<ResourceRecordSet> resourceRecordSets = awsClient.DnsService.GetResourceRecordSets(hostedZone);
				string nameFilter = string.Format("{0}.{1}", awsStack.StackName, zoneName);

				resourceRecords = resourceRecordSets
					.Where(x => x.Name.ToLower().Contains(nameFilter.ToLower()))
					.Select(x => new ResourceRecord
					{
						FullyQualifiedDomainName = x.Name,
						TimeToLive = (int) x.TTL,
						Type = x.Type,
						Values = x.ResourceRecords.Select(rr => rr.Value).ToList(),
						ResourceId = x.Name // resource records don't have an ID in AWS, but this is unique so it's good enough for now
					}).ToList();
			}

			return resourceRecords;
		}
	}
}