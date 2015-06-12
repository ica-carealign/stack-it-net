using System;
using System.Linq;
using System.Net;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using LukeSkywalker.IPNetwork;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.UnorderedQueueName)]
	public class RefreshIpRanges : CommandBase
	{
		private readonly IRepository<IPRange> _ipRangeRepository;

		public RefreshIpRanges(IRepository<AwsProfile> profileRepository, IRepository<IPRange> ipRangeRepository, IAwsClientFactory awsClientFactory)
			: base(profileRepository, awsClientFactory)
		{
			_ipRangeRepository = ipRangeRepository;
		}

		[DisableConcurrentExecution(60)]
		public void Execute(Guid profileId)
		{
			IAwsClient awsClient;
			if (!TryInitializeClient(profileId, out awsClient))
			{
				return;
			}
			var networkService = awsClient.NetworkService;

			var ipRanges = _ipRangeRepository.FindAll().Where(x => x.AwsProfileId == profileId).ToList();
			var allocatedIps = networkService.GetAllocatedIpAddresses().ToList();
			var now = DateTime.UtcNow;

			foreach (var range in ipRanges)
			{
				var ipRangeNeedsUpdate = false;
				var cidr = range.Cidr;
				var network = IPNetwork.Parse(cidr);

				foreach (var address in range.Addresses)
				{
					var addressAlreadyAllocated = false;
					foreach (var allocated in allocatedIps)
					{
						// If the allocated address isn't the particular address we care about in this iteration
						// or the allocated network address isn't in the ip range,
						// skip it
						if (address.Key != allocated || !IPNetwork.Contains(network, IPAddress.Parse(allocated)) )
						{
							continue;
						}

						addressAlreadyAllocated = true;

						// The addresses is allocated in AWS but isn't marked as in use in the database
						if (!address.Value.IsInUse)
						{
							address.Value.IsInUse = true;
							ipRangeNeedsUpdate = true;
						}
					}

					// The address is not allocated in but is marked as in use in the database
					if (!addressAlreadyAllocated && address.Value.IsInUse)
					{
						address.Value.IsInUse = false;
						ipRangeNeedsUpdate = true;
					}

					// By this point, if ipRangeNeedsUpdate is false, that means that the current address in the iteration either
					// 1. Is allocated in AWS and also already marked as such in the database OR
					// 2. Is not allocated in AWS and is already marked as such in the database

					if (ipRangeNeedsUpdate)
					{
						address.Value.LastUpdateTime = now;
					}
				}

				if (ipRangeNeedsUpdate)
				{
					_ipRangeRepository.Update(range);
				}
			}
		}
	}
}