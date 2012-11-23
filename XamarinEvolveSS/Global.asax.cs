using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface;
using ServiceStack.WebHost.Endpoints;
using Funq;
using ServiceStack.Configuration;

namespace XamarinEvolveSS
{
    public class Global : System.Web.HttpApplication
    {
        public class Evolve2013AppHost : AppHostBase
        {
            //Tell Service Stack the name of your application and where to find your web services
            public Evolve2013AppHost() : base("Evolve 2013 Web Services", typeof(UserService).Assembly) { }

            public override void Configure(Container container)
            {
                var appSettings = new AppSettings();

                //Plugins.Add(new AuthFeature(() => new AuthUserSession(), new IAuthProvider[] {
                //   new CustomCredentialsAuthProvider()
                //}));

                //Routes
                //  .Add<XamarinEvolveSSLibrary.User>("/user")
                //  .Add<XamarinEvolveSSLibrary.User>("/user/{username}");
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            new Evolve2013AppHost().Init();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }

    public class CustomCredentialsAuthProvider : CredentialsAuthProvider
    {
        public override bool TryAuthenticate(IServiceBase authService, string userName, string password)
        {
            return true;
        }

        public override void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, Dictionary<string, string> authInfo)
        {
            //Fill the IAuthSession with data which you want to retrieve in the app eg:
            session.FirstName = "some_firstname_from_db";
            //...

            //Important: You need to save the session!
            authService.SaveSession(session, SessionExpiry);
        }
    }
}