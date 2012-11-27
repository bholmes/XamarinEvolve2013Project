using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XamarinEvolveSSLibrary;

namespace XamarinEvolveSS
{
    public class MySqlCheckInAccess : CheckInAccess
    {
        public override PlaceList GetPopularPlaceList(int limit)
        {
            throw new NotImplementedException();
        }

        public override PlaceList GetRecentPlaceList(int limit)
        {
            throw new NotImplementedException();
        }

        public override PlaceList GetPlaceListNearLocation(float lat, float lng, int limit)
        {
            throw new NotImplementedException();
        }

        public override void GetCheckinInfo(Place place, out List<CheckInUserPair> activeList, out List<CheckInUserPair> recentList, int recentLimit)
        {
            throw new NotImplementedException();
        }

        public override void CheckInUserAtPlace(Place place)
        {
            throw new NotImplementedException();
        }
    }
}