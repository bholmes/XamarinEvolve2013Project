using System;

namespace XamarinEvolveSSLibrary
{
	abstract public class AvatarAccess
	{
		public class PostNewAvatarResult
		{
			public string URL{get;set;}
			public Exception Exceptin{get;set;}
		}
		
		abstract public string PostNewAvatar (byte [] data);
		
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
}

