namespace Ica.StackIt.Interactive.WebPortal.Models
{
	public class InstanceOverviewViewModel
	{
		public bool? Status { get; set; }
		public string Name { get; set; }
		public string Image { get; set; }
		public string Type { get; set; }
		public string PrivateIp { get; set; }
		public string PublicIp { get; set; }
		public string Platform { get; set; }
		public string State { get; set; }
		public string ResourceId { get; set; }
		public HttpAccessibility HttpAccessability{ get; set; }
	}
}