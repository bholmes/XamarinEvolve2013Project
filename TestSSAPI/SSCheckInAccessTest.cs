using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.ServiceClient.Web;
using XamarinEvolveSSLibrary;
using ServiceStack.Common.ServiceClient.Web;


namespace TestSSAPI
{
    public class SSCheckInAccessTest
    {
        public void RunTests()
        {
            using (JsonServiceClient client = new JsonServiceClient(SystemConstants.WebServiceBaseURL))
            {
                var users = new string[] { "checkInUser1", "checkInUser2", "checkInUser3", "checkInUser4", "checkInUser5"};
                var places = new Place[] { 
                    new Place { Name = "Home", Address="555 street"},
                    new Place { Name = "Home", Address="999 street"},
                    new Place { Name = "Work", Address="123 street"},
                    new Place { Name = "Bar", Address="456 street"},
                    new Place { Name = "Giant Eagle", Address="456 street"},
                };
                AddUsers(client, users);

                try
                {
                    CheckInUser(client, users[0], places[0]);
                    CheckInUser(client, users[0], places[2]);
                    CheckInUser(client, users[0], places[1]);
                    CheckInUser(client, users[0], places[0]);
                }
                finally
                {
                    DeleteUsers(client, users);
                }
            }
        }

        void CheckInUser(JsonServiceClient client, string userName, Place place)
        {
            LoginUser(client, userName);

            try
            {
                var request = new CheckInRequest { Place = place };
                client.Put<CheckInRequestResponse>("CheckIns", request);
            }
            finally
            {
                LogoutUser(client);
            }
        }

        void AddUsers(JsonServiceClient client, string[] users)
        {
            foreach (string userName in users)
            {
                User user = new User { UserName = userName, Password = MD5Helper.CalculateMD5Hash("password123") };
                UserResponse response = client.Put<XamarinEvolveSSLibrary.UserResponse>("User", user);
            }
        }

        void LoginUser(JsonServiceClient client, string userName)
        {
            Auth auth = new Auth()
            {
                UserName = userName,
                Password = MD5Helper.CalculateMD5Hash("password123"),
                RememberMe = false,
                provider = "credentials"
            };
            try
            {
                client.Post<AuthResponse>("Auth", auth);
            }
            catch (Exception)
            {
                throw new UserAuthenticationException(string.Format("Could not login {0}", userName));
            }
        }

        void LogoutUser(JsonServiceClient client)
        {
            Auth auth = new Auth()
            {
                provider = "logout"
            };
            try
            {
                client.Post<AuthResponse>("Auth", auth);
            }
            catch (Exception)
            {
                throw new UserAuthenticationException(string.Format("Could not logout"));
            }
        }

        void DeleteUsers(JsonServiceClient client, string[] users)
        {
            foreach (string userName in users)
            {
                LoginUser(client, userName);
                client.Delete<XamarinEvolveSSLibrary.UserResponse>(string.Format ("User?UserName={0}", userName));
                LogoutUser(client);
            }
        }
    }
}
