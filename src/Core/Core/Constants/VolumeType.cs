using System.Collections.Generic;

namespace Ica.StackIt.Core.Constants
{
	public static class VolumeType
	{
		public static string Magnetic = "Magnetic";
		public static string Ssd = "SSD";

		public static IEnumerable<string> GetAll()
		{
			yield return Magnetic;
			yield return Ssd;
		}
	}
}