using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;

using Ica.StackIt.Application.Parser;

namespace Ica.StackIt.Application.AWS
{
	public class IdentityService : IIdentityService
	{
		private readonly IAmazonIdentityManagementService _iamClient;
		private readonly IArnParser _arnParser;

		public IdentityService(IAmazonIdentityManagementService iamClient, IArnParser arnParser)
		{
			_iamClient = iamClient;
			_arnParser = arnParser;
		}

		/// <summary>
		///     Get the user associated with the current profile.
		/// </summary>
		/// <returns></returns>
		public User GetCurrentUser()
		{
			var request = new GetUserRequest();
			GetUserResponse response = _iamClient.GetUser(request);

			return response.User;
		}

		/// <summary>
		///     Get the account associated with the current profile.
		/// </summary>
		/// <returns></returns>
		public string GetCurrentAccount()
		{
			User user = GetCurrentUser();
			return _arnParser.GetAccountNumber(user.Arn);
		}

		/// <summary>
		/// Ensure that an account exists in AWS.
		/// </summary>
		/// <param name="account"></param>
		/// <returns></returns>
		public bool ValidateProfileOwnership(string account)
		{
			var currentAccount = GetCurrentAccount();
			return currentAccount == account;
		}
	}
}