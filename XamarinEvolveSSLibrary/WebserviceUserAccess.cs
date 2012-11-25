using System;
using ServiceStack.ServiceClient.Web;
using ServiceStack.Common.ServiceClient.Web;

namespace XamarinEvolveSSLibrary
{
	public class WebserviceUserAccess : UserAccess
	{
		private User _currentUser = new User ();
		object _currentUserLock = new object ();
		private ClientWrapper _clientWrapper;

		public WebserviceUserAccess (ClientWrapper clientWrapper)
		{
			_clientWrapper = clientWrapper;
		}

		#region implemented abstract members of UserAccess

		public override User GetCurrentUser ()
		{
			lock (_currentUserLock)
			{
				return _currentUser;
			}
		}

		protected override User CreateNewUser (string username, string password)
		{
			if (string.IsNullOrWhiteSpace (username) || string.IsNullOrWhiteSpace (password))
				throw new UserAuthenticationException (string.Format ("Could not login {0}", username));

			User user = new User()
			{
				UserName = username,
				Password = MD5Helper.CalculateMD5Hash (password),
			};
			
			lock (_clientWrapper.ClientLock)
			{
				UserResponse response = _clientWrapper.Client.Put<XamarinEvolveSSLibrary.UserResponse>("User", user);

				if (response.Exception != null)
					throw new DuplicateUserException (string.Format ("username {0} already exists", username) );
			}

			lock (_currentUserLock)
				_currentUser = user;

			try
			{
				SendLoginAuth (username, password);
			}
			catch (Exception)
			{
				lock (_currentUserLock)
					_currentUser = new User ();
				throw;
			}

			lock (_currentUserLock)
				return _currentUser;
		}

		protected override User UserLogin (string username, string password)
		{
			if (string.IsNullOrWhiteSpace (username) || string.IsNullOrWhiteSpace (password))
				throw new UserAuthenticationException (string.Format ("Could not login {0}", username));
			
			SendLoginAuth (username, password);

			string uri = string.Format ("User/{0}", username);

			UserResponse response;
			lock (_clientWrapper.ClientLock)
			{
				response = _clientWrapper.Client.Get<XamarinEvolveSSLibrary.UserResponse>(uri);
				if (response.Exception != null || response.Users == null || response.Users.Count != 1)
					throw new UserAuthenticationException (string.Format ("Could not login {0}", username));
			}

			lock (_currentUserLock)
				return _currentUser = response.Users[0];
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

			User userToSend;

			lock (_currentUserLock)
			{
				userToSend = new User () {
					UserName = _currentUser.UserName,
					FullName = _currentUser.FullName,
					City = _currentUser.City,
					Company = _currentUser.Company,
					Title = _currentUser.Title,
					Email = _currentUser.Email,
					Phone = _currentUser.Phone,
				};
			}

			Func <int> func = delegate {
				lock (_clientWrapper.ClientLock)
				{
					_clientWrapper.Client.Post<XamarinEvolveSSLibrary.UserResponse>("User", userToSend);
					return 0;
				}
			};
			func.BeginInvoke (null, null);
		}

		public override void Logout ()
		{
			lock (_currentUserLock)
			{
				_currentUser = new User ();
			}

			Func <int> func = delegate {
				SendLogoutAuth ();
				
				return 0;
			};
			func.BeginInvoke (null, null);
		}

		public override void DeleteUser ()
		{
			User userToDelete;

			lock (_currentUserLock)
			{
				userToDelete = _currentUser;
			}
			
			_currentUser = new User ();

			Func <int> func = delegate {
				lock (_clientWrapper.ClientLock)
				{
					string uri = string.Format ("User/{0}", userToDelete.UserName);
					_clientWrapper.Client.Delete<XamarinEvolveSSLibrary.UserResponse>(uri);
				}

				SendLogoutAuth ();

				return 0;
			};
			func.BeginInvoke (null, null);
		}

		#endregion

		void SendLoginAuth (string username, string password)
		{
			lock (_clientWrapper.ClientLock) {
				Auth auth = new Auth () {
					UserName = username,
					Password = MD5Helper.CalculateMD5Hash (password),
					RememberMe = false,
					provider = "credentials"
				};
				try {
					_clientWrapper.Client.Post<AuthResponse> ("Auth", auth);
				}
				catch (Exception) {
					throw new UserAuthenticationException (string.Format ("Could not login {0}", username));
				}
			}
		}

		void SendLogoutAuth ()
		{
			lock (_clientWrapper.ClientLock) {
				Auth auth = new Auth () {
					provider = "logout"
				};
				try {
					_clientWrapper.Client.Post<AuthResponse> ("Auth", auth);
				}
				catch (Exception) {
					throw new UserAuthenticationException (string.Format ("Could not logout"));
				}
			}
		}
	}
}

