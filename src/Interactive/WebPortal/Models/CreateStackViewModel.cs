using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

using ProductEntity = Ica.StackIt.Core.Entities.Product;

namespace Ica.StackIt.Interactive.WebPortal.Models
{
	public class CreateStackViewModel
	{
		[Display(Name = "AWS Profile")]
		public Guid SelectedProfileId { get; set; }

		[Display(Name = "Stack Name")]
		public string StackName { get; set; }

		[Display(Name = "VPC Id")]
		public string SelectedVpcId { get; set; }

		[Display(Name = "Subnet Id")]
		public string SelectedSubnetId { get; set; }

		[Display(Name = "Notes")]
		public string Notes { get; set; }

		[Display(Name = "Schedule")]
		public List<ScheduleViewModel> Schedules { get; set; }

		public Guid SelectedScheduleId { get; set; }

		public bool ScheduleEnabled { get; set; }

		public List<Guid> SelectedProductIds { get; set; }
		public List<string> SelectedVersionNames { get; set; }
		public List<string> SelectedRoleNames { get; set; }
		public List<OptionsViewModel> Options { get; set; }

		public List<ProductEntity> Products { get; set; }
		public List<string> InstanceTypes { get; set; }

	}
}