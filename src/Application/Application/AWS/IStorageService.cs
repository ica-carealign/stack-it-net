using System;
using System.Collections.Generic;
using System.IO;

namespace Ica.StackIt.Application.AWS
{
	public interface IStorageService
	{
		string UploadFile(string bucket, string key, byte[] contents);

		string GetFile(string path);

		IList<string> ListFiles(string path);

		Stream GetFileIfChangedSince(string s3Url, DateTime lastModified, out DateTime newLastModified);

		void CreateExpirationRule(string bucket, string prefix, int expirationDays, string description);
	}
}