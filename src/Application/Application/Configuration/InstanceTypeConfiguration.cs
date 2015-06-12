namespace Ica.StackIt.Application.Configuration
{
	public class InstanceTypeConfiguration : IInstanceTypeConfiguration
	{
		/// <summary>
		///     The instance type used by AWS. (t2.micro, t2.small, etc.)
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     A short description of the instance type. ("1 vCPU, 2 GiB RAM")
		/// </summary>
		public string Description { get; set; }
	}
}