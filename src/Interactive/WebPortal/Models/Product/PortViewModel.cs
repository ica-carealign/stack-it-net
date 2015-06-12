namespace Ica.StackIt.Interactive.WebPortal.Models.Product
{
	public class PortViewModel
	{
		public string Provides { get; set; }
		public int PortNumber { get; set; }

		public bool Tcp { get; set; }
		public bool Udp { get; set; }

		public bool Inbound { get; set; }
		public bool Outbound { get; set; }

		public bool Clusterable { get; set; }
		public bool External { get; set; }

		public string DnsName { get; set; }
		public int DnsTtl { get; set; }
	}
}