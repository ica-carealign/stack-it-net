namespace Ica.StackIt.Application.Parser
{
	public interface IS3PathParser
	{
		S3PathParts Parse(string path);
	}
}