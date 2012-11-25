using System;
using System.Text;
using ServiceStack.ServiceClient.Web;
using System.Collections.Generic;

namespace XamarinEvolveSSLibrary.GoogleAPI
{
	public class Place
	{
		public string name {get;set;}
		public string vicinity {get;set;} 
		public PlaceGometry geometry {get;set;}
		public string [] types {get;set;}
	}
	
	public class PlaceGometry
	{
		public PlaceLocaiton location {get;set;}
	}
	
	public class PlaceLocaiton
	{
		public float lat {get;set;}
		public float lng {get;set;}
	}

	public class PlaceLocator
	{
		JsonServiceClient _client = new JsonServiceClient("https://maps.googleapis.com/maps/api/place");

		public List<Place> GetNearbyPlaces ()
		{	
			return GetNearbyPlaces (30.264108f,  -97.73955f);
		}

		public List<Place> GetNearbyPlaces (float lat, float lng)
		{	
			List<Place> ret = new List<Place> ();

			PlaceSearch search = new PlaceSearch {
				lat = lat,
				lng = lng,
				types="bar|cafe|casino|food|gym|hospital|liquor_store|lodging|movie_theater|night_club|restaurant|stadium|store|zoo",
				sensor="false",
			};
			
			string uri = string .Format("nearbysearch/json{0}", 
			                            search.GetQueryString ());

			PlaceSearchResponse response = null;

			try
			{
				response = _client.Get <PlaceSearchResponse> (uri);
			}
			catch (Exception)
			{

			}

			if (response != null && response.results != null &&
			    response.results.Length > 0)
				ret = new List<Place> (response.results);

			return ret;
		}


		private class PlaceSearch
		{
			public float lat {get;set;}
			public float lng {get;set;}
			public string types {get;set;}
			public string sensor {get;set;}
			
			public string GetQueryString ()
			{
				StringBuilder bob = new StringBuilder ();
				bob.AppendFormat ("?key={0}&&rankby=distance&location={1},{2}&", 
				                  SystemConstants.GoogleAPIKey, lat,lng);
				if (types != null)
					bob.AppendFormat ("types={0}&", types);
				if (sensor != null)
					bob.AppendFormat ("sensor={0}&", sensor);
				
				return bob.ToString ();
			}
		}

		private class PlaceSearchResponse
		{
			public string status {get;set;}
			public string next_page_token {get;set;}
			public Place [] results {get;set;}
		}
	}
}

