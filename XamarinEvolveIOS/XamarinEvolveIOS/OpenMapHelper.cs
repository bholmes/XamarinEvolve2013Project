using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MapKit;
using MonoTouch.CoreLocation;
using XamarinEvolveSSLibrary.GoogleAPI;
using XamarinEvolveSSLibrary;

namespace XamarinEvolveIOS
{
	public class OpenMapHelper
	{
		public static void OpenInMaps (Place place )
		{
			bool canDoIOS6Maps = false;
			
			using (NSString curVer = new NSString (UIDevice.CurrentDevice.SystemVersion))
			{
				using (NSString targetVar = new NSString ("6.0"))
				{
					canDoIOS6Maps = curVer.Compare (
						targetVar, NSStringCompareOptions.NumericSearch) != NSComparisonResult.Ascending;
					
				}
			}
			
			if (canDoIOS6Maps)
				OpenIOSMap (place);
			else
				OpenGoogleMap (place);
		}

		static void OpenIOSMap (Place place)
		{
			double latitude = place.Latitude;
			double longitude = place.Longitude;
			CLLocationCoordinate2D center = new CLLocationCoordinate2D (latitude, longitude);
			
			MKPlacemark placemark = new MKPlacemark (center, null);
			MKMapItem mapItem = new MKMapItem (placemark);
			mapItem.Name = place.Name;
			MKLaunchOptions options = new MKLaunchOptions ();
			options.MapSpan = MKCoordinateRegion.FromDistance (center, 200, 200).Span;
			mapItem.OpenInMaps (options);
		}
		
		static void OpenGoogleMap (Place place)
		{
			UIApplication.SharedApplication
				.OpenUrl (new NSUrl (string.Format("https://maps.google.com/?q={0},{1}",
				                                   place.Latitude,
				                                   place.Longitude)));
		}
	}
}

