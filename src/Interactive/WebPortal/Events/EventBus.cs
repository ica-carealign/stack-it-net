using System;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Ica.StackIt.Interactive.WebPortal.Events
{
	public class EventBus
	{
		// singleton instance
		private static readonly Lazy<EventBus> _instance = new Lazy<EventBus>(
			() => new EventBus(GlobalHost.ConnectionManager.GetHubContext<EventBusHub, IEventSubscriber>().Clients));

		private EventBus(IHubConnectionContext<IEventSubscriber> clients)
		{
			Clients = clients;
		}

		private IHubConnectionContext<IEventSubscriber> Clients { get; set; }

		public static EventBus Instance
		{
			get { return _instance.Value; }
		}

		public static event EventHandler<EventArgs> HeartbeatReceived;

		public void Heartbeat()
		{
			EventHandler<EventArgs> handlers = HeartbeatReceived;
			if (handlers != null)
			{
				handlers(this, new EventArgs());
			}
		}

		public void BroadcastMessage(string message)
		{
			Clients.All.OnEvent(new BasicEvent
			{
				EventType = "BroadcastMessageEvent",
				Content = message
			});
		}
	}
}