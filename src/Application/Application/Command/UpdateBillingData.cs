using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Hangfire;

using Ica.StackIt.Application.Billing;
using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Application.Parser;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using ICSharpCode.SharpZipLib.Zip;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.UnorderedQueueName)]
	public class UpdateBillingData : CommandBase
	{
		internal static class MonthlyCsvUrl
		{
			// https://ica-billing-reports.s3.amazonaws.com/509438855493-aws-billing-detailed-line-items-with-resources-and-tags-2015-03.csv.zip
			private const string _monthlyCsvFormat =
				"s3://{BucketName}/{Account}-aws-billing-detailed-line-items-with-resources-and-tags-{Period}.csv.zip";

			public static string Create(string bucketName, string accountNumber, string period)
			{
				return _monthlyCsvFormat.Inject(new {BucketName = bucketName, Account = accountNumber, Period = period});
			}
		}

		private readonly IRepository<AwsProfile> _awsProfileRepository;
		private readonly IBillingManager _billingManager;
		private readonly IClock _clock;
		private readonly IS3PathParser _pathParser;

		private static readonly object _mutex = new object();

		public UpdateBillingData(
			IAwsClientFactory awsClientFactory,
			IRepository<AwsProfile> awsProfileRepository,
			IBillingManager billingManager,
			IClock clock,
			IS3PathParser pathParser
			) : base(awsProfileRepository, awsClientFactory)
		{
			_awsProfileRepository = awsProfileRepository;
			_billingManager = billingManager;
			_clock = clock;
			_pathParser = pathParser;
		}

		public void LoadDeltas(Guid profileId)
		{
			DateTime utcNow = _clock.UtcNow;
			string period = string.Format("{0:0000}-{1:00}", utcNow.Year, utcNow.Month);
			LoadDeltas(profileId, period);
		}

		public void LoadDeltas(Guid profileId, string period)
		{
			lock (_mutex) SynchronizedLoadDeltas(profileId, period);
		}

		private void SynchronizedLoadDeltas(Guid profileId, string period)
		{
			IAwsClient client;
			AwsProfile profile;

			if (! TryInitialize(profileId, out client, out profile))
			{
				return;
			}

			if (string.IsNullOrEmpty(profile.DetailedBillingS3Bucket))
			{
				// detailed billing not configured for this profile
				return;
			}

			if (! profile.IsBillingHistoryLoaded)
			{
				// historical billing data has not been loaded for this profile yet,
				// so deltas cannot be loaded.
				return;
			}

			RefreshDataForPeriod(profile, period, client, _clock.UtcNow);

			_awsProfileRepository.Update(profile);
		}

		public void LoadAllHistory(Guid profileId)
		{
			lock (_mutex)
			{
				SynchronizedLoadAllHistory(profileId);
			}
		}

		private void SynchronizedLoadAllHistory(Guid profileId)
		{
			IAwsClient client;
			AwsProfile profile;
			if (!TryInitialize(profileId, out client, out profile))
			{
				return;
			}

			if (string.IsNullOrEmpty(profile.DetailedBillingS3Bucket))
			{
				// detailed billing not configured for this profile
				return;
			}

			if (profile.IsBillingHistoryLoading)
			{
				return;
			}

			// Prevent other processes from trying to load at the same time.
			// This still has a race condition that would be eliminated using optimistic concurrency.
			profile.IsBillingHistoryLoading = true;
			_awsProfileRepository.Update(profile);

			// http://s3.amazonaws.com/509438855493-aws-billing-detailed-line-items-with-resources-and-tags-2014-08.csv.zip
			var pattern = new Regex(@"\A\d+-aws-billing-detailed-line-items-with-resources-and-tags-(\d+\-\d+).csv.zip\z");

			var rootPath = new S3PathParts(profile.DetailedBillingS3Bucket, "");
			List<string> availablePeriods = client.StorageService
			                                      .ListFiles(rootPath.ToString())
			                                      .Select(path => _pathParser.Parse(path))
			                                      .Select(parsedPath => pattern.Match(parsedPath.Key))
			                                      .Where(match => match.Success)
			                                      .Select(match => match.Groups[1].Captures[0].Value)
			                                      .OrderBy(period => period, StringComparer.InvariantCulture)
			                                      .ToList();

			// Forget that we have pulled CSVs before...
			profile.BillingMetadata.Clear();
			// ... or loaded them into the ledger.
			_billingManager.WipeAllData();

			DateTime utcNow = _clock.UtcNow;
			foreach (string period in availablePeriods)
			{
				RefreshDataForPeriod(profile, period, client, utcNow);
			}

			profile.IsBillingHistoryLoaded = true;
			profile.IsBillingHistoryLoading = false;
			_awsProfileRepository.Update(profile);
		}

		private void RefreshDataForPeriod(AwsProfile profile, string period, IAwsClient client, DateTime currentTime)
		{
			string s3Url = MonthlyCsvUrl.Create(profile.DetailedBillingS3Bucket, profile.Account, period);

			var lastModified = new DateTime();
			if (profile.BillingMetadata.ContainsKey(period))
			{
				lastModified = profile.BillingMetadata[period].LastModified;
			}

			DateTime newLastModified;
			Stream file = client.StorageService.GetFileIfChangedSince(s3Url, lastModified, out newLastModified);
			if (file == null)
			{
				return;
			}

			using (file)
			{
				using (Stream zipStream = OpenFirstZipEntry(file))
				{
					using (var streamReader = new StreamReader(zipStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 32768, leaveOpen: true))
					{
						using (var parser = new LineItemCsvParser(streamReader))
						{
							_billingManager.LoadLineItems(parser.GetLineItems(), period);
						}
					}
				}
			}

			BillingMetadata metadata;
			if (! profile.BillingMetadata.ContainsKey(period))
			{
				metadata = new BillingMetadata();
				profile.BillingMetadata.Add(period, metadata);
			}
			else
			{
				metadata = profile.BillingMetadata[period];
			}
			metadata.LastModified = newLastModified;
			metadata.LastLoaded = currentTime;
		}

		private static Stream OpenFirstZipEntry(Stream rawStream)
		{
			var zipStream = new ZipInputStream(rawStream);
			try
			{
				zipStream = new ZipInputStream(rawStream);
				zipStream.GetNextEntry();
				return zipStream;
			}
			catch
			{
				zipStream.Dispose();
				throw;
			}
		}
	}
}