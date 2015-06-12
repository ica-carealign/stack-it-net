using System.Collections.Generic;

namespace Ica.StackIt.Core.Entities
{
	public class ResourceRecord : ICloudResource
	{
		public ResourceRecord()
		{
			Values = new List<string>();
		}

		public string FullyQualifiedDomainName { get; set; }
		public string Type { get; set; }
		public int TimeToLive { get; set; }

		/// <summary>
		///     Resource record data
		/// </summary>
		/// <example>
		///     {
		///     ns-2021.awsdns-60.co.uk,
		///     ns-252.awsdns-31.com,
		///     ns-1321.awsdns-37.org,
		///     ns-611.awsdns-12.net,
		///     }
		/// </example>
		public ICollection<string> Values { get; set; }

		public string ResourceId { get; set; }

		public string ResourceType
		{
			get { return "ResourceRecord"; }
		}
	}
}