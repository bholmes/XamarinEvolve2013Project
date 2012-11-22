using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceClient.Web;

namespace TestSSAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new JsonServiceClient("http://localhost:54238");

            var response4 = client.Get<XamarinEvolveSSLibrary.UserResponse>("users");

            Console.WriteLine(response4.Users.Count);
        }
    }
}
