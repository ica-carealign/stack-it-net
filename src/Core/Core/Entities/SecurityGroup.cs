using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Core.Entities
{
	public class SecurityGroup : ICloudResource
	{
		[Required]
		public string Name { get; set; }

		[Required]
		public string ResourceId { get; set; }

		public string ResourceType
		{
			get { return "SecurityGroup"; }
		}
	}
}