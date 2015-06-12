using System;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	public class AuthenticatedUserClientException : ApplicationException
	{
		public AuthenticatedUserClientException() {}

		public AuthenticatedUserClientException(string message)
			: base(message) {}

		public AuthenticatedUserClientException(string message, Exception inner)
			: base(message, inner) {}
	}
}