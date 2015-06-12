using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using Amazon.S3;
using Amazon.S3.Model;

using Ica.StackIt.Application.Parser;

namespace Ica.StackIt.Application.AWS
{
	public class StorageService : IStorageService
	{
		private readonly IAmazonS3 _s3Client;
		private readonly IS3PathParser _pathParser;

		private const string _httpTemplate = "http://s3.amazonaws.com/{0}/{1}";

		public StorageService(IAmazonS3 s3Client, IS3PathParser pathParser)
		{
			_s3Client = s3Client;
			_pathParser = pathParser;
		}

		public string UploadFile(string bucket, string key, byte[] contents)
		{
			var request = new PutObjectRequest
			{
				BucketName = bucket,
				Key = key,
				ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
			};

			using (var stream = new MemoryStream(contents))
			{
				request.InputStream = stream;
				_s3Client.PutObject(request);
			}

			return string.Format(_httpTemplate, bucket, key);
		}

		public string GetFile(string path)
		{
			S3PathParts parts = _pathParser.Parse(path);
			return GetFile(parts.Bucket, parts.Key);
		}

		public IList<string> ListFiles(string path)
		{
			S3PathParts parts = _pathParser.Parse(path);
			return _s3Client.ListObjects((new ListObjectsRequest
			{
				BucketName = parts.Bucket,
				Delimiter = "/",
				Prefix = parts.Key
			})).S3Objects.Select(o => new S3PathParts(parts.Bucket, o.Key).ToString()).ToList();
		}

		public Stream GetFileIfChangedSince(string s3Url, DateTime lastModified, out DateTime newLastModified)
		{
			S3PathParts path = _pathParser.Parse(s3Url);
			var request = new GetObjectRequest
			{
				BucketName = path.Bucket,
				Key = path.Key,
				ModifiedSinceDate = lastModified
			};

			try
			{
				// Do NOT dispose of the response here because it will dispose of the response stream also
				// (and that is ALL it does). It's a little gross, but I'll accept it because the alternative
				// is to return a custom Stream that will dispose the response when the Stream itself is
				// disposed, which is grosser.
				GetObjectResponse response = _s3Client.GetObject(request);
				newLastModified = response.LastModified;
				return response.ResponseStream;
			}
			catch (AmazonS3Exception e)
			{
				if (e.StatusCode == HttpStatusCode.NotModified)
				{
					newLastModified = default(DateTime);
					return null;
				}
				throw;
			}
		}

		public void CreateExpirationRule(string bucket, string prefix, int expirationDays, string description)
		{
			var rule = new LifecycleRule
			{
				Id = description,
				Prefix = prefix,
				Status = LifecycleRuleStatus.Enabled,
				Expiration = new LifecycleRuleExpiration {Days = expirationDays}
			};

			var lifecycleConfiguration = new LifecycleConfiguration
			{
				Rules = new List<LifecycleRule> {rule}
			};

			_s3Client.PutLifecycleConfiguration(bucket, lifecycleConfiguration);
		}

		private string GetFile(string bucket, string key)
		{
			using (GetObjectResponse getObjectResponse = _s3Client.GetObject(bucket, key))
			{
				using (Stream responseStream = getObjectResponse.ResponseStream)
				{
					using (var reader = new StreamReader(responseStream))
					{
						string contents = reader.ReadToEnd();
						return contents;
					}
				}
			}
		}
	}
}