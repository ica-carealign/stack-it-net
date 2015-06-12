using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.UnorderedQueueName)]
	public class CleanUpPuppet : ICleanUpPuppet
	{
		private readonly HttpClient _httpClient;

		private const string _uri = "cgi-bin/node.cgi";
		private const string _hostNameTemplate = "ip-{privateIp}.ec2.internal";

		public CleanUpPuppet(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public void CleanUp(Instance instance)
		{
			string privateIp = instance.PrivateAddresses.FirstOrDefault();
			if (privateIp == null)
			{
				return;
			}

			privateIp = privateIp.Replace('.', '-');

			string hostNameToDelete = _hostNameTemplate.Inject(new { privateIp });
			var queryString = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("action", "delete"),
				new KeyValuePair<string, string>("node", hostNameToDelete)
			});
			Task<HttpResponseMessage> response = _httpClient.PostAsync(_uri, queryString);
			response.Result.EnsureSuccessStatusCode();
		}
	}
}