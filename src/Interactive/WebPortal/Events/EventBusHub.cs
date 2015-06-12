using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Ica.StackIt.Interactive.WebPortal.Events
{
	[HubName("eventBus")]
	public class EventBusHub : Hub<IEventSubscriber>
	{
		private readonly EventBus _bus;

		public EventBusHub(EventBus bus)
		{
			_bus = bus;
		}

		public void Heartbeat()
		{
			_bus.Heartbeat();
		}

		public void BroadcastMessage(string message)
		{
			_bus.BroadcastMessage(message);
		}
	}
}