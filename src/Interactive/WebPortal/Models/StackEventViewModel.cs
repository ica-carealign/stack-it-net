using System.Collections.Generic;

using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Interactive.WebPortal.Models
{
	public class StackEventViewModel
	{
		public string StackName { get; set; }
		public List<StackEvent> StackEvents { get; set; }
	}
}