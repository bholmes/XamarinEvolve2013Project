using System;

namespace XamarinEvolveSSLibrary
{
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

