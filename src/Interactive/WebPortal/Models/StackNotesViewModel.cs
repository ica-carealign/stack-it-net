using System;

namespace Ica.StackIt.Interactive.WebPortal.Models
{
	public class StackNotesViewModel
	{
		public Guid StackId { get; set; }
		public Guid OwnerProfileId { get; set; }
		public string OwnerUserName { get; set; }
		public string Notes { get; set; }

		public string Error { get; set; }
	}
}