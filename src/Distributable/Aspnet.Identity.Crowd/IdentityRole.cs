using Microsoft.AspNet.Identity;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	public class IdentityRole : IRole<string>
	{
		public string Id
		{
			get { return Name; }
		}

		public string Name { get; set; }
	}
}