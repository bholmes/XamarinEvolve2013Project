using System;
using System.Collections.Generic;
using System.Linq;

namespace XamarinEvolveSSLibrary
{
	public class CheckInAccess
	{
		CheckInList _checkInListForTesting;
		PlaceList _placeListForTesting;

		// Used when sorting places in the Meetup list
		// There is a limit to return
		//
		// Returns a list of places sorted by the number of 
		// checkins for that place
		public PlaceList GetPopularPlaceList (int limit)
		{
			Debug.SimulateNetworkWait ();

			var query = CachedPlaceList.Places.
				OrderByDescending (p=>p.NumberOfCheckIns).
					Take (limit);

			return new PlaceList (query.ToList ());
		}

		// Used when sorting places in the Meetup list
		// There is a limit to return
		//
		// Returns a list of places sorted by the most 
		// recent chekin
		public PlaceList GetRecentPlaceList (int limit)
		{
			Debug.SimulateNetworkWait ();

			List <Place> resultList = new List<Place> ();

			DateTime refTime = DateTime.Now - new TimeSpan 
				(0, SystemConstants.RecentThresholdHours, 0, 0, 0);

			// List of unique place Ids sorted by most recent check-in
			var checkInList = CachedCheckInList.CheckIns.
				Where (c=>c.Time > refTime).
					OrderByDescending (c=>c.Time).
					Select (c=>c.PlaceId).
					Take (limit).
					Distinct ();

			// List of places from previous query 'checkInList'
			var placeList = CachedPlaceList.Places.
				Where (p=> checkInList.Contains (p.Id));

			// Build a list of places in the right order
			// Must be a better way?
			foreach (int placeId in checkInList)
			{
				resultList.Add (placeList.FirstOrDefault (p=>p.Id == placeId));
			}

			return new PlaceList (resultList);
		}

		// Used when sorting places in the Meetup list
		// There is a limit to return
		//
		// Returns a list of places sorted by the shortest  
		// distance to the location specified
		public PlaceList GetPlaceListNearLocation (float lat, float lng, int limit)
		{
			Debug.SimulateNetworkWait ();
			
			return CachedPlaceList.SortByDistance (lat, lng, limit);
		}

		// Used when viewing checkins at a place
		// There is a limit to the recent list
		// 
		// activeList lists users how have checked in in the last 'RecentThresholdHours' hours
		// AND that have not checked in elsewhere since
		//
		// recentList lists last 'recentLimit' users that have checked into 
		// a place and that are not in the activeList
		public void GetCheckinInfo (Place place, out CheckInList activeList,
		                            out CheckInList recentList, int recentLimit)
		{
			Debug.SimulateNetworkWait ();

			DateTime refTime = DateTime.Now - new TimeSpan 
				(0, SystemConstants.RecentThresholdHours, 0, 0, 0);

			// All checkins in the time frame sorted
			var checkInsInTimeFrame = CachedCheckInList.CheckIns
				.Where (c=>c.Time > refTime)
					.OrderByDescending (c=>c.Time);

			// Last unique user check-in to the palce list
			var possibleActiveUserQuery = checkInsInTimeFrame
				.Where (c=> c.PlaceId == place.Id)
					.Select(c=>c.UserName).Distinct ();

			var actList = new List <CheckIn>();

			foreach (string possibleActiveUser in possibleActiveUserQuery)
			{
				CheckIn tCheckIn = checkInsInTimeFrame
					.FirstOrDefault(c=>c.UserName == possibleActiveUser);

				if (tCheckIn.PlaceId == place.Id)
				{
					actList.Add (tCheckIn);
				}
			}

			var allRecentCheckIns = CachedCheckInList.CheckIns
				.Where (c=>c.PlaceId == place.Id && 
				        !actList.Select(actC=>actC.UserName).Contains(c.UserName))
					.OrderByDescending (c=>c.Time);

			var recentCheckInsNames = allRecentCheckIns.Select (c=>c.UserName)
				.Distinct ()
					.Take (recentLimit);

			var recList = new List<CheckIn> ();

			foreach (string recentCheckInUserName in recentCheckInsNames)
			{
				recList.Add (allRecentCheckIns.FirstOrDefault (c=>c.UserName == recentCheckInUserName));
			}

			activeList = new CheckInList (actList);
			recentList = new CheckInList (recList);

			if (false) // or I could do this
			{
				activeList = new CheckInList ();
				recentList = new CheckInList ();

				CachedCheckInList.GetCheckInsForPlace (place, SystemConstants.RecentThresholdHours,
			                                       out activeList, out recentList);
			}
		}

		public void CheckInUserAtPlace (Place place)
		{
			User currentUser = Engine.Instance.UserAccess.GetCurrentUser ();
			if (currentUser.IsAnonymousUser)
				return;

			Place tPlace = CachedPlaceList.AddIfNew (place);
			CachedCheckInList.Add (new CheckIn {
				PlaceId = tPlace.Id,
				Time = DateTime.Now,
				UserName = currentUser.UserName,
			});
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
						NumberOfCheckIns = 1,
					});

					places.Add (new Place {
						Id = 2,
						Name = "Piranha Killer Sushi",
						Address = "207 San Jacinto Boulevard #202, Austin",
						Latitude = 30.2642190f,
						Longitude = -97.7414510f,
						NumberOfCheckIns = 1,
					});

					places.Add (new Place {
						Id = 3,
						Name = "Max's Wine Dive",
						Address = "207 San Jacinto Boulevard, Austin",
						Latitude = 30.2642140f,
						Longitude = -97.7414300f,
						NumberOfCheckIns = 2,
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
						Time = DateTime.Now.Subtract (new TimeSpan (0, 5, 3, 4, 0)),
					});

					checkIns.Add (new CheckIn {
						Id = 2,
						UserName = "migueldeicaza",
						PlaceId = 2,
						Time = DateTime.Now.Subtract (new TimeSpan (0, 1, 30, 50, 0)),
					});

					checkIns.Add (new CheckIn {
						Id = 3,
						UserName = "billholmes",
						PlaceId = 3,
						Time = DateTime.Now.Subtract (new TimeSpan (0, 0, 11, 4, 0)),
					});

					checkIns.Add (new CheckIn {
						Id = 4,
						UserName = "josephhill",
						PlaceId = 3,
						Time = DateTime.Now.Subtract (new TimeSpan (0, 0, 11, 4, 30)),
					});

					_checkInListForTesting = new CheckInList (checkIns);
				}
				
				return _checkInListForTesting;
			}
		}
	}
}

