using System;
using System.Collections.Generic;

namespace XamarinEvolveSSLibrary
{
	public class UserList
	{
		List<User> _list;

		public UserList ()
		{
			_list = new List<User> ();
		}

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
}

