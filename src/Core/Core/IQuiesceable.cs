namespace Ica.StackIt.Core
{
	public interface IQuiesceable
	{
		/// <summary>
		///     Prepares the object for being stored.
		/// </summary>
		void Quiesce();
	}
}