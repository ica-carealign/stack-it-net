using System;
using System.Collections.Generic;

namespace Ica.StackIt.Core.Entities
{
	public class Role
	{
		public Role()
		{
			Ports = new List<Port>();
			Options = new RoleOptions();
		}

		public Guid BaseImageId { get; set; }

		public string Name { get; set; }

		public ICollection<Port> Ports { get; set; }

		public RoleOptions Options { get; set; }
	}
}