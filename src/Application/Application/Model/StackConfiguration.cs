using System;
using System.Collections.Generic;

using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.Model
{
	public class StackConfiguration
	{
		public string StackName { get; set; }
		public string StackDescription { get; set; }

		public string OwnerUserName { get; set; }
		public Guid OwnerProfileId { get; set; }
		public string DefaultSecurityGroupId { get; set; }

		public string VpcId { get; set; }
		public string SubnetId { get; set; }

		public string HostedZone { get; set; }
		public string BootstrapperUrl { get; set; }
		public string PuppetInstallerUrl { get; set; }
		public string PuppetHost { get; set; }

		public string Notes { get; set; }

		public Guid ScheduleId { get; set; }
		public bool ScheduleEnabled { get; set; }

		/// <summary>
		///     The list of instances to be used to generate a Cloud Formation template.
		/// </summary>
		public IList<Instance> Instances = new List<Instance>();

		/// <summary>
		///     The base images associated with each instance.
		/// </summary>
		/// <remarks>
		///     These must be in the same order and have the same count as Instances
		///     such that zipping Instances to BaseImages produces a valid set
		/// </remarks>
		public IList<BaseImage> BaseImages = new List<BaseImage>();
	}
}