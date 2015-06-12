namespace Ica.StackIt.Interactive.WebPortal.Events
{
	public interface IEventSubscriber
	{
		void OnEvent(IEvent e);
	}
}