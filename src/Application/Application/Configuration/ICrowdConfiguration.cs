namespace Ica.StackIt.Application.Configuration
{
	public interface ICrowdConfiguration
	{
		/// <summary>
		///     The Crowd service endpoint URL.
		/// </summary>
		/// <remarks>
		///     Defaults to https://crowd.example.com:8443/crowd/services/SecurityServer
		/// </remarks>
		string Url { get; }

		/// <summary>
		///     The name of the application used to validate against the Crowd SOAP endpoint.
		/// </summary>
		string ApplicationName { get; }

		/// <summary>
		///     The password to the Crowd API.
		/// </summary>
		string ApiPassword { get; }
	}
}
