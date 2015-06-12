using System;
using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Core.Entities
{
	public class Schedule : IEntity<Guid>
	{
		[Required]
		public Guid Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string StartCron { get; set; }

		[Required]
		public string StopCron { get; set; }

		[Required]
		public bool StartOnWeekends { get; set; }

		[Required]
		public bool GlobalDefault { get; set; }
	}
}