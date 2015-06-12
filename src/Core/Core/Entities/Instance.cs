using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Core.Entities
{
	public class Instance : IEntity<Guid>, ICloudResource
	{
		public Instance()
		{
			PublicAddresses = new List<string>();
			PrivateAddresses = new List<string>();
			Tags = new List<Tag>();
			SecurityGroups = new List<SecurityGroup>();
			VolumeResourceIds = new HashSet<string>();
		}

		public Guid Id { get; set; }

		public Guid OwnerProfileId { get; set; }

		[Required]
		public string Name { get; set; }
		public DateTime LaunchTime { get; set; }
		public string InstanceType { get; set; }
		public string State { get; set; }
		public string AvailabilityZone { get; set; }

		/// <summary>
		///     The ssh key name associated with the instance
		/// </summary>
		public string KeyName { get; set; }

		public bool MonitoringState { get; set; }
		public string VirtualizationType { get; set; }

		/// <summary>
		///     EBS or instance storage
		/// </summary>
		public string StorageType { get; set; }

		public string VpcId { get; set; }
		public string SubnetId { get; set; }

		public ICollection<string> PublicAddresses { get; set; }
		public ICollection<string> PrivateAddresses { get; set; }
		public ICollection<SecurityGroup> SecurityGroups { get; set; }
		public ICollection<string> VolumeResourceIds { get; set; }

		public string ProductName { get; set; } // Direct
		public string VersionName { get; set; } // 2.17
		public Role Role { get; set; } // Direct HISP, Web, and Bind

		public string ResourceId { get; set; }

		public ICollection<Tag> Tags { get; set; }

		public string UserData { get; set; }

		public bool NeedsRefreshing { get; set; }

		public string IamRole { get; set; }

		public string ResourceType
		{
			get { return "EC2Instance"; }
		}

		public string SecurityGroupName
		{
			get { return Name + "SG"; }
		}

		public string WaitHandleName
		{
			get { return Name + "WH"; }
		}

		public string WaitConditionName
		{
			get { return Name + "WC"; }
		}
	}
}