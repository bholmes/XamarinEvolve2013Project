using System;
using ServiceStack.ServiceClient.Web;

namespace XamarinEvolveSSLibrary
{
	public class WebserviceUserAccess : UserAccess
	{
		private User _currentUser = new User ();
		private ClientWrapper _clientWrapper;

		public WebserviceUserAccess (ClientWrapper clientWrapper)
		{
			_clientWrapper = clientWrapper;
		}

		#region implemented abstract members of UserAccess

		public override User GetCurrentUser ()
		{
			lock (_clientWrapper.ClientLock)
			{
				return _currentUser;
			}
		}

		protected override User CreateNewUser (string username, string password)
		{
			User user = new User()
			{
				UserName = username,
			};
			
			lock (_clientWrapper.ClientLock)
			{
				UserResponse response = _clientWrapper.Client.Put<XamarinEvolveSSLibrary.UserResponse>("User", user);

				if (response.Exception != null)
					throw new DuplicateUserException (string.Format ("username {0} already exists", username) );

				return _currentUser = user;
			}
		}

		protected override User UserLogin (string username, string password)
		{
			string uri = string.Format ("User/{0}", username);

			lock (_clientWrapper.ClientLock)
			{
				UserResponse response = _clientWrapper.Client.Get<XamarinEvolveSSLibrary.UserResponse>(uri);
				if (response.Exception != null || response.Users == null || response.Users.Count != 1)
					throw new UserAuthenticationException (string.Format ("Could not login {0}", username));
			
				return _currentUser = response.Users[0];
			}
		}

		protected override UserList GetUsers ()
		{
			UserResponse response;

			lock (_clientWrapper.ClientLock)
			{
				response = _clientWrapper.Client.Get<XamarinEvolveSSLibrary.UserResponse>("User");
			}

			if (response.Users == null || response.Exception != null)
				return new UserList ();

			return new UserList (response.Users);
		}

		public override void CommitCurrentUserChanges ()
		{
			// Make copy to ensure password and avatar are null

			User userToSend = new User () {
				UserName = _currentUser.UserName,
				FullName = _currentUser.FullName,
				City = _currentUser.City,
				Company = _currentUser.Company,
				Title = _currentUser.Title,
				Email = _currentUser.Email,
				Phone = _currentUser.Phone,
			};

			Func <int> func = delegate {
				lock (_clientWrapper.ClientLock)
				{
					_clientWrapper.Client.Post<XamarinEvolveSSLibrary.UserResponse>("User", userToSend);
					return 0;
				}
			};
			func.BeginInvoke (null, null);
		}

		#endregion
	}
}

