using System;
using System.Collections.Generic;
using System.Linq;

using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;
using Ica.StackIt.Interactive.WebPortal.Models;

namespace Ica.StackIt.Interactive.WebPortal.ViewModelHelpers
{
	public class StackViewModelHelper : IStackViewModelHelper
	{
		private readonly IRepository<Instance> _instanceRepository;
		private Dictionary<Guid, IEnumerable<Instance>> StackInstances { get; set; }

		public StackViewModelHelper(IRepository<Instance> instanceRepository)
		{
			_instanceRepository = instanceRepository;
			StackInstances = new Dictionary<Guid, IEnumerable<Instance>>();
		}

		public StackPowerViewModel CreateStackPowerViewModel(Stack stack)
		{
			IEnumerable<Instance> instances = GetInstances(stack);

			var model = new StackPowerViewModel
			{
				StackName = stack.Name,
				StackRecordId = stack.Id,
				StackPowerState = null
			};

			foreach (var instance in instances)
			{
				if (model.StackPowerState == null)
				{
					model.StackPowerState = instance.State;
				}
				else if (model.StackPowerState != instance.State)
				{
					model.StackPowerState = "mixed";
				}
			}

			return model;
		}

		public StackInstancesViewModel CreateStackInstancesViewModel(Stack stack)
		{
			var instances = GetInstances(stack);

			var model = new StackInstancesViewModel
			{
				StackName = stack.Name,
				StackRecordId = stack.Id,
				Instances = instances.Select(x => new InstanceOverviewViewModel { State = x.State })
			};

			return model;
		}

		private IEnumerable<Instance> GetInstances(Stack stack)
		{
			if(!StackInstances.ContainsKey(stack.Id))
			{
				var instances = stack.InstanceIds.Select(x => _instanceRepository.Find(x)).Where(x => x != null);
				StackInstances[stack.Id] = instances;
			}

			return StackInstances[stack.Id];
		}
	}
}