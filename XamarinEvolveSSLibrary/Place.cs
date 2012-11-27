using System;

namespace XamarinEvolveSSLibrary
{
	public class Place
	{
		public int Id {get;set;}
		public string Name {get;set;}
		public string Address {get;set;}
		public float Latitude {get;set;}
		public float Longitude {get;set;}

		public float DistanceFrom (float lat, float lng)
		{
			return GeoDistanceHelper.DistanceBetweenPlaces (
				lng, lat, Longitude, Latitude);
		}
	}
}

