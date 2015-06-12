namespace Ica.StackIt.Core.Entities
{
	public class RoleOptions
	{
		public string InstanceType { get; set; }

		/// <summary>
		/// Disk type (magnetic or SSD)
		/// </summary>
		public string VolumeType { get; set; }

		/// <summary>
		///     Volume size in gigabytes
		/// </summary>
		public int VolumeSize { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Schedule { get; set; }
	}
}