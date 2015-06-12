namespace Ica.StackIt.Application.Configuration
{
	public interface IInstanceTypeConfiguration
	{
		/// <summary>
		///     The instance type used by AWS. (t2.micro, t2.small, etc.)
		/// </summary>
		string Name { get; set; }

		/// <summary>
		///     A short description of the instance type. ("1 vCPU, 2 GiB RAM")
		/// </summary>
		string Description { get; set; }
	}
}