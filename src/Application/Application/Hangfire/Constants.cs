namespace Ica.StackIt.Application.Hangfire
{
	public static class Constants
	{
		// This must be all lowercase because the Hangfire Queue attribute requires it to be
		public const string UnorderedQueueName = "unordered_commands";
		public const string OrderedQueueName = "ordered_commands";
	}
}