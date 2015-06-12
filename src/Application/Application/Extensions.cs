using System;
using System.Collections.Generic;
using System.Linq;

using Amazon.EC2;

using Ica.StackIt.Core;

namespace Ica.StackIt.Application
{
	public static class Extensions
	{
		public static List<T> AsList<T>(this IEnumerable<T> listLike)
		{
			return listLike as List<T> ?? listLike.ToList();
		}

		public static TEntity FindByResourceId<TEntity>(this IRepository<TEntity> self, string resourceId)
			where TEntity : ICloudResource
		{
			return self.FindAll().FirstOrDefault(x => x.ResourceId == resourceId);
		}

		public static string RemoveAllWhitespace(this string self)
		{
			return new string(self.Where(character => !char.IsWhiteSpace(character)).ToArray());
		}

		public static string RemoveNonAlphaNumericCharacters(this string self)
		{
			return new string(self.Where(char.IsLetterOrDigit).ToArray());
		}

		public static string ToNonNull(this PlatformValues self)
		{
			string platformString = self;
			return self == null ? "linux" : platformString;
		}

		public static DateTime ToCentralStandardTime(this DateTime self)
		{
			var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
			return TimeZoneInfo.ConvertTime(self, centralTimeZone);
		}
	}
}