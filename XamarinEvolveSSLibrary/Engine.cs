using System;

namespace XamarinEvolveSSLibrary
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
		
		public UserAccess UserAccess {get; private set;}
		public AvatarAccess AvatarAccess {get; private set;}
		
		public Engine ()
		{
			UserAccess = new UserAccessLocalTest ();
			AvatarAccess = new AvatarAccessLocalTest ();
		}
	}
}

