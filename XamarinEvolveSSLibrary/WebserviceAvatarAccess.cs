using System;
using System.IO;

namespace XamarinEvolveSSLibrary
{
	public class WebserviceAvatarAccess : AvatarAccess
	{
		private ClientWrapper _clientWrapper;
		
		public WebserviceAvatarAccess (ClientWrapper clientWrapper)
		{
			_clientWrapper = clientWrapper;
		}

		#region implemented abstract members of AvatarAccess

		protected override byte[] InternalGetAvararForUser (User user, int size)
		{
			lock (_clientWrapper.ClientLock)
			{
				string uri = string.Format ("{0}/UserAvatar/{1}/{2}", 
				                            _clientWrapper.Client.BaseUri, user.UserName, size);
				try
				{
					using (System.Net.WebClient webClient = new System.Net.WebClient ())
					{
						byte [] ret = webClient.DownloadData(uri);
						if (ret != null && ret.Length > 0)
							return ret;
					}
				}
				catch (Exception exp)
				{

				}
			}

			return null;
		}

		public override void PostNewAvatar (byte[] data)
		{
			User currentUser = Engine.Instance.UserAccess.GetCurrentUser ();

			UserAvatar userAvatar = new UserAvatar()
			{
				UserName = currentUser.UserName,
				Data = data,
				Size = 200,
			};

			lock (_clientWrapper.ClientLock)
			{
				_clientWrapper.Client.Post<UserAvatarResponse>("UserAvatar", userAvatar);
			}
		}

		#endregion
	}
}

