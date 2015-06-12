using System;
using System.Net;

namespace Ica.StackIt.Core.Entities
{
	public class SubnetIpAddress
	{
		public IPAddress Address { get; set; }
		public bool IsInUse { get; set; }
		public DateTime LastUpdateTime { get; set; }
	}
}