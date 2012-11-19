using System;
using System.Collections.Generic;

namespace XamarinEvolveIOS
{
	public class Engine
	{
		public class Debug
		{
			static public void SimulateNetworkWait ()
			{
				System.Threading.Thread.Sleep (3000);
			}
		}

		static Engine _instance = new Engine ();
		public static Engine Instance
		{
			get 
			{
				return _instance;
			}
		}

		public Engine ()
		{
		}

		private UserList _userListForTesting;
		private User _currentUser = new User ();

		public User GetCurrentUser ()
		{
			return _currentUser;
		}

		public class UserLoginResult
		{
			public User User {get; set;}
			public Exception Exceptin {get;set;}
		}

		void AsyncUserLoginCall (Func<User> call, Action<UserLoginResult> onComplete)
		{
			Func <int> func = delegate {
				try
				{
					User newUser = call ();
					
					if (onComplete != null)
					{
						onComplete (new UserLoginResult (){
							User = newUser
						});
					}
				}
				catch (Exception exp)
				{
					if (onComplete != null)
					{
						onComplete (new UserLoginResult (){
							Exceptin = exp
						});
					}
				}
				
				return 0;
			};
			func.BeginInvoke (null, null);
		}

		public User CreateNewUser (string username, string password)
		{
			Engine.Debug.SimulateNetworkWait ();

			UserList list = GetUsers ();
			
			User ret = list[username];
			
			if (ret != null)
				throw new DuplicateUserException (string.Format ("username {0} already exists", username) );
			
			return _currentUser = _userListForTesting.Add (new User (){
				UserName = username,
			});
		}

		public void CreateNewUser (string username, string password, Action<UserLoginResult> onComplete)
		{
			AsyncUserLoginCall (delegate {
				return CreateNewUser (username, password);
			}, onComplete);
		}

		public User UserLogin (string username, string password)
		{
			Engine.Debug.SimulateNetworkWait ();

			UserList list = GetUsers ();
			
			User ret = list[username];
			if (ret == null)
				throw new UserAuthenticationException (string.Format ("Could not login {0}", username));

			return _currentUser = ret;
		}

		public void UserLogin (string username, string password, Action<UserLoginResult> onComplete)
		{
			AsyncUserLoginCall (delegate {
				return UserLogin (username, password);
			}, onComplete);
		}

		public UserList GetUsers ()
		{
			if (_userListForTesting == null)
			{
				List<User> users = new List<User> ();
				users.Add (new User () {
					UserName="billholmes",
					FullName="William Holmes",
					City = "Pittsburgh, PA",
					Company = "moBill Holmes",
					Title = "Owner",
					Phone = "(412)-555-5555", 
					EMail = "bill@mobillholmes.com"
				});
				users.Add (new User () {
					UserName="natfriedman",
					FullName="Nat Friedman",
					City="San Francisco, CA",
					Company="Xamarin",
					Title="CEO",
					Phone="(855)-926-2746",
					EMail="nat@xamarin.com"
				});
				users.Add (new User () {
					UserName="migueldeicaza",
					FullName="Miguel de Icaza",
					City="Boston, MA",
					Company="Xamarin",
					Title="CTO",
					Phone="(855)-926-2746",
					EMail="miguel@xamarin.com"
				});
				users.Add (new User () {
					UserName="josephhill",
					FullName="Joseph Hill",
					City="Boston, MA",
					Company="Xamarin",
					Title="COO",
					Phone="(855)-926-2746",
					EMail="joseph@xamarin.com"
				});

				_userListForTesting = new UserList (users);
			}

			return _userListForTesting;
		}

		public class PostNewAvatarResult
		{
			public string URL{get;set;}
			public Exception Exceptin{get;set;}
		}

		public string PostNewAvatar (byte [] data)
		{
			Engine.Debug.SimulateNetworkWait ();

			User currentUser = GetCurrentUser ();
			string fullName = currentUser.AvatarURL;

			if (string.IsNullOrEmpty (fullName))
			{
				string path = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
				string filename = System.IO.Path.GetRandomFileName ();

				fullName = System.IO.Path.Combine (path, filename + ".png");
			}
			else
			{
				if (fullName.StartsWith ("file:///"))
					fullName = fullName.Remove (0, "file://".Length);
			}

			System.IO.FileStream fileStream = null;
			try
			{
				fileStream = new System.IO.FileStream(
					fullName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
				fileStream.Write(data, 0, data.Length);
			}
			finally
			{
				if (fileStream != null)
					fileStream.Close();
			}

			currentUser.AvatarURL = fullName = string.Format ("file://{0}", fullName);

			return fullName;
		}

		public void PostNewAvatar (byte [] data, Action<PostNewAvatarResult> onComplete)
		{
			Func <int> func = delegate {
				try
				{
					string newURL = PostNewAvatar (data);
					
					if (onComplete != null)
					{
						onComplete (new PostNewAvatarResult (){
							URL = newURL,
						});
					}
				}
				catch (Exception exp)
				{
					if (onComplete != null)
					{
						onComplete (new PostNewAvatarResult (){
							Exceptin = exp,
						});
					}
				}
				
				return 0;
			};
			func.BeginInvoke (null, null);
		}
	}

	public class User
	{
		public string UserName {get; set;}
		public string FullName {get; set;}
		public string EMail {get; set;}
		public string City {get;set;}
		public string Company {get; set;}
		public string Title {get; set;}
		public string Phone {get;set;}
		public string AvatarURL {get;set;}

		public bool IsLocalUser 
		{
			get
			{
				User localUser = Engine.Instance.GetCurrentUser ();

				if (string.IsNullOrEmpty (localUser.UserName) && string.IsNullOrEmpty (UserName))
					return true;

				return localUser.UserName.Equals (UserName);
			}
		}

		public bool IsAnonymousUser 
		{
			get
			{
				return string.IsNullOrEmpty (UserName);
			}
		}
	}

	public class UserList
	{
		List<User> _list;

		public UserList (List<User> list)
		{
			_list = list;
		}

		public User this[int index]
		{
			get {
				return _list[index];
			}
		}

		public User this[string username]
		{
			get {
				User ret = _list.Find (e=>e.UserName == username);

				return ret;
			}
		}

		public User Add (User newUser)
		{
			_list.Add (newUser);
			return newUser;
		}

		public int Count {get{return _list.Count;}}
	}

	public class UserAuthenticationException : System.Exception
	{
		public UserAuthenticationException (string message) : base (message)
		{

		}
	}

	public class DuplicateUserException : System.Exception
	{
		public DuplicateUserException (string message) : base (message)
		{
			
		}
	}

}

