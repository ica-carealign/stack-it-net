using System;
using System.ComponentModel;

namespace Ica.StackIt.Interactive.WebPortal.Models
{
	public class StackOverviewViewModel
	{
		public Guid Id { get; set; }

		[DisplayName("Name")]
		public string Name { get; set; }

		[DisplayName("Creation Time")]
		public DateTime CreateTime { get; set; }

		[DisplayName("Status")]
		public string Status { get; set; }

		[DisplayName("Description")]
		public string Description { get; set; }

		[DisplayName("Owner User Name")]
		public string OwnerUserName { get; set; }

		[DisplayName("Total Cost")]
		public decimal TotalCost { get; set; }

		[DisplayName("Notes")]
		public string Notes { get; set; }

		[DisplayName("Schedule")]
		public bool ScheduleEnabled { get; set; }

		public Guid OwnerProfileId { get; set; }

		public StackPowerViewModel StackPowerViewModel { get; set; }

		public StackInstancesViewModel StackInstanceViewModel { get; set; }
	}
}