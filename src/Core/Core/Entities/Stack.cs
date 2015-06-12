using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Core.Entities
{
	public class Stack : IEntity<Guid>, ICloudResource
	{
		public Stack()
		{
			StackEvents = new List<StackEvent>();
			ResourceRecords = new List<ResourceRecord>();
			InstanceIds = new List<Guid>();
		}

		public Guid Id { get; set; }

		[Required]
		public string Name { get; set; }
		public string Description { get; set; }


		public string OwnerUserName { get; set; }

		[Required]
		public Guid OwnerProfileId { get; set; }

		public DateTime CreateTime { get; set; }
		public string Status { get; set; }

		public IEnumerable<ICloudResource> Resources
		{
			get { return ResourceRecords; }
		}

		public ICollection<StackEvent> StackEvents { get; set; }

		public ICollection<ResourceRecord> ResourceRecords { get; set; }

		/// <summary>
		///     The instances associated with this stack. These are ICA ids, not resource ids.
		/// </summary>
		public ICollection<Guid> InstanceIds { get; set; }

		public string ResourceId { get; set; }

		public string ResourceType
		{
			get { return "Stack"; }
		}

		[Required]
		public bool NeedsRefreshing { get; set; }

		public string Notes { get; set; }

		[Required]
		public bool CreatedByApplication { get; set; }

		/// <summary>
		/// The id of the schedule that controls when this stack automatically powers up and powers down.
		/// </summary>
		public Guid ScheduleId { get; set; }

		[Required]
		public bool ScheduleEnabled { get; set; }
	}
}