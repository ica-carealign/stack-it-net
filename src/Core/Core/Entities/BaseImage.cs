using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Core.Entities
{
	public class BaseImage : IEntity<Guid>, ICloudResource
	{
		public BaseImage()
		{
			Tags = new Collection<Tag>();
			ProductionAmi = new Dictionary<Guid, string>();
			EdgeAmi = new Dictionary<Guid, string>();
		}

		[Required]
		public Guid Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string ResourceId { get; set; }

		public ICollection<Tag> Tags { get; set; }

		/// <summary>
		/// Defines a dictionary of a profile entity ID to an AMI ID for production ready AMIs
		/// </summary>
		public IDictionary<Guid, string> ProductionAmi { get; set; }

		/// <summary>
		/// Defines a dictionary of a profile entity ID to an AMI ID for bleeding edge / testing AMIs
		/// </summary>
		public IDictionary<Guid, string> EdgeAmi { get; set; }

		public string Platform { get; set; }

		public string RootDeviceName { get; set; }

		public string ResourceType
		{
			get { return "AMI"; }
		}
	}
}