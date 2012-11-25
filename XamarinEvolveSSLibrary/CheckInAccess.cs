using System;
using System.Collections.Generic;

namespace XamarinEvolveSSLibrary
{
	public class CheckInAccess
	{
		CheckInList _checkInListForTesting;
		PlaceList _placeListForTesting;

		public PlaceList GetPlaceList ()
		{
			Debug.SimulateNetworkWait ();

			return CachedPlaceList.Copy ();
		}

		private PlaceList CachedPlaceList
		{
			get 
			{
				if (_placeListForTesting == null)
				{
					List <Place> places = new List<Place> ();

					places.Add (new Place {
						Id = 1,
						Name = "Downtown Burgers",
						Address = "310 East 3rd Street, Austin",
						Latitude = 30.2644490f,
						Longitude = -97.7405240f,
					});

					places.Add (new Place {
						Id = 2,
						Name = "Piranha Killer Sushi",
						Address = "207 San Jacinto Boulevard #202, Austin",
						Latitude = 30.2642190f,
						Longitude = -97.7414510f,
					});

					places.Add (new Place {
						Id = 3,
						Name = "Max's Wine Dive",
						Address = "207 San Jacinto Boulevard, Austin",
						Latitude = 30.2642140f,
						Longitude = -97.7414300f,
					});

					_placeListForTesting = new PlaceList (places);
				}
				
				return _placeListForTesting;
			}

		}

		private CheckInList CachedCheckInList
		{
			get 
			{
				if (_checkInListForTesting == null)
				{
					List <CheckIn> checkIns = new List<CheckIn> ();

					checkIns.Add (new CheckIn {
						Id = 1,
						UserName = "billholmes",
						PlaceId = 1,
						Time = DateTime.Now.Subtract (new TimeSpan (1, 2, 3, 4, 0)),
					});

					checkIns.Add (new CheckIn {
						Id = 2,
						UserName = "migueldeicaza",
						PlaceId = 2,
						Time = DateTime.Now.Subtract (new TimeSpan (0, 8, 6, 50, 0)),
					});

					checkIns.Add (new CheckIn {
						Id = 3,
						UserName = "billholmes",
						PlaceId = 3,
						Time = DateTime.Now.Subtract (new TimeSpan (0, 6, 11, 4, 0)),
					});

					checkIns.Add (new CheckIn {
						Id = 4,
						UserName = "josephhill",
						PlaceId = 3,
						Time = DateTime.Now.Subtract (new TimeSpan (0, 6, 11, 4, 30)),
					});

					_checkInListForTesting = new CheckInList (checkIns);
				}
				
				return _checkInListForTesting;
			}
		}
	}
}

