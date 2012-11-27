using System;
using System.Collections.Generic;
using System.Linq;

namespace XamarinEvolveSSLibrary
{
	public class CheckInAccessLocalTest : CheckInAccess
	{
		CheckInList _checkInListForTesting;
		PlaceList _placeListForTesting;
		
		UserList _sharedUserList;
		
		public CheckInAccessLocalTest (UserList userList)
		{
			_sharedUserList = userList;
		}
		
		// Used when sorting places in the Meetup list
		// There is a limit to return
		//
		// Returns a list of places sorted by the number of 
		// checkins for that place
		override public PlaceList GetPopularPlaceList (int limit)
		{
			Debug.SimulateNetworkWait ();
			
			var query = CachedCheckInList.CheckIns
				.GroupBy(c => c.PlaceId)
					.OrderByDescending(g => g.Count())
					.Take(limit)
					.Select(g => CachedPlaceList.Places.First(p => p.Id == g.First().PlaceId));
			
			return new PlaceList (query.ToList ());
		}
		
		// Used when sorting places in the Meetup list
		// There is a limit to return
		//
		// Returns a list of places sorted by the most 
		// recent chekin
		override public PlaceList GetRecentPlaceList (int limit)
		{
			Debug.SimulateNetworkWait ();
			
			DateTime refTime = DateTime.Now - new TimeSpan
				(0, SystemConstants.RecentThresholdHours, 0, 0, 0);
			
			// List of unique place Ids sorted by most recent check-in
			var checkInList = CachedCheckInList.CheckIns
				.Where(c => c.Time > refTime)
					.OrderByDescending(c => c.Time)
					.Select(c => c.PlaceId)
					.Take(limit)
					.Distinct();
			
			// Build a list of places in the right order
			var placeList = checkInList.Select(placeId => CachedPlaceList.Places.FirstOrDefault(p => p.Id == placeId));
			
			return new PlaceList (placeList.ToList());
		}
		
		// Used when sorting places in the Meetup list
		// There is a limit to return
		//
		// Returns a list of places sorted by the shortest  
		// distance to the location specified
		override public PlaceList GetPlaceListNearLocation (float lat, float lng, int limit)
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
		override public void GetCheckinInfo (Place place, out List<CheckInUserPair> activeList,
		                            out List<CheckInUserPair> recentList, int recentLimit)
		{
			Debug.SimulateNetworkWait ();
			
			DateTime refTime = DateTime.Now - new TimeSpan 
				(0, SystemConstants.RecentThresholdHours, 0, 0, 0);
			
			// Groups of checkins grouped by user then sorted by time
			var sortedGroupList = CachedCheckInList.CheckIns.OrderByDescending(c => c.Time)
				.GroupBy(c => c.UserId);
			
			// represents users last check-in anywhere within time limit
			var usersLastCheckIn = sortedGroupList.Select(g => g.First()).Where(c => c.Time > refTime);
			
			// represents users last check-in at the place
			var usersListCheckInToPlace = sortedGroupList.Select(g => g.FirstOrDefault(c => c.PlaceId == place.Id)).Where(c => c != null);
			
			// The activeList is where the users 
			// last check-in anywhere within the time limit
			// AND
			// at this place, are the same place.
			var actList = usersListCheckInToPlace.Intersect(usersLastCheckIn);
			
			// the recent list is all users last check-in at the place
			// that is not in the active list
			// limited by recentLimit
			var recList = usersListCheckInToPlace.Except(actList).Take(recentLimit);
			
			List<User> userList = _sharedUserList.Users;
			
			activeList = actList.Select (
				c=> new CheckInUserPair { CheckIn=c, User = userList.FirstOrDefault (u=> u.Id == c.UserId)})
				.ToList ();
			
			recentList = recList.Select (c=> new CheckInUserPair { CheckIn=c, User = userList.FirstOrDefault (u=> u.Id == c.UserId)})
				.ToList ();
		}
		
		override public void CheckInUserAtPlace (Place place)
		{
			User currentUser = Engine.Instance.UserAccess.GetCurrentUser ();
			if (currentUser.IsAnonymousUser)
				return;
			
			Place tPlace = CachedPlaceList.AddIfNew (place);
			CachedCheckInList.Add (new CheckIn {
				PlaceId = tPlace.Id,
				Time = DateTime.Now,
				UserId = currentUser.Id,
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
						Longitude = -97.7405240f
					});
					
					places.Add (new Place {
						Id = 2,
						Name = "Piranha Killer Sushi",
						Address = "207 San Jacinto Boulevard #202, Austin",
						Latitude = 30.2642190f,
						Longitude = -97.7414510f
					});
					
					places.Add (new Place {
						Id = 3,
						Name = "Max's Wine Dive",
						Address = "207 San Jacinto Boulevard, Austin",
						Latitude = 30.2642140f,
						Longitude = -97.7414300f
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
						UserId = _sharedUserList.Users.First (u=>u.UserName == "billholmes").Id,
						PlaceId = 1,
						Time = DateTime.Now.Subtract (new TimeSpan (0, 5, 3, 4, 0)),
					});
					
					checkIns.Add (new CheckIn {
						Id = 2,
						UserId = _sharedUserList.Users.First (u=>u.UserName == "migueldeicaza").Id,
						PlaceId = 2,
						Time = DateTime.Now.Subtract (new TimeSpan (0, 1, 30, 50, 0)),
					});
					
					checkIns.Add (new CheckIn {
						Id = 3,
						UserId = _sharedUserList.Users.First (u=>u.UserName == "billholmes").Id,
						PlaceId = 3,
						Time = DateTime.Now.Subtract (new TimeSpan (0, 0, 11, 4, 0)),
					});
					
					checkIns.Add (new CheckIn {
						Id = 4,
						UserId = _sharedUserList.Users.First (u=>u.UserName == "josephhill").Id,
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

