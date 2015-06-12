namespace Ica.StackIt.Application.Parser
{
	public interface IArnParser
	{
		string GetAccountNumber(string iamArn);
	}
}