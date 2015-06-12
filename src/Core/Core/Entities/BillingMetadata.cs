using System;

namespace Ica.StackIt.Core.Entities
{
	public class BillingMetadata
	{
		/// <summary>
		///     The date/time the billing data was last loaded from S3.
		/// </summary>
		public DateTime LastLoaded;

		/// <summary>
		///     The modification date/time of the S3 object that contains the billing details
		///     as of the <see cref="LastLoaded" /> time.
		/// </summary>
		public DateTime LastModified;
	}
}