using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XamarinEvolveSSLibrary;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;

namespace XamarinEvolveSS
{
    public class MySqlCheckInAccess
    {
        public List<Place> GetPopularPlaceList(int limit)
        {
            List<Place> returnList = new List<Place>();

            SetupAndCall((dbCmd) =>
            {
                SqlExpressionVisitor<Place> evPlace = OrmLiteConfig.DialectProvider.ExpressionVisitor<Place>();
                SqlExpressionVisitor<CheckIn> evCheckIn = OrmLiteConfig.DialectProvider.ExpressionVisitor<CheckIn>();

                var checkInList = dbCmd.Select(evCheckIn);

                var placeIdList = checkInList
                    .GroupBy(c => c.PlaceId)
                        .OrderByDescending(g => g.Count())
                        .Take(limit)
                        .Select (g=>(object)g.First().PlaceId);

                evPlace.Where(p => Sql.In(p.Id, placeIdList.ToList()));
                var unsortedPlaceList = dbCmd.Select(evPlace);

                returnList = placeIdList.Select(pId => unsortedPlaceList.First(p => p.Id == (int)pId)).ToList ();
            });

            return returnList;
        }

        public List<Place> GetRecentPlaceList(int limit)
        {
            List<Place> returnList = new List<Place>();

            SetupAndCall((dbCmd) =>
            {
                SqlExpressionVisitor<Place> evPlace = OrmLiteConfig.DialectProvider.ExpressionVisitor<Place>();
                SqlExpressionVisitor<CheckIn> evCheckIn = OrmLiteConfig.DialectProvider.ExpressionVisitor<CheckIn>();

                DateTime refTime = DateTime.UtcNow - new TimeSpan
                    (0, SystemConstants.RecentThresholdHours, 0, 0, 0);

                // List of unique place Ids sorted by most recent check-in
                evCheckIn.Where(c => c.Time > refTime)
                    .OrderByDescending(c => c.Time)
                    .Select (c=>c.PlaceId);

                var checkInRsponse = dbCmd.Select(evCheckIn);
                var checkInIds = checkInRsponse
                    .Select(c => (object)c.PlaceId)
                    .Distinct()
                    .Take(limit).ToArray ();

                evPlace.Where(p => Sql.In(p.Id, checkInIds));
                var unsortedPlaces = dbCmd.Select(evPlace);

                var placeList = checkInIds.Select(placeId => unsortedPlaces.FirstOrDefault(p => p.Id == (int)placeId));

                returnList = placeList.ToList();
            });

            return returnList;
        }

        public List<Place> GetPlaceListNearLocation(float lat, float lng, int limit)
        {
            List<Place> returnList = new List<Place>();

            SetupAndCall((dbCmd) =>
            {
                SqlExpressionVisitor<Place> evPlace = OrmLiteConfig.DialectProvider.ExpressionVisitor<Place>();
                var placeList = dbCmd.Select(evPlace);

                returnList = new PlaceList(placeList.ToList()).SortByDistance(lat, lng, limit).Places;
            });

            return returnList;
        }

        public void GetCheckinInfo(int placeId, out List<CheckInUserPair> activeList, out List<CheckInUserPair> recentList, int recentLimit)
        {
            List<CheckInUserPair>  t_activeList = new List<CheckInUserPair>();
            List<CheckInUserPair> t_recentList = new List<CheckInUserPair>();

            SetupAndCall((dbCmd) =>
            {
                SqlExpressionVisitor<CheckIn> evCheckIn = OrmLiteConfig.DialectProvider.ExpressionVisitor<CheckIn>();
                SqlExpressionVisitor<User> evUser = OrmLiteConfig.DialectProvider.ExpressionVisitor<User>();

                DateTime refTime = DateTime.UtcNow - new TimeSpan
                    (0, SystemConstants.RecentThresholdHours, 0, 0, 0);

                // Groups of checkins grouped by user then sorted by time
                var sortedCheckInList = dbCmd.Select(evCheckIn.OrderByDescending(c => c.Time));
                var sortedGroupList = sortedCheckInList.GroupBy(c => c.UserId);

                // represents users last check-in anywhere within time limit
                var usersLastCheckIn = sortedGroupList.Select(g => g.First()).Where(c => c.Time > refTime);

                // represents users last check-in at the place
                var usersListCheckInToPlace = sortedGroupList.Select(g => g.FirstOrDefault(c => c.PlaceId == placeId)).Where(c => c != null);

                // The activeList is where the users 
                // last check-in anywhere within the time limit
                // AND
                // at this place, are the same place.
                var actList = usersListCheckInToPlace.Intersect(usersLastCheckIn);

                // the recent list is all users last check-in at the place
                // that is not in the active list
                // limited by recentLimit
                var recList = usersListCheckInToPlace.Except(actList).Take(recentLimit);

                // does this need to be an object [] again?
                var allUserIds = actList.Select(c => (object)c.UserId).Union(recList.Select(c => (object)c.UserId)).ToArray ();

                if (!allUserIds.Any())
                    return;

                evUser.Where(u => Sql.In(u.Id, allUserIds));
                var userList = dbCmd.Select(evUser);

                t_activeList = actList.Select(
                    c => new CheckInUserPair { CheckIn = c, User = userList.FirstOrDefault(u => u.Id == c.UserId) })
                    .ToList();

                t_recentList = recList.Select(c => new CheckInUserPair { CheckIn = c, User = userList.FirstOrDefault(u => u.Id == c.UserId) })
                    .ToList();

            });

            activeList = t_activeList;
            recentList = t_recentList;
        }

        public void CheckInUserAtPlace(string username, Place place)
        {
            SetupAndCall((dbCmd) =>
            {
                SqlExpressionVisitor<Place> evPlace = OrmLiteConfig.DialectProvider.ExpressionVisitor<Place>();
                SqlExpressionVisitor<User> evUser = OrmLiteConfig.DialectProvider.ExpressionVisitor<User>();
                SqlExpressionVisitor<CheckIn> evCheckIn = OrmLiteConfig.DialectProvider.ExpressionVisitor<CheckIn>();

                // Find the user Id
                int userId;
                evUser.Where(u => u.UserName == username).Select(u => u.Id);
                var userResult = dbCmd.Select(evUser);

                if (userResult.Count == 0)
                    throw new ArgumentException(string.Format("username : '{0}' does not exist", username));
                userId = userResult[0].Id;
               

                //// Get the last checkin for the user
                evCheckIn.Where(c => c.UserId == userId).OrderByDescending(c => c.Time).Limit(1);
                var lastCheckIn = dbCmd.Select(evCheckIn);

                if (lastCheckIn.Count != 0)
                {
                    DateTime refTime = DateTime.UtcNow - new TimeSpan
                    (0, SystemConstants.RecentThresholdHours, 0, 0, 0);

                    // If it has been a while allow the checkin
                    if (refTime < lastCheckIn.First().Time)
                    {
                        //// Find the palce for that place id
                        evPlace.Where(p => p.Id == lastCheckIn.First().PlaceId).Limit(1);
                        var existingPlaceResult = dbCmd.Select(evPlace);

                        // if it is the same place just update the time
                        if (existingPlaceResult.First().Name == place.Name &&
                            existingPlaceResult.First().Address == place.Address)
                        {
                            evCheckIn.Where(c => c.Id == lastCheckIn.First().Id).Update(c => c.Time);
                            dbCmd.UpdateOnly(new CheckIn { Time = DateTime.UtcNow }, evCheckIn);
                            return;
                        }
                    }
                }

                // Add place to table if it does not exist
                // compared my name and address
                int placeId;
                evPlace = OrmLiteConfig.DialectProvider.ExpressionVisitor<Place>();
                evPlace.Where(p => p.Name == place.Name && p.Address == place.Address)
                    .Select(p => p.Id);

                var placeResult = dbCmd.Select(evPlace);

                if (placeResult.Count == 0)
                {
                    dbCmd.Insert(place);
                    placeId = (int)dbCmd.GetLastInsertId();
                }
                else
                    placeId = placeResult[0].Id;

                // Check in the user
                CheckIn checkIn = new CheckIn { UserId = userId, PlaceId = placeId, Time = DateTime.UtcNow };
                dbCmd.Insert(checkIn);
            });            
        }

        public void DeleteCheckInsForUser(string username)
        {
            SetupAndCall((dbCmd) =>
            {
                SqlExpressionVisitor<User> evUser = OrmLiteConfig.DialectProvider.ExpressionVisitor<User>();
                SqlExpressionVisitor<CheckIn> evCheckIn = OrmLiteConfig.DialectProvider.ExpressionVisitor<CheckIn>();

                int userId;
                evUser.Where(u => u.UserName == username).Select(u => u.Id).Limit(1);
                var userResult = dbCmd.Select(evUser);

                if (userResult.Count == 0)
                    throw new ArgumentException(string.Format("username : '{0}' does not exist", username));
                userId = userResult[0].Id;

                evCheckIn.Where(c => c.UserId == userId);
                dbCmd.Delete(evCheckIn);
            });
        }

        private void SetupAndCall(Action<IDbCommand> call)
        {
            OrmLiteConfig.DialectProvider = MySqlDialectProvider.Instance;
            using (IDbConnection db =
                   string.Format("Server = {0}; Database = {1}; Uid = {2}; Pwd = {3}",
                   SystemConstants.DatabaseServer, SystemConstants.DatabaseName, SystemConstants.DatabaseUser,
                   SystemConstants.DatabasePassword).OpenDbConnection())
            {
                using (IDbCommand dbCmd = db.CreateCommand())
                {
                    call(dbCmd);
                }
            }
        }
    }
}