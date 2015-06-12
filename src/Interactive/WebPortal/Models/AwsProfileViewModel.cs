using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Interactive.WebPortal.Models
{
	public class AwsProfileViewModel
	{
		public AwsProfileViewModel()
		{
			ServerGroups = new List<string>();
			Groups = new List<string>();
		}

		public Guid Id { get; set; }

		[DisplayName("Profile Name")]
		public string Name { get; set; }

		[DisplayName("Account")]
		public string Account { get; set; }

		[DisplayName("Access Key ID")]
		public string AccessKeyId { get; set; }

		[DisplayName("Secret Access Key")]
		public string SecretAccessKey { get; set; }

		[DisplayName("Default VPC ID")]
		public string DefaultVpcId { get; set; }

		[DisplayName("Subnet ID")]
		public string DefaultSubnetId { get; set; }

		[DisplayName("Hosted Zone")]
		public string HostedZone { get; set; }

		[DisplayName("AD Groups")]
		public List<string> Groups { get; set; }

		public List<string> ServerGroups { get; set; }

		[Display(Name = "Detailed Billing S3 Bucket", Description = "The S3 bucket where Amazon deposits detailed billing reports for this AWS profile.")]
		public string DetailedBillingS3Bucket { get; set; }
	}
}