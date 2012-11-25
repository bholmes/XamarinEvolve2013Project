using System;
using Xamarin.Geolocation;
using System.Threading.Tasks;
using System.Threading;
using MonoTouch.UIKit;

namespace XamarinEvolveIOS
{
	public class GeolocationHelper
	{
		Geolocator _geolocator = new Geolocator ();
		private TaskScheduler _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		private CancellationTokenSource _cancelSource;

		public GeolocationHelper ()
		{
		
		}

		public void GetLocation (Action<GeolocationResult> onComplete)
		{
			this._cancelSource = new CancellationTokenSource();

			_geolocator.GetPositionAsync (timeout: 4000, cancelToken: this._cancelSource.Token, includeHeading: false)
				.ContinueWith (t =>
				{
					if (t.IsFaulted)
						Console.WriteLine (((GeolocationException)t.Exception.InnerException).Error.ToString());
					else if (t.IsCanceled)
						Console.WriteLine ("Canceled");
					else
					{
						Console.WriteLine (t.Result.Timestamp.ToString("G"));
						Console.WriteLine ("La: " + t.Result.Latitude.ToString("N4"));
						Console.WriteLine ("Lo: " + t.Result.Longitude.ToString("N4"));
					}

					if (onComplete != null)
						onComplete (new GeolocationResult (_geolocator, t));
					
				}, _scheduler);
		}

		public void CancelPosition ()
		{
			CancellationTokenSource cancel = this._cancelSource;
			if (cancel != null)
				cancel.Cancel();
		}
	}
}

