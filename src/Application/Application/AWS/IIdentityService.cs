using Amazon.IdentityManagement.Model;

namespace Ica.StackIt.Application.AWS
{
	public interface IIdentityService
	{
		User GetCurrentUser();

		string GetCurrentAccount();

		bool ValidateProfileOwnership(string account);
	}
}