using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Amazon.EC2;

using Hangfire;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using LukeSkywalker.IPNetwork;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.OrderedQueueName)]
	public class CreateIpRange
	{
		private readonly IRepository<IPRange> _ipRangeRepository;
		private readonly IRepository<AwsProfile> _profileRepository;
		private readonly IAwsClientFactory _awsClientFactory;

		public CreateIpRange(IRepository<IPRange> ipRangeRepository, IRepository<AwsProfile> profileRepository, IAwsClientFactory awsClientFactory)
		{
			_ipRangeRepository = ipRangeRepository;
			_profileRepository = profileRepository;
			_awsClientFactory = awsClientFactory;
		}

		[DisableConcurrentExecution(60)]
		public void Execute(Guid profileId, string subnetId)
		{
			AwsProfile profile = _profileRepository.Find(profileId);
			if (profile == null)
			{
				return;
			}

			INetworkService networkService = _awsClientFactory.GetClient(profile).NetworkService;
			string cidrRange;
			try
			{
				cidrRange = networkService.GetCidrBySubnetId(subnetId);
			}
			catch (AmazonEC2Exception e)
			{
				Debug.WriteLine(e.Message);
				return;
			}

			if (_ipRangeRepository.FindAll().Any(x => x.Cidr == cidrRange))
			{
				return;
			}

			DateTime utcNow = DateTime.UtcNow;
			IPNetwork network = IPNetwork.Parse(cidrRange);

			List<SubnetIpAddress> subnetIpAddresses = IPNetwork.ListIPAddress(network)
			                                                   .Skip(5) // Amazon reserves the first four IP addresses... (x.x.x.1 - x.x.x.4)
			                                                   .Select(x => new SubnetIpAddress {Address = x, IsInUse = false, LastUpdateTime = utcNow}).ToList();

			if (subnetIpAddresses.Any())
			{
				subnetIpAddresses.RemoveAt(subnetIpAddresses.Count - 1); // and last IP address.
			}

			var ipRange = new IPRange
			{
				AwsProfileId = profileId,
				Cidr = cidrRange,
				Addresses = subnetIpAddresses.ToDictionary(x => x.Address.ToString())
			};

			_ipRangeRepository.Add(ipRange);
		}
	}
}