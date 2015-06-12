using System.Collections.Generic;

using Amazon.Route53.Model;

namespace Ica.StackIt.Application.AWS
{
	public interface IDnsService
	{
		HostedZone GetHostedZoneByName(string name);

		IEnumerable<ResourceRecordSet> GetResourceRecordSets(HostedZone zone);
	}
}