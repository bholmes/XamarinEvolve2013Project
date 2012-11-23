using System;
using System.Collections.Generic;

namespace XamarinEvolveSSLibrary
{
	public class UserAccessLocalTest : UserAccess
	{
		private UserList _userListForTesting;
		private User _currentUser = new User ();
		
		override public User GetCurrentUser ()
		{
			return _currentUser;
		}
		
		override public User CreateNewUser (string username, string password)
		{
			Debug.SimulateNetworkWait ();
			
			UserList list = CachedUserList;
			
			User ret = list[username];
			
			if (ret != null)
				throw new DuplicateUserException (string.Format ("username {0} already exists", username) );
			
			return _currentUser = _userListForTesting.Add (new User (){
				UserName = username,
			});
		}
		
		override public User UserLogin (string username, string password)
		{
			Debug.SimulateNetworkWait ();
			
			UserList list = CachedUserList;
			
			User ret = list[username];
			if (ret == null)
				throw new UserAuthenticationException (string.Format ("Could not login {0}", username));
			
			return _currentUser = ret;
		}
		
		override public UserList GetUsers ()
		{
			Debug.SimulateNetworkWait ();

			return CachedUserList;
		}

		private UserList CachedUserList
		{
			get 
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
						Email = "bill@mobillholmes.com"
					});
					users.Add (new User () {
						UserName="natfriedman",
						FullName="Nat Friedman",
						City="San Francisco, CA",
						Company="Xamarin",
						Title="CEO",
						Phone="(855)-926-2746",
						Email="nat@xamarin.com"
					});
					users.Add (new User () {
						UserName="migueldeicaza",
						FullName="Miguel de Icaza",
						City="Boston, MA",
						Company="Xamarin",
						Title="CTO",
						Phone="(855)-926-2746",
						Email="miguel@xamarin.com"
					});
					users.Add (new User () {
						UserName="josephhill",
						FullName="Joseph Hill",
						City="Boston, MA",
						Company="Xamarin",
						Title="COO",
						Phone="(855)-926-2746",
						Email="joseph@xamarin.com"
					});
					
					_userListForTesting = new UserList (users);
				}
				
				return _userListForTesting;
			}
		}
	}
}

