using System;
using XamarinEvolveSSLibrary;
using ServiceStack.ServiceClient.Web;
using System.Net;
using System.IO;
using System.Text;
 
// notes
// this.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;

namespace TestSSAPI
{
    public class AuthTest
    {
        private CookieContainer Login(string username, string passHash)
        {
            CookieContainer cookieJar = new CookieContainer();
            string authURI = string.Format("{0}/json/syncreply/Auth", SystemConstants.WebServiceBaseURL);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(authURI);
            request.CookieContainer = cookieJar;
            request.ContentType = "application/json";
            request.Method = "POST";
            using (StreamWriter sw = new StreamWriter(request.GetRequestStream(), new UTF8Encoding()))
            {
                string requestText = string.Format("{{\"provider\":\"credentials\",\"UserName\":\"{0}\",\"Password\":\"{1}\",\"RememberMe\":false}}",
                    username, passHash);

                sw.Write(requestText);
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.WriteLine("Bad Status Code");

                foreach (Cookie c in cookieJar.GetCookies(request.RequestUri))
                {
                    Console.WriteLine("cookieJar.Add(new Cookie(\"{0}\", \"{1}\", \"/\", SystemConstants.WebServiceDomain));",
                        c.Name, c.Value);
                }
            }

            return cookieJar;
        }

        private CookieContainer Logout(CookieContainer cookieJar)
        {
            string authURI = string.Format("{0}/json/syncreply/Auth", SystemConstants.WebServiceBaseURL);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(authURI);
            request.CookieContainer = cookieJar;
            request.ContentType = "application/json";
            request.Method = "POST";
            using (StreamWriter sw = new StreamWriter(request.GetRequestStream(), new UTF8Encoding()))
            {
                string requestText = string.Format("{{\"provider\":\"logout\"}}");

                sw.Write(requestText);
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.WriteLine("Bad Status Code");

                foreach (Cookie c in cookieJar.GetCookies(request.RequestUri))
                {
                    Console.WriteLine("cookieJar.Add(new Cookie(\"{0}\", \"{1}\", \"/\", SystemConstants.WebServiceDomain));",
                        c.Name, c.Value);
                }
            }

            return cookieJar;
        }

        public void RunTests()
        {
            string username = "passuser";
            string passHash = MD5Helper.CalculateMD5Hash("password");

            JsonServiceClient client = new JsonServiceClient(SystemConstants.WebServiceBaseURL);
            client.CookieContainer = Login(username, passHash);

            client.Get<XamarinEvolveSSLibrary.UserResponse>("User");

            Logout(client.CookieContainer);

            client.Get<XamarinEvolveSSLibrary.UserResponse>("User");

            client.CookieContainer = Login(username, passHash);

            User user = new User()
            {
                UserName = username,
                City = "changedtown",
                Email = "new@address.com"
            };

            client.Post<XamarinEvolveSSLibrary.UserResponse>("User", user);

            Logout(client.CookieContainer);

            client.Post<XamarinEvolveSSLibrary.UserResponse>("User", user);
        }
    }
}
