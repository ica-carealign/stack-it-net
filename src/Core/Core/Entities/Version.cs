using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Core.Entities
{
	public class Version
	{
		public Version()
		{
			Roles = new List<Role>();
		}

		[Required]
		public string Name { get; set; }

		public string IamRole { get; set; }

		public List<Role> Roles { get; set; }
	}
}