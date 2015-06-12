namespace Ica.StackIt.Application.Configuration
{
	public interface IQueueingConfiguration
	{
		string UnorderedCommandQueue { get; }
		string OrderedCommandQueue { get; }
	}
}