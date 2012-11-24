using System;

namespace XamarinEvolveSSLibrary
{
	public class Debug
	{
		static public void SimulateNetworkWait ()
		{
			SimulateNetworkWait (2000);
		}

		static public void SimulateNetworkWait (int milliseconds)
		{
			System.Threading.Thread.Sleep (milliseconds);
		}
	}
}

