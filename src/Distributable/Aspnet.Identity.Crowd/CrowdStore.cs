using System.Threading.Tasks;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	public abstract class CrowdStore
	{
		protected IAuthenticatedUserClient AuthenticatedUserClient { get; private set; }
		protected TaskFactory TaskFactory { get; private set; }

		protected CrowdStore(IAuthenticatedUserClient authenticatedUserClient)
		{
			AuthenticatedUserClient = authenticatedUserClient;
			TaskFactory = new TaskFactory();
		}
	}
}