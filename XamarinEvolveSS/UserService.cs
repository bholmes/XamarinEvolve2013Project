using System;
using System.Collections.Generic;
using ServiceStack.ServiceInterface;
using XamarinEvolveSSLibrary;

namespace XamarinEvolveSS
{
    public class UserService : RestServiceBase<User>
    {
        public override object OnGet(User request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.username))
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
                User user = sql.FindUser(request.username);
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
                EvolveUsersMySqlAccess sql = new EvolveUsersMySqlAccess();

                if (request == null || string.IsNullOrWhiteSpace(request.username))
                    throw new ArgumentException("username not specified");

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

                if (request == null || string.IsNullOrWhiteSpace(request.username))
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
            EvolveUsersMySqlAccess sql = new EvolveUsersMySqlAccess();

            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.username))
                    throw new ArgumentException("username not specified");

                sql.DeleteUser(request.username);
            }
            catch (Exception ex)
            {
                return new UserResponse() { Exception = ex };
            }

            return new UserResponse ();
        }
    }
}
