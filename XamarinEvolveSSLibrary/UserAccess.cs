using System;

namespace XamarinEvolveSSLibrary
{
	abstract public class UserAccess 
	{
		abstract public User GetCurrentUser ();
		abstract protected User CreateNewUser (string username, string password);
		abstract protected User UserLogin (string username, string password);
		abstract protected UserList GetUsers ();
		
		public class UserAsyncResult
		{
			public User User {get; set;}
			public Exception Exceptin {get;set;}
		}

		public class UserListAsyncResult
		{
			public UserList UserList {get; set;}
			public Exception Exceptin {get;set;}
		}
		
		protected void UserAsyncCall (Func<User> call, Action<UserAsyncResult> onComplete)
		{
			Func <int> func = delegate {
				try
				{
					User newUser = call ();
					
					if (onComplete != null)
					{
						onComplete (new UserAsyncResult (){
							User = newUser
						});
					}
				}
				catch (Exception exp)
				{
					if (onComplete != null)
					{
						onComplete (new UserAsyncResult (){
							Exceptin = exp
						});
					}
				}
				
				return 0;
			};
			func.BeginInvoke (null, null);
		}

		protected void UserListAsyncCall (Func<UserList> call, Action<UserListAsyncResult> onComplete)
		{
			Func <int> func = delegate {
				try
				{
					UserList newUser = call ();
					
					if (onComplete != null)
					{
						onComplete (new UserListAsyncResult (){
							UserList = newUser
						});
					}
				}
				catch (Exception exp)
				{
					if (onComplete != null)
					{
						onComplete (new UserListAsyncResult (){
							Exceptin = exp
						});
					}
				}
				
				return 0;
			};
			func.BeginInvoke (null, null);
		}
		
		public void CreateNewUser (string username, string password, Action<UserAsyncResult> onComplete)
		{
			UserAsyncCall (delegate {
				return CreateNewUser (username, password);
			}, onComplete);
		}
		
		public void UserLogin (string username, string password, Action<UserAsyncResult> onComplete)
		{
			UserAsyncCall (delegate {
				return UserLogin (username, password);
			}, onComplete);
		}

		public void GetUsers (Action<UserListAsyncResult> onComplete)
		{
			UserListAsyncCall (delegate {
				return GetUsers ();
			}, onComplete);
		}

		public virtual void CommitCurrentUserChanges ()
		{
			// Noop
		}
	}
}

