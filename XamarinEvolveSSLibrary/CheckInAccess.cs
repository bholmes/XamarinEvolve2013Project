using System;
using System.Collections.Generic;
using System.Linq;

namespace XamarinEvolveSSLibrary
{
	abstract public class CheckInAccess
	{
		// Used when sorting places in the Meetup list
		// There is a limit to return
		//
		// Returns a list of places sorted by the number of 
		// checkins for that place
		abstract public PlaceList GetPopularPlaceList (int limit);

		// Used when sorting places in the Meetup list
		// There is a limit to return
		//
		// Returns a list of places sorted by the most 
		// recent chekin
		abstract public PlaceList GetRecentPlaceList (int limit);

		// Used when sorting places in the Meetup list
		// There is a limit to return
		//
		// Returns a list of places sorted by the shortest  
		// distance to the location specified
		abstract public PlaceList GetPlaceListNearLocation (float lat, float lng, int limit);

		// Used when viewing checkins at a place
		// There is a limit to the recent list
		// 
		// activeList lists users how have checked in in the last 'RecentThresholdHours' hours
		// AND that have not checked in elsewhere since
		//
		// recentList lists last 'recentLimit' users that have checked into 
		// a place and that are not in the activeList
		abstract public void GetCheckinInfo (Place place, out List<CheckInUserPair> activeList,
		                            out List<CheckInUserPair> recentList, int recentLimit);

		abstract public void CheckInUserAtPlace (Place place);
	}
}

