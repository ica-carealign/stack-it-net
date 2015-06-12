using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Core.Entities
{
	public class AwsProfile : IEntity<Guid>
	{
		public AwsProfile()
		{
			Groups = new List<string>();
			BillingMetadata = new Dictionary<string, BillingMetadata>();
		}

		public Guid Id { get; set; }

		[Required]
		public string Account { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string AccessKeyId { get; set; }

		[Required]
		public string EncryptedKey { get; set; }

		[Required]
		public string EncryptedSecretAccessKey { get; set; }

		[Required]
		public string DefaultVpcId { get; set; }

		[Required]
		public string DefaultSubnetId { get; set; }

		[Required]
		public string HostedZone { get; set; }

		/// <summary>
		///     The S3 bucket where detailed billing reports for this AWS account may be stored.
		/// </summary>
		public string DetailedBillingS3Bucket { get; set; }

		/// <summary>
		///     True if all billing history for this profile has been loaded, which indicates the profile is ready for incremental
		///     billing updates.
		/// </summary>
		public bool IsBillingHistoryLoaded { get; set; }

		/// <summary>
		///     True if the billing history is currently being loaded, which indicates another process should not also load it.
		/// </summary>
		public bool IsBillingHistoryLoading { get; set; }

		/// <summary>
		///     Metadata about the processing of billing data for this AWS profile.
		///     Keyed by the period ID, which is the month and year in the form "YYYY-MM".
		/// </summary>
		public IDictionary<string, BillingMetadata> BillingMetadata { get; set; }

		// TODO: Once we create a billing profile type, we must be sure to change the CreateDefaultSecurityGroup command
		// so that it doesn't try to create a deafult security group in the case of a billing profile type.
		/// <summary>
		///     The default security group that will be assigned to all instances created with this profile.
		/// </summary>
		public string DefaultSecurityGroupId { get; set; }

		public IList<string> Groups { get; set; }
	}
}