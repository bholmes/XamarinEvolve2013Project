using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XamarinEvolveSSLibrary;

namespace TestSSAPI
{
    class CheckInLINQTest
    {
        public void RunTests()
        {
            List<Place> placeList;
            List<CheckIn> checkIns;
            SetupTables(out placeList, out checkIns);

            List<CheckIn> tActiveList;
            List<CheckIn> tRecentList;
            int[] intResults;

            //////////
            GetCheckinInfo(checkIns, placeList, placeList[3], out tActiveList, out tRecentList, 50, 50);

            intResults = new int[] { 6,2 };
            CheckResults(tActiveList, intResults, c => c.Id);
            intResults = new int[] { 4 };
            CheckResults(tRecentList, intResults, c => c.Id);

            //////////
            GetCheckinInfo(checkIns, placeList, placeList[3], out tActiveList, out tRecentList, 50, 47);

            intResults = new int[] { 6 };
            CheckResults(tActiveList, intResults, c => c.Id);
            intResults = new int[] { 4, 2 };
            CheckResults(tRecentList, intResults, c => c.Id);

            //////////////
            intResults = new int[] { 4,1,5,6 };
            CheckResults(
                GetPopularPlaceList(checkIns, placeList, 50), 
                intResults, c => c.Id);
            
            
            //////////////
            intResults = new int[] { 4, 1, 6, 5 };
            CheckResults(
                SortPlaceByRecentCheckIns(checkIns, placeList, 50, 50),
                intResults, c => c.Id);
        }

        private static void CheckResults<TT>(IEnumerable<TT> list, int[] intResults, Func<TT, int> selector)
        {
            int index = 0;
            foreach (TT t in list)
            {
                if (index >= intResults.Length)
                    throw new Exception("List sizes differ");

                if (selector(t) != intResults[index])
                    throw new Exception(string.Format ("items differ at index {0}", index));

                index++;
            }

            if (index != intResults.Length)
                throw new Exception("List sizes differ");
        }

        void GetCheckinInfo(List<CheckIn> CheckInList, List<Place> PlaceList,
            Place place, out List<CheckIn> activeList, out List<CheckIn> recentList,
            int recentLimit, int hours)
        {
            DateTime refTime = DateTime.Now - new TimeSpan
                (0, hours, 0, 0, 0);

            // All checkins in the time frame sorted
            var checkInsInTimeFrame = CheckInList
                .Where(c => c.Time > refTime)
                    .OrderByDescending(c => c.Time);

            // Last unique user check-in to the palce list
            var possibleActiveUserQuery = checkInsInTimeFrame
                .Where(c => c.PlaceId == place.Id)
                    .Select(c => c.UserName).Distinct();

            var actList = new List<CheckIn>();

            foreach (string possibleActiveUser in possibleActiveUserQuery)
            {
                CheckIn tCheckIn = checkInsInTimeFrame
                    .FirstOrDefault(c => c.UserName == possibleActiveUser);

                if (tCheckIn.PlaceId == place.Id)
                {
                    actList.Add(tCheckIn);
                }
            }

            var allRecentCheckIns = CheckInList
                .Where(c => c.PlaceId == place.Id &&
                        !actList.Select(actC => actC.UserName).Contains(c.UserName))
                    .OrderByDescending(c => c.Time);

            allRecentCheckIns.GroupBy(c => c.UserName);

            var recentCheckInsNames = allRecentCheckIns.Select(c => c.UserName)
                .Distinct()
                    .Take(recentLimit);

            var recList = new List<CheckIn>();

            foreach (string recentCheckInUserName in recentCheckInsNames)
            {
                recList.Add(allRecentCheckIns.FirstOrDefault(c => c.UserName == recentCheckInUserName));
            }

            activeList = actList;
            recentList = recList;
        }

        List<Place> GetPopularPlaceList(List<CheckIn> CheckInList, List<Place> PlaceList, int limit)
        {
            //var query = PlaceList.
            //            OrderByDescending(p => p.NumberOfCheckIns).
            //                Take(limit);

            //return query.ToList();


            var query2 = CheckInList.GroupBy(c => c.PlaceId)
                .OrderByDescending(g => g.Count())
                .Select(g => PlaceList.First(p => p.Id == g.First().PlaceId));

            return query2.ToList();

        }

        List<Place> SortPlaceByRecentCheckIns(List<CheckIn> CheckInList, List<Place> PlaceList, int limit, int hours)
        {
            List<Place> resultList = new List<Place>();

            DateTime refTime = DateTime.Now - new TimeSpan
                (0, hours, 0, 0, 0);

            // List of unique place Ids sorted by most recent check-in
            var checkInList = CheckInList.
                Where(c => c.Time > refTime).
                    OrderByDescending(c => c.Time).
                    Select(c => c.PlaceId).
                    Take(limit).
                    Distinct();

            // List of places from previous query 'checkInList'
            var placeList = PlaceList.
                Where(p => checkInList.Contains(p.Id));

            // Build a list of places in the right order
            // Must be a better way?
            foreach (int placeId in checkInList)
            {
                resultList.Add(placeList.FirstOrDefault(p => p.Id == placeId));
            }

            return resultList.ToList();
        }

        void CheckInUser(string user, Place place, DateTime time, List<CheckIn> checkInList, List<Place> placeList)
        {
            placeList.FirstOrDefault(p => p.Id == place.Id).NumberOfCheckIns++;
            CheckIn newCheckIn = new CheckIn { UserName = user, PlaceId = place.Id, Time = time, Id = checkInList.Count };
            checkInList.Add(newCheckIn);
        }

        List<Place> GetPlaceList()
        {
            List<Place> list = new List<Place>();
            list.Add(new Place { Id = 1, Name = "Home" });
            list.Add(new Place { Id = 2, Name = "Work" });
            list.Add(new Place { Id = 3, Name = "Bar" });
            list.Add(new Place { Id = 4, Name = "Hotel" });
            list.Add(new Place { Id = 5, Name = "Joe's" });
            list.Add(new Place { Id = 6, Name = "Giant Eagle" });
            return list;
        }

        private void SetupTables(out List<Place> placeList, out List<CheckIn> checkIns)
        {
            DateTime current = DateTime.Now - new TimeSpan(48, 0, 0);
            placeList = GetPlaceList();
            checkIns = new List<CheckIn>();

            CheckInUser("Bill", placeList[0], current, checkIns, placeList);

            current += new TimeSpan(0, 45, 0);
            CheckInUser("Bill", placeList[4], current, checkIns, placeList);

            current += new TimeSpan(0, 15, 0);
            CheckInUser("Joe", placeList[3], current, checkIns, placeList);

            current += new TimeSpan(0, 15, 0);
            CheckInUser("Bill", placeList[5], current, checkIns, placeList);

            current += new TimeSpan(0, 15, 0);
            CheckInUser("Pete", placeList[3], current, checkIns, placeList);

            current += new TimeSpan(0, 15, 0);
            CheckInUser("Pete", placeList[0], current, checkIns, placeList);

            current += new TimeSpan(0, 15, 0);
            CheckInUser("Jason", placeList[3], current, checkIns, placeList);
        }
    }
}
