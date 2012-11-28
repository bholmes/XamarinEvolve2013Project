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
		public GoogleAPI.PlaceLocator PlaceLocator {get; private set;}
		public CheckInAccess CheckInAccess {get; private set;}

		public Engine ()
		{
			if (SystemConstants.EngineUseTestClasses)
			{
				UserAccessLocalTest userAccessLocalTest = new UserAccessLocalTest ();
				UserAccess = userAccessLocalTest;
				AvatarAccess = new AvatarAccessLocalTest ();
				CheckInAccess = new CheckInAccessLocalTest (userAccessLocalTest.CachedUserList);
			}

			else
			{
				ClientWrapper clientWrapper = new ClientWrapper ();
				UserAccess = new WebserviceUserAccess (clientWrapper);
				AvatarAccess = new WebserviceAvatarAccess (clientWrapper);
				CheckInAccess = new WebserviceCheckInAccess (clientWrapper);
			}

			PlaceLocator = new XamarinEvolveSSLibrary.GoogleAPI.PlaceLocator ();
		}
	}
}

