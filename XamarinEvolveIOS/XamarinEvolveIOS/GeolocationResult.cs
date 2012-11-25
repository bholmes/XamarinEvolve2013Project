using System;
using Xamarin.Geolocation;
using System.Threading.Tasks;

namespace XamarinEvolveIOS
{
	public class GeolocationResult
	{
		Task<Position> _task;
		Geolocator _geolocator;
		
		public GeolocationResult (Geolocator geolocator, Task<Position> task)
		{
			_task = task;
			_geolocator = geolocator;
			Exception = _task.Exception;
		}
		
		public Position Position 
		{
			get
			{
				if (_task.IsFaulted || _task.IsCanceled)
					return null;
				
				return _task.Result;
			}
		}
		public bool Canceled
		{
			get
			{
				return _task.IsCanceled;
			}
		}
		
		public bool GeolocationNotAvailable
		{
			get
			{
				return _task.IsFaulted && !_geolocator.IsGeolocationAvailable;
			}
		}
		
		public bool GeolocationDisabled
		{
			get
			{
				return _task.IsFaulted && !_geolocator.IsGeolocationEnabled;
			}
		}

		public Exception Exception {get; private set;}
	}
}

