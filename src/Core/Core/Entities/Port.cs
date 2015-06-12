using System.Collections.Generic;

namespace Ica.StackIt.Core.Entities
{
	public class Port
	{
		/// <summary>
		/// Provides becomes a hostname that the deployed applications can use.
		/// </summary>
		public string Provides { get; set; }
		public int PortNumber { get; set; }
		public bool Tcp { get; set; }
		public bool Udp { get; set; }
		public bool External { get; set; }
		public bool Inbound { get; set; }
		public bool Outbound { get; set; }
		public bool Clusterable { get; set; }

		/// <summary>
		/// DNS records
		/// </summary>
		public ICollection<ResourceRecord> ResourceRecords { get; set; } 
	}
}