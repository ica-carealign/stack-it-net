using System;
using System.Linq;

using EdgeJs;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Application.Model;
using Ica.StackIt.Application.Properties;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Newtonsoft.Json;

using Instance = Ica.StackIt.Core.Entities.Instance;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.OrderedQueueName)]
	public class CreateStack : CommandBase
	{
		private readonly IRepository<IPRange> _ipRangeRepository;
		private readonly IRepository<Instance> _instanceRepository;
		private readonly IRepository<Stack> _stackRepository;
		private readonly IRepository<BaseImage> _baseImageRepository;
		private readonly IRepository<Schedule> _scheduleRepository;
		private readonly IStackPowerKickstarter _stackPowerKickstarter;

		public CreateStack(
			IRepository<AwsProfile> profileRepository,
			IRepository<IPRange> ipRangeRepository,
			IRepository<Instance> instanceRepository,
			IRepository<Stack> stackRepository,
			IRepository<BaseImage> baseImageRepository,
			IRepository<Schedule> scheduleRepository,
			IStackPowerKickstarter stackPowerKickstarter,
			IAwsClientFactory awsClientFactory) : base(profileRepository, awsClientFactory)
		{
			_ipRangeRepository = ipRangeRepository;
			_instanceRepository = instanceRepository;
			_stackRepository = stackRepository;
			_baseImageRepository = baseImageRepository;
			_scheduleRepository = scheduleRepository;
			_stackPowerKickstarter = stackPowerKickstarter;
		}

		public void Execute(Guid profileId, StackConfiguration configuration)
		{
			IAwsClient awsClient;
			if (!TryInitializeClient(profileId, out awsClient))
			{
				return;
			}

			// Go ahead and reserve some IP addresses for this stack
			// If building the stack fails, the refresh IP range job will make those IP addresses available again anyway
			IPRange ipRange = _ipRangeRepository.FindAll().Single(x => x.AwsProfileId == profileId);
			var reservedIps = ipRange.Addresses.Values.Where(x => !x.IsInUse).Take(configuration.Instances.Count());

			var index = 0;
			foreach(var ip in reservedIps)
			{
				ip.IsInUse = true;
				configuration.Instances[index].PrivateAddresses.Add(ip.Address.ToString());
				index++;
			}

			var stack = new Stack();
			foreach (var instance in configuration.Instances)
			{
				var baseImage = _baseImageRepository.Find(instance.Role.BaseImageId);
				configuration.BaseImages.Add(baseImage);
				instance.Id = Guid.NewGuid();
				instance.Tags.Add(new Tag {Name = "StackItId", Value = instance.Id.ToString()});

				_instanceRepository.Add(instance);

				stack.InstanceIds.Add(instance.Id);
			}
			// Do not change instance after this line and expect the value to be in the database!

			_ipRangeRepository.Update(ipRange);

			object templateObj = GetTemplate(configuration);
			var templateString = JsonConvert.SerializeObject(templateObj);

			awsClient.StackService.CreateStack(configuration.StackName, templateString);

			// Create a bare-bones stack in the database with the values that are not stored in AWS
			stack.NeedsRefreshing = true;
			stack.OwnerProfileId = configuration.OwnerProfileId;
			stack.OwnerUserName = configuration.OwnerUserName;
			stack.CreateTime = DateTime.UtcNow;
			stack.Name = configuration.StackName;
			stack.Notes = configuration.Notes;
			stack.CreatedByApplication = true;
			stack.ScheduleEnabled = configuration.ScheduleEnabled;

			var selectedSchedule = _scheduleRepository.Find(configuration.ScheduleId);
			stack.ScheduleId = selectedSchedule.Id;

			_stackRepository.Add(stack);

			_stackPowerKickstarter.KickstartSchedule(stack);
		}

		private static object GetTemplate(StackConfiguration configuration)
		{
			var script = Resources.GenerateCloudFormationTemplate;

			var func = Edge.Func(script);

			return func(configuration).Result;
		}
	}
}