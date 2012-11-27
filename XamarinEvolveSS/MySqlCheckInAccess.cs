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
        public PlaceList GetPopularPlaceList(int limit)
        {
            throw new NotImplementedException();
        }

        public PlaceList GetRecentPlaceList(int limit)
        {
            throw new NotImplementedException();
        }

        public PlaceList GetPlaceListNearLocation(float lat, float lng, int limit)
        {
            throw new NotImplementedException();
        }

        public void GetCheckinInfo(Place place, out List<CheckInUserPair> activeList, out List<CheckInUserPair> recentList, int recentLimit)
        {
            throw new NotImplementedException();
        }

        public void CheckInUserAtPlace(string username, Place place)
        {
            SetupAndCall((dbCmd) =>
            {
                SqlExpressionVisitor<Place> evPlace = OrmLiteConfig.DialectProvider.ExpressionVisitor<Place>();
                SqlExpressionVisitor<User> evUser = OrmLiteConfig.DialectProvider.ExpressionVisitor<User>();

                // Add place to table if it does not exist
                // compared my name and address
                int placeId;
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

                // Find the user Id
                int userId;
                evUser.Where(u => u.UserName == username).Select(u => u.Id);
                var userResult = dbCmd.Select(evUser);

                if (userResult.Count == 0)
                    throw new ArgumentException(string.Format("username : '{0}' does not exist", username));
                userId = userResult[0].Id;

                CheckIn checkIn = new CheckIn { UserId = userId, PlaceId = placeId, Time = DateTime.Now };
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