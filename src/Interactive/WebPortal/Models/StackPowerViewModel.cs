using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.StackIt.Interactive.WebPortal.Models
{
	public class StackPowerViewModel
	{
		public string StackName { get; set; }
		public Guid StackRecordId { get; set; }
		public string StackPowerState { get; set; }
	}
}