using System;
using System.IO;

namespace XamarinEvolveSSLibrary
{
	abstract public class AvatarAccess
	{
		byte [] _defaultImage;

		public AvatarAccess ()
		{
            // This is messing up my tests.
            try
            {
                _defaultImage = File.ReadAllBytes("Images/blankavatar.jpg");
            }// Going to ignore it
            catch (Exception) {}
		}

		public class GetAvatarResult
		{
			public byte [] Data {get;set;}
			public Exception Exceptin {get;set;}
		}

		public class PostNewAvatarResult
		{
			public Exception Exceptin {get;set;}
		}

		abstract protected byte [] InternalGetAvararForUser (User user, int size);

		public byte [] GetAvararForUser (User user, int size)
		{
			byte [] data = InternalGetAvararForUser (user, size);
			if (data != null && data.Length > 0)
				return data;

			data = GravatarHelper.GetGravatarImage (user, size);
			if (data != null && data.Length > 0)
				return data;

			return _defaultImage;
		}

		public byte [] GetAvararForUser (User user, int size, Action<GetAvatarResult> onComplete)
		{
			Func <int> func = delegate {
				try
				{
					byte [] data = GetAvararForUser (user, size);
					
					if (onComplete != null)
					{
						onComplete (new GetAvatarResult (){
							Data = data,
						});
					}
				}
				catch (Exception exp)
				{
					if (onComplete != null)
					{
						onComplete (new GetAvatarResult (){
							Exceptin = exp,
						});
					}
				}
				
				return 0;
			};
			func.BeginInvoke (null, null);

			return _defaultImage;
		}

		abstract protected void PostNewAvatar (byte [] data);
		
		public void PostNewAvatar (byte [] data, Action<PostNewAvatarResult> onComplete)
		{
			Func <int> func = delegate {
				try
				{
					PostNewAvatar (data);

					Engine.Instance.ImageCache.TouchUser (
						Engine.Instance.UserAccess.GetCurrentUser ());
					
					if (onComplete != null)
					{
						onComplete (new PostNewAvatarResult ());
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
}

