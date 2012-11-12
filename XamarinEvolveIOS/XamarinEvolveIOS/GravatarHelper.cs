using System;

namespace XamarinEvolveIOS
{
	public class GravatarHelper
	{
		static public string GetGravatarURL(string email, uint size)
		{
			if (string.IsNullOrEmpty (email))
			{
				throw new ArgumentException ("input can not be null or empty.", "input");
			}
			
			string md5 = CalculateMD5Hash (email.ToLower ().Trim ());
			
			if (size > 0)
				return string.Format ("http://www.gravatar.com/avatar/{0}.jpg?s={1}&d=mm", md5, size);
			
			return string.Format ("http://www.gravatar.com/avatar/{0}.jpg?d=mm", md5);
		}
		
		static private string CalculateMD5Hash(string input)
		{
			// step 1, calculate MD5 hash from input
			System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);
			
			// step 2, convert byte array to hex string
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("x2"));
			}
			return sb.ToString();
		}
	}
}

