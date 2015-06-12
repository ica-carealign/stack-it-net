using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Ica.StackIt.Core.Constants;

namespace Ica.StackIt.Interactive.WebPortal.Models.Product
{
	public class RoleViewModel
	{
		public Guid ProductId { get; set; }
		public string ProductName { get; set; }

		public string VersionName { get; set; }

		[Display(Name = "Base Image")]
		public Dictionary<Guid, string> BaseImages { get; set; }

		public Guid BaseImageId { get; set; }

		[Display(Name = "Role Name")]
		[Required]
		public string Name { get; set; }

		public List<PortViewModel> Ports { get; set; }

		public List<string> CurrentRoles { get; set; }

		public List<string> InstanceTypes { get; set; }

		[Display(Name = "Instance Type")]
		public string SelectedInstanceType { get; set; }

		public List<string> DiskTypes
		{
			get { return VolumeType.GetAll().ToList(); }
		}

		[Display(Name = "Disk Type")]
		public string SelectedDiskType { get; set; }

		[Display(Name = "Volume Size (GB)")]
		public int VolumeSize { get; set; }

		[Display(Name = "Schedule")]
		public string Schedule { get; set; }

		public string Error { get; set; }
	}
}