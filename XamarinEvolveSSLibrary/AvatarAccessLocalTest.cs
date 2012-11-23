using System;

namespace XamarinEvolveSSLibrary
{
	public class AvatarAccessLocalTest : AvatarAccess
	{
		override public string PostNewAvatar (byte [] data)
		{
			Debug.SimulateNetworkWait ();
			
			User currentUser = Engine.Instance.UserAccess.GetCurrentUser ();
			string fullName = currentUser.Avatar;
			
			if (string.IsNullOrEmpty (fullName))
			{
				string path = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
				string filename = System.IO.Path.GetRandomFileName ();
				
				fullName = System.IO.Path.Combine (path, filename + ".png");
			}
			else
			{
				if (fullName.StartsWith ("file:///"))
					fullName = fullName.Remove (0, "file://".Length);
			}
			
			System.IO.FileStream fileStream = null;
			try
			{
				fileStream = new System.IO.FileStream(
					fullName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
				fileStream.Write(data, 0, data.Length);
			}
			finally
			{
				if (fileStream != null)
					fileStream.Close();
			}
			
			currentUser.Avatar = fullName = string.Format ("file://{0}", fullName);
			
			return fullName;
		}
	}
}

