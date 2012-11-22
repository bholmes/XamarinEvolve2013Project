using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceClient.Web;
using XamarinEvolveSSLibrary;

namespace TestSSAPI
{
    public class MySqlOnlyTests
    {
        public void RunTests()
        {
            
        }
    }

    public class SSTests
    {
        public void RunTests()
        {
            JsonServiceClient client = new JsonServiceClient(SystemConstants.WebServiceBaseURL);

            UserResponse response = client.Get<XamarinEvolveSSLibrary.UserResponse>("users");
            Console.WriteLine(response.Users.Count);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            SSTests ssTests = new SSTests();
            ssTests.RunTests();
        }
    }
}
