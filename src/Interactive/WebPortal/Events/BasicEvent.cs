namespace Ica.StackIt.Interactive.WebPortal.Events
{
	public class BasicEvent : IEvent
	{
		public string EventType { get; set; }
		public dynamic Content { get; set; }
	}
}