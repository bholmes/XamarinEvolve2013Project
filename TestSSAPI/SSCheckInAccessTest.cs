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
                    places[0].Id = GetCheckInLastPlace(client, places[0]);

                    CheckInUser(client, users[0], places[2]);
                    places[2].Id = GetCheckInLastPlace(client, places[2]);

                    CheckInUser(client, users[0], places[1]);
                    places[1].Id = GetCheckInLastPlace(client, places[1]);

                    CheckInUser(client, users[0], places[0]);
                    places[0].Id = GetCheckInLastPlace(client, places[0]);



                    CheckInUser(client, users[1], places[4]);
                    places[4].Id = GetCheckInLastPlace(client, places[4]);

                    CheckInUser(client, users[2], places[2]);
                    places[2].Id = GetCheckInLastPlace(client, places[2]);

                    CheckInUser(client, users[3], places[3]);
                    places[3].Id = GetCheckInLastPlace(client, places[3]);

                    CheckInUser(client, users[3], places[0]);
                    places[0].Id = GetCheckInLastPlace(client, places[0]);


                    places.Select(p => { GetCheckInsForPlace(client, p, true); return 1; }).ToArray();

                    GetRecentPlaceList(client);
                    GetPopularPlaceList(client);
                }
                finally
                {
                    DeleteUsers(client, users);
                }
            }
        }

        private void GetPopularPlaceList(JsonServiceClient client)
        {
            var response = client.Get<PlacesRequestResponse>(string.Format("Places/?Method=Popular&Limit=10"));

            Console.WriteLine("Entire Popular List");
            if (response != null && response.Places != null)
            {
                foreach (Place place in response.Places)
                {
                    Console.WriteLine("{0} at {1}", place.Name, place.Address);
                }
            }

            Console.WriteLine("\n");

            response = client.Get<PlacesRequestResponse>(string.Format("Places/?Method=Popular&Limit=3"));

            Console.WriteLine("Top 3 Popular List");

            if (response != null && response.Places != null)
            {
                foreach (Place place in response.Places)
                {
                    Console.WriteLine("{0} at {1}", place.Name, place.Address);
                }
            }

            Console.WriteLine("\n");
        }

        private void GetRecentPlaceList(JsonServiceClient client)
        {
            var response = client.Get<PlacesRequestResponse>(string.Format("Places/?Method=Recent&Limit=10"));

            Console.WriteLine("Entire Recent List");
            if (response != null && response.Places != null)
            {
                foreach (Place place in response.Places)
                {
                    Console.WriteLine("{0} at {1}", place.Name, place.Address);
                }
            }

            Console.WriteLine("\n");

            response = client.Get<PlacesRequestResponse>(string.Format("Places/?Method=Recent&Limit=3"));

            Console.WriteLine("Top 3 Recent List");

            if (response != null && response.Places != null)
            {
                foreach (Place place in response.Places)
                {
                    Console.WriteLine("{0} at {1}", place.Name, place.Address);
                }
            }

            Console.WriteLine("\n");
        }

        private void GetCheckInsForPlace(JsonServiceClient client, Place place, bool dump)
        {
            var result = client.Get<CheckInRequestResponse>(string.Format("CheckIns/?PlaceId={0}", place.Id));

            if (!dump)
                return;
            Console.WriteLine("Active Users for palce '{0}' at {1}", place.Name, place.Address);
            foreach (CheckInUserPair pair in result.ActivePairList)
            {
                Console.WriteLine("     '{0}'", pair.User.UserName);
            }
            Console.WriteLine("\n");

            Console.WriteLine("Recent Users for palce '{0}' at {1}", place.Name, place.Address);
            foreach (CheckInUserPair pair in result.RecentPairList)
            {
                Console.WriteLine("     '{0}'", pair.User.UserName);
            }

            Console.WriteLine("\n");
        }

        private int GetCheckInLastPlace(JsonServiceClient client, Place shoudBe)
        {
            var response = client.Get<PlacesRequestResponse>(string.Format("Places/?Method=Recent&Limit=1"));

            if (response != null && response.Places != null && response.Places.Count() == 1)
            {
                Console.WriteLine("{0} should be {1} at {2}", response.Places[0].Id, shoudBe.Name, shoudBe.Address);
                return response.Places[0].Id;
            }
            throw new Exception("Did not get the Id back");
        }

        void CheckInUser(JsonServiceClient client, string userName, Place place)
        {
            LoginUser(client, userName);

            try
            {
                var request = new CheckInRequest { Place = place };
                client.Put<CheckInRequestResponse>("CheckIns", request);
                System.Threading.Thread.Sleep(1000);
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
