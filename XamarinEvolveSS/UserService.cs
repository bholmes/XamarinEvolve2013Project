using System;
using System.Collections.Generic;
using ServiceStack.ServiceInterface;
using XamarinEvolveSSLibrary;
using ServiceStack.ServiceInterface.Auth;

namespace XamarinEvolveSS
{
    public class UserService : RestServiceBase<User>
    {
        public override object OnGet(User request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName))
            {
                try
                {
                    List<User> users;
                    EvolveUsersMySqlAccess sql = new EvolveUsersMySqlAccess();
                    users = sql.GetAllUsers();
                    return new UserResponse() { Users = users };
                }
                catch (Exception ex)
                {
                    return new UserResponse() { Exception = ex };
                }
            }
            else
            {
                return OnGetSpecificUser(request);
            }
        }

        private object OnGetSpecificUser(User request)
        {
            try
            {
                EvolveUsersMySqlAccess sql = new EvolveUsersMySqlAccess();

                List<User> users = new List<User>();
                User user = sql.FindUser(request.UserName);
                users.Add(user);
                return new UserResponse() { Users = users};
            }
            catch (Exception ex)
            {
                return new UserResponse() { Exception = ex };
            }
        }

        public override object OnPost(User request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.UserName))
                    throw new ArgumentException("username not specified");

                if (!CheckIsAuthorized(request.UserName))
                    return null;

                EvolveUsersMySqlAccess sql = new EvolveUsersMySqlAccess();

                sql.UpdateUser(request);
            }
            catch (Exception ex)
            {
                return new UserResponse() { Exception = ex };
            }

            return new UserResponse();
        }

        public override object OnPut(User request)
        {
            try
            {
                EvolveUsersMySqlAccess sql = new EvolveUsersMySqlAccess();

                if (request == null || string.IsNullOrWhiteSpace(request.UserName))
                    throw new ArgumentException("username not specified");

                sql.AddUser(request);
            }
            catch (Exception ex)
            {
                return new UserResponse() { Exception = ex };
            }

            return new UserResponse();
        }

        public override object OnDelete(User request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.UserName))
                    throw new ArgumentException("username not specified");

                if (!CheckIsAuthorized(request.UserName))
                    return null;

                MySqlCheckInAccess sqlCheckIn = new MySqlCheckInAccess();
                sqlCheckIn.DeleteCheckInsForUser(request.UserName);

                EvolveUsersMySqlAccess sql = new EvolveUsersMySqlAccess();
                sql.DeleteUser(request.UserName);
            }
            catch (Exception ex)
            {
                return new UserResponse() { Exception = ex };
            }

            return new UserResponse ();
        }

        private bool CheckIsAuthorized (string username)
        {
            if (SystemConstants.UseAuthentication)
            {
                IAuthSession auth = this.GetSession();
                if (auth == null || auth.UserName != username)
                {
                    this.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    return false;
                }
            }

            return true;
        }
    }
}
