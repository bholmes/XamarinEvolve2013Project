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
            EvolveUsersMySqlAccess sql = new EvolveUsersMySqlAccess();
            List<User> users = sql.GetAllUsers();
            return new UserResponse() { Users = users };
        }
    }
}
