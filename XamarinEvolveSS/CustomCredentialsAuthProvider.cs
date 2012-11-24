using System;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface;
using System.Collections.Generic;
using XamarinEvolveSSLibrary;

namespace XamarinEvolveSS
{
    public class CustomCredentialsAuthProvider : CredentialsAuthProvider
    {
        public override bool TryAuthenticate(IServiceBase authService, string userName, string password)
        {
            try
            {
                EvolveUsersMySqlAccess sql = new EvolveUsersMySqlAccess();

                User user = sql.FindUser(userName);
                if (user != null && user.Password == password)
                    return true;
                
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public override void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, Dictionary<string, string> authInfo)
        {
            session.UserName = session.UserAuthName;

            //Important: You need to save the session!
            authService.SaveSession(session, SessionExpiry);
        }
    }
}