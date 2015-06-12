namespace Ica.StackIt.Application.Parser
{
	public class S3PathParts
	{
		private readonly string _bucket;
		private readonly string _key;

		public S3PathParts(string bucket, string key)
		{
			_bucket = bucket;
			_key = key;
		}

		public string Bucket
		{
			get { return _bucket; }
		}

		public string Key
		{
			get { return _key; }
		}

		public override string ToString()
		{
			return string.Format("s3://{0}/{1}", Bucket, Key);
		}
	}
}