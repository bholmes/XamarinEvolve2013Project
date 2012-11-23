using System;

namespace XamarinEvolveSSLibrary
{
	public class GravatarHelper
	{
		static public string GetGravatarURL(string email, uint size)
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
	}
}

