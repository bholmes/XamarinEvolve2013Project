using System;
using System.Collections.Generic;

namespace XamarinEvolveIOS
{
	public class Engine
	{
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

		public User GetCurrentUser ()
		{
			return GetUsers ()["billholmes"];
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
				return Engine.Instance.GetCurrentUser ().UserName.Equals (UserName);
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

				if (ret == null)
					throw new KeyNotFoundException (username + " not found");

				return ret;
			}
		}

		public int Count {get{return _list.Count;}}
	}

}

