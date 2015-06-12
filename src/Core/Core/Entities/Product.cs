using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Core.Entities
{
	public class Product : IEntity<Guid>
	{
		public Product()
		{
			Versions = new List<Version>();
		}

		public Guid Id { get; set; }

		[Required]
		public string Name { get; set; }

		public List<Version> Versions { get; set; }
	}
}