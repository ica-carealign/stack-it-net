using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Interactive.WebPortal.Models.Product
{
	public class ProductSummaryViewModel
	{
		public Guid ProductId { get; set; }

		[Display(Name = "Product Name")]
		public string ProductName { get; set; }

		[Display(Name = "Versions")]
		public IEnumerable<string> ProductVersions { get; set; }
	}
}