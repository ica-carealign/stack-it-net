using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Ica.StackIt.Core.Entities;
using System.ComponentModel;

namespace Ica.StackIt.Interactive.WebPortal.Models
{
	public class SessionViewModel
	{
		public List<AwsProfile> Profiles { get; set; }

		[DisplayName("Active AWS Profile")]
		public Guid SelectedProfileId { get; set; }
	}
}