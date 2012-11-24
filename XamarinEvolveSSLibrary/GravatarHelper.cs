using System;
using System.Net;

namespace XamarinEvolveSSLibrary
{
	public class GravatarHelper
	{
		static public string GetGravatarURL(string email, int size)
		{
			if (string.IsNullOrEmpty (email))
			{
				throw new ArgumentException ("input can not be null or empty.", "input");
			}
			
			string md5 = MD5Helper.CalculateMD5Hash (email.ToLower ().Trim ());
			
			if (size > 0)
				return string.Format ("http://www.gravatar.com/avatar/{0}.jpg?s={1}&d=404", md5, size);
			
			return string.Format ("http://www.gravatar.com/avatar/{0}.jpg?d=404", md5);
		}

		static public byte [] GetGravatarImage (User user, int size)
		{
			if (string.IsNullOrEmpty (user.Email))
				return null;

			string urlString = GetGravatarURL (user.Email, size);
			try
			{
				using (WebClient client = new WebClient ())
				{
					byte [] data = client.DownloadData (urlString);
					if (data != null && data.Length > 1)
						return data;
				}
			}
			catch
			{

			}

			return null;
		}
	}
}

