using System;

namespace XamarinEvolveSSLibrary
{
	abstract public class UserAccess 
	{
		abstract public User GetCurrentUser ();
		abstract public User CreateNewUser (string username, string password);
		abstract public User UserLogin (string username, string password);
		abstract public UserList GetUsers ();
		
		public class UserLoginResult
		{
			public User User {get; set;}
			public Exception Exceptin {get;set;}
		}
		
		protected void AsyncUserLoginCall (Func<User> call, Action<UserLoginResult> onComplete)
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
		
		public void CreateNewUser (string username, string password, Action<UserLoginResult> onComplete)
		{
			AsyncUserLoginCall (delegate {
				return CreateNewUser (username, password);
			}, onComplete);
		}
		
		public void UserLogin (string username, string password, Action<UserLoginResult> onComplete)
		{
			AsyncUserLoginCall (delegate {
				return UserLogin (username, password);
			}, onComplete);
		}
	}
}

