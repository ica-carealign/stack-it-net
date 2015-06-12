using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Ica.StackIt.Interactive.WebPortal.Models
{
	public class DeleteStackViewModel
	{
		public StackOverviewViewModel StackModel { get; set; }

		[DisplayName("Instances")]
		public IEnumerable<InstanceOverviewViewModel> InstanceModels { get; set; }

		[DisplayName("AWS Profile")]
		public Guid SelectedProfileId { get; set; }
	}
}