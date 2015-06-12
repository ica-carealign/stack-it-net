using System;
using System.Linq;

namespace Ica.StackIt.Application.Parser
{
	public class S3PathParser : IS3PathParser
	{
		private const string _uriStart = "s3://";

		public S3PathParts Parse(string path)
		{
			var uri = new Uri(path);

			if(path.StartsWith(_uriStart, StringComparison.InvariantCultureIgnoreCase))
			{
				return new S3PathParts(uri.Host, uri.LocalPath.TrimStart('/'));
			}

			var segments = uri.Segments.Where(x => x != "/").ToList();
			var key = string.Join("", segments.Skip(1).ToArray());

			return new S3PathParts(segments[0].Trim('/'), key);
		}
	}
}