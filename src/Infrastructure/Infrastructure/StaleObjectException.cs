using System;

namespace Ica.StackIt.Infrastructure
{
	/// <summary>
	///     The exception that is thrown when a version number check failed, indicating the object being saved has been saved
	///     by a different party since it was retrieved from the data store.
	/// </summary>
	[Serializable]
	public class StaleObjectException : Exception
	{
		public StaleObjectException(string message) : base(message) {}
	}
}