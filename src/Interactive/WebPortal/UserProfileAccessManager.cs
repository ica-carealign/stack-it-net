using System;
using System.Collections.Generic;
using System.Linq;

using Ica.StackIt.AspNet.Identity.Crowd;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Microsoft.Ajax.Utilities;

namespace Ica.StackIt.Interactive.WebPortal
{
	public class UserProfileAccessManager : IUserProfileAccessManager
	{
		private readonly IAuthenticatedUserClient _authClient;
		private readonly IRepository<AwsProfile> _profileRepository;

		public UserProfileAccessManager(
			IAuthenticatedUserClient authClient,
			IRepository<AwsProfile> profileRepository)
		{
			_authClient = authClient;
			_profileRepository = profileRepository;
		}

		public IEnumerable<AwsProfile> GetProfilesForUser(string username)
		{
			IdentityUser user = _authClient.GetUserByName(username);
			IEnumerable<string> currentUsersRoles = _authClient.GetRolesByUser(user);

			List<AwsProfile> profiles = _profileRepository.FindAll().ToList();

			List<AwsProfile> userProfiles = currentUsersRoles
				.SelectMany(role => profiles.FindAll(x => x.Groups.Contains(role)))
				.DistinctBy(x => x.Name)
				.ToList();

			return userProfiles;
		}

		public bool UserHasPermissionForProfile(string username, Guid profileId)
		{
			bool userHasPermissionForProfile = GetProfilesForUser(username)
				.Any(x => x.Id == profileId);

			return userHasPermissionForProfile;
		}
	}
}