using System;
using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Interactive.WebPortal.Models.Product
{
	public class VersionViewModel
	{
		[Required]
		public Guid ProductId { get; set; }

		[Display(Name = "Version name")]
		[Required]
		public string Name { get; set; }

		[Display(Name = "Default IAM Role")]
		public string IamRole { get; set; }
	}
}