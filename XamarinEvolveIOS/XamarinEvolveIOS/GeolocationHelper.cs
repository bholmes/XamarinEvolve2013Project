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
		private int _timeout = 4000;

		public GeolocationHelper ()
		{

		}

		Task<Position> GetTask ()
		{
			this._cancelSource = new CancellationTokenSource();
			
			return _geolocator.GetPositionAsync (timeout: _timeout,
			                                     cancelToken: this._cancelSource.Token, 
			                                     includeHeading: false);
		}

		public GeolocationResult GetLocation ()
		{
			ManualResetEvent waitToComplete = new ManualResetEvent (false);

			GeolocationResult result = null;

			// Not getting the right results if not on main thread
			MonoTouch.Foundation.NSObject obj = new MonoTouch.Foundation.NSObject ();
			obj.InvokeOnMainThread (delegate {
			
				Task<Position> task =  GetTask ();

				task.ContinueWith (t =>
				{
					result = new GeolocationResult (_geolocator, t);
					waitToComplete.Set ();
					
				}, _scheduler);
			});

			waitToComplete.WaitOne ();

			return result;
		}

		public void GetLocation (Action<GeolocationResult> onComplete)
		{
			Task<Position> task =  GetTask ();

			task.ContinueWith (t =>
			{
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

