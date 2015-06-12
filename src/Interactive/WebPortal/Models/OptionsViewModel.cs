using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Interactive.WebPortal.Models
{
	public class OptionsViewModel
	{
		[Display(Name="Instance Name")]
		public string InstanceName { get; set; }

		[Display(Name = "Instance Type")]
		public string InstanceType { get; set; }

		[Display(Name = "Volume Type")]
		public string VolumeType { get; set; }

		[Display(Name = "Volume Size")]
		public int VolumeSize { get; set; }

		[Display(Name = "IAM Role")]
		public string IamRole { get; set; }
	}
}