using System;

namespace XamarinEvolveSSLibrary
{
	public class GeoDistanceHelper
	{
		// cos(d) = sin(φА)·sin(φB) + cos(φА)·cos(φB)·cos(λА − λB),
		//  where φА, φB are latitudes and λА, λB are longitudes
		// Distance = d * R
		public static float DistanceBetweenPlaces(float lon1, float lat1, float lon2, float lat2)
		{
			float R = 6371; // km
			
			double sLat1 = Math.Sin(Radians(lat1));
			double sLat2 = Math.Sin(Radians(lat2));
			double cLat1 = Math.Cos(Radians(lat1));
			double cLat2 = Math.Cos(Radians(lat2));
			double cLon = Math.Cos(Radians(lon1) - Radians(lon2));
			
			double cosD = sLat1*sLat2 + cLat1*cLat2*cLon;
			
			double d = Math.Acos(cosD);
			
			double dist = R * d;
			
			return (float) dist;
		}
		
		public static float Radians(float x)
		{
			return x * ((float)Math.PI) / 180.0f;
		}
	}
}

