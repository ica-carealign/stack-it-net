namespace Ica.StackIt.Application.Configuration
{
	public interface IPuppetConfiguration
	{
		/// <summary>
		/// The Puppet bootstrapping script URL.
		/// </summary>
		string BootstrapperUrl { get; }

		/// <summary>
		/// The Puppet Windows installer URL.
		/// </summary>
		string PuppetInstallerUrl { get; }

		/// <summary>
		/// The Puppet Master host.
		/// </summary>
		string PuppetHost { get; }

		/// <summary>
		/// The host where the Puppet clean up web service lives.
		/// </summary>
		/// <remarks>This *should* be the internal IP address of the puppet master,
		/// but it doesn't have to be for development purposes.</remarks>
		string PuppetCleanupHost { get; }

		/// <summary>
		/// The port on the PuppetHost where the node action service listens
		/// </summary>
		int NodeServicePort { get; }
	}
}