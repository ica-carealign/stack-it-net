using System;
using System.Collections.Generic;

using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Interactive.WebPortal
{
	public interface IUserProfileAccessManager
	{
		IEnumerable<AwsProfile> GetProfilesForUser(string username);

		bool UserHasPermissionForProfile(string username, Guid profileId);
	}
}