using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;
using XamarinEvolveSSLibrary;

namespace XamarinEvolveSS
{
    public class EvolveUsersMySqlAccess
    {
        public List<User> GetAllUsers()
        {
            List<User> result = null;

            SetupAndCall((dbCmd, ev) =>
            {
                ev.Where(rn => true);
                result = dbCmd.Select(ev);
            });
            return result;
        }

        public User FindUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("username is not valid");

            List<User> result = null;

            SetupAndCall((dbCmd, ev) =>
            {
                ev.Where(rn => username == rn.username);
                result = dbCmd.Select(ev);
            });

            if (result == null || result.Count == 0)
                throw new Exception(string.Format("user '{0}' not found", username));
            else if (result.Count > 1)
                throw new Exception(string.Format("too many users named '{0}' found'", username));

            return result[0];
        }

        public void DeleteUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("username is not valid");

            int result = 1;

            SetupAndCall((dbCmd, ev) =>
            {
                ev.Where(rn => username == rn.username);
                result = dbCmd.Delete(ev);
            });

            if (result != 1)
                throw new Exception(string.Format("user '{0}' not found", username));
        }

        public int AddUser(User user)
        {
            int result = 0;

            SetupAndCall((dbCmd, ev) =>
            {
                dbCmd.Insert(user);
                result = (int)dbCmd.GetLastInsertId();
            });

            return result;
        }

        public void UpdateUser(User user)
        {
            SetupAndCall((dbCmd, ev) =>
            {
                List<string> updateStrs = new List<string>();
                if (user.fullname != null)
                    updateStrs.Add("fullname");
                if (user.city != null)
                    updateStrs.Add("city");
                if (user.email != null)
                    updateStrs.Add("email");
                if (user.phone != null)
                    updateStrs.Add("phone");
                if (user.password != null)
                    updateStrs.Add("password");
                if (user.avatar != null)
                    updateStrs.Add("avatar");

                if (updateStrs.Count == 0)
                    return;

                ev.Where(rn => user.username == rn.username).UpdateFields = updateStrs;
                dbCmd.UpdateOnly(user, ev);
            });

            return;
        }

        private void SetupAndCall(Action<IDbCommand, SqlExpressionVisitor<User>> call)
        {
            OrmLiteConfig.DialectProvider = MySqlDialectProvider.Instance;
            SqlExpressionVisitor<User> ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<User>();

            using (IDbConnection db =
                   string.Format("Server = {0}; Database = {1}; Uid = {2}; Pwd = {3}",
                   SystemConstants.DatabaseServer, SystemConstants.DatabaseName, SystemConstants.DatabaseUser,
                   SystemConstants.DatabasePassword).OpenDbConnection())
            {
                using (IDbCommand dbCmd = db.CreateCommand())
                {
                    call(dbCmd, ev);
                }
            }
        }
    }
}
