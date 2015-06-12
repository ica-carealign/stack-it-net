using System;

namespace Ica.StackIt.Core.Entities
{
	public class StackEvent
	{
		public DateTime TimeStamp { get; set; }
		public string Status { get; set; }
		public string Type { get; set; }
		public string LogicalId { get; set; }
		public string Reason { get; set; }
	}
}