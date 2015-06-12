namespace Ica.StackIt.Application.Configuration
{
	public interface ICloudOptions
	{
		string MasterKeyAlias { get; }

		string ConfigurationTemplateBucket { get; }
	}
}