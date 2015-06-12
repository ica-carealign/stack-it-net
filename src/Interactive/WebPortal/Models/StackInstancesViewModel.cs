using System;
using System.Collections.Generic;

namespace Ica.StackIt.Interactive.WebPortal.Models
{
	public class StackInstancesViewModel
	{
		public string StackName { get; set; }
		public Guid StackRecordId { get; set; }
		public IEnumerable<InstanceOverviewViewModel> Instances { get; set; }
		public StackPowerViewModel StackPowerViewModel { get; set; }
	}
}