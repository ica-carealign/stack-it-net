using System.Collections.Generic;
using System.Linq;

using Amazon.Route53;
using Amazon.Route53.Model;

namespace Ica.StackIt.Application.AWS
{
	public class DnsService : IDnsService
	{
		private readonly IAmazonRoute53 _route53Client;

		public DnsService(IAmazonRoute53 route53Client)
		{
			_route53Client = route53Client;
		}

		public HostedZone GetHostedZoneByName(string name)
		{
			IEnumerable<HostedZone> zones = _route53Client.ListHostedZones().HostedZones.Where(x => x.Name.Contains(name));
			return zones.SingleOrDefault();
		}

		public IEnumerable<ResourceRecordSet> GetResourceRecordSets(HostedZone zone)
		{
			var request = new ListResourceRecordSetsRequest(zone.Id);
			ListResourceRecordSetsResponse response = _route53Client.ListResourceRecordSets(request);
			return response.ResourceRecordSets;
		}
	}
}