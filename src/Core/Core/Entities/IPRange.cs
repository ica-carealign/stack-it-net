using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ica.StackIt.Core.Entities
{
	public class IPRange : IEntity<Guid>
	{
		public Guid Id { get; set; }

		[Required]
		public Guid AwsProfileId { get; set; }

		[Required]
		public string Cidr { get; set; }

		public IDictionary<string, SubnetIpAddress> Addresses { get; set; }
	}
}