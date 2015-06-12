using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Core.Entities
{
	public class DnsZone : ICloudResource
	{
		public DnsZone()
		{
			ResourceRecords = new List<ResourceRecord>();
		}

		[Required]
		public string Name { get; set; }

		public ICollection<ResourceRecord> ResourceRecords { get; set; }

		[Required]
		public string ResourceId { get; set; }

		public string ResourceType
		{
			get { return "DnsZone"; }
		}
	}
}