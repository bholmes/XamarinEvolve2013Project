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
                throw new ArgumentException("UserName is not valid");

            List<User> result = null;

            SetupAndCall((dbCmd, ev) =>
            {
                ev.Where(rn => username == rn.UserName);
                result = dbCmd.Select(ev);
            });

            if (result == null || result.Count == 0)
                throw new Exception(string.Format("User '{0}' not found", username));
            else if (result.Count > 1)
                throw new Exception(string.Format("Too many Users named '{0}' found'", username));

            return result[0];
        }

        public void DeleteUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is not valid");

            int result = 1;

            SetupAndCall((dbCmd, ev) =>
            {
                ev.Where(rn => username == rn.UserName);
                result = dbCmd.Delete(ev);
            });

            if (result != 1)
                throw new Exception(string.Format("User '{0}' not found", username));
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
                if (user.FullName != null)
                    updateStrs.Add("FullName");
                if (user.City != null)
                    updateStrs.Add("City");
                if (user.Email != null)
                    updateStrs.Add("Email");
                if (user.Phone != null)
                    updateStrs.Add("Phone");
                if (user.Password != null)
                    updateStrs.Add("Password");
                if (user.Avatar != null)
                    updateStrs.Add("Avatar");
                if (user.Company != null)
                    updateStrs.Add("Company");
                if (user.Title != null)
                    updateStrs.Add("Title");

                if (updateStrs.Count == 0)
                    return;

                ev.Where(rn => user.UserName == rn.UserName).UpdateFields = updateStrs;
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
