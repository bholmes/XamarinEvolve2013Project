using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceClient.Web;
using XamarinEvolveSSLibrary;

namespace TestSSAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonServiceClient client = new JsonServiceClient(SystemConstants.WebServiceBaseURL);

            UserResponse response = client.Get<XamarinEvolveSSLibrary.UserResponse>("users");
            Console.WriteLine(response.Users.Count);
        }
    }
}
