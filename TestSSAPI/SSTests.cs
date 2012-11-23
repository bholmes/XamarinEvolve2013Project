using System;
using ServiceStack.ServiceClient.Web;
using XamarinEvolveSSLibrary;


namespace TestSSAPI
{
    public class SSTests
    {
        public void RunTests()
        {
            JsonServiceClient client = new JsonServiceClient(SystemConstants.WebServiceBaseURL);
            Test1(client);
            Test2(client);
            Test3(client);
            Test4(client);
            Test5(client);
            Test6(client);
            Test7(client);
            Test8(client);
            Test9(client);
            Test10(client);
            Test11(client);
            Test12(client);
        }

        void Test1(JsonServiceClient client)
        {
            Console.WriteLine("~~~~~ AddUser (newuser2) ~~~~~~~~~");

            User user = new User()
            {
                UserName = "newuser2",
                FullName = "Awesome guy",
                City = "funkytown",
                Email = "fun@hello.net",
                Phone = "8675309",
                Password = "pass",
                Avatar = "avatar.jpg",
                Title = "the man",
                Company = "three's",
            };

            UserResponse response = client.Put<XamarinEvolveSSLibrary.UserResponse>("User", user);
            Console.WriteLine();
        }

        void Test2(JsonServiceClient client)
        {
            Console.WriteLine("~~~~~ GetAllUsers () ~~~~~~~~~");

            UserResponse response = client.Get<XamarinEvolveSSLibrary.UserResponse>("User");

            foreach (User user in response.Users)
            {
                Console.WriteLine(user);
            }
            Console.WriteLine();
        }

        void Test3(JsonServiceClient client)
        {
            Console.WriteLine("~~~~~ FindUser (billholmes) ~~~~~~~~~");

            UserResponse response = client.Get<XamarinEvolveSSLibrary.UserResponse>("User/billholmes");

            User user = response.Users[0];
            Console.WriteLine(user);
            Console.WriteLine();
        }

        void Test4(JsonServiceClient client)
        {
            Console.WriteLine("~~~~~ UpdateUser (newuser2) ~~~~~~~~~");

            User user = new User()
            {
                UserName = "newuser2",
                City = "changedtown",
                Email = "new@address.com"
            };

            UserResponse response = client.Post<XamarinEvolveSSLibrary.UserResponse>("User", user);
            Console.WriteLine();
        }

        void Test5(JsonServiceClient client)
        {
            Console.WriteLine("~~~~~ FindUser (newuser2) ~~~~~~~~~");

            UserResponse response = client.Get<XamarinEvolveSSLibrary.UserResponse>("User/newuser2");

            User user = response.Users[0];
            Console.WriteLine(user);
            Console.WriteLine();
        }

        void Test6(JsonServiceClient client)
        {
            Console.WriteLine("~~~~~ DeleteUser (newuser2) ~~~~~~~~~");
            UserResponse response = client.Delete<XamarinEvolveSSLibrary.UserResponse>("User/newuser2");
            Console.WriteLine();
        }

        void Test7(JsonServiceClient client)
        {
            Console.WriteLine("~~~~~ FindUser (newuser2) ~~~~~~~~~");
            UserResponse response = client.Get<XamarinEvolveSSLibrary.UserResponse>("User/newuser2");
            Console.WriteLine("Expected : " + response.Exception);
            Console.WriteLine();
        }

        void Test8(JsonServiceClient client)
        {
            Console.WriteLine("~~~~~ DeleteUser (newuser2) ~~~~~~~~~");
            UserResponse response = client.Delete<XamarinEvolveSSLibrary.UserResponse>("User/newuser2");
            Console.WriteLine("Expected : " + response.Exception);
            Console.WriteLine();
        }

        void Test9(JsonServiceClient client)
        {
            Console.WriteLine("~~~~~ DeleteUser (newuser2) ~~~~~~~~~");
            UserResponse response = client.Delete<XamarinEvolveSSLibrary.UserResponse>("User");
            Console.WriteLine("Expected : " + response.Exception);
            Console.WriteLine();
        }

        void Test10(JsonServiceClient client)
        {
            Console.WriteLine("~~~~~ AddUser (newuser3) ~~~~~~~~~");

            User user = new User()
            {
                UserName = "newuser3"
            };

            UserResponse response = client.Put<XamarinEvolveSSLibrary.UserResponse>("User", user);
            Console.WriteLine();
        }

        void Test11(JsonServiceClient client)
        {
            Console.WriteLine("~~~~~ FindUser (newuser3) ~~~~~~~~~");
            UserResponse response = client.Get<XamarinEvolveSSLibrary.UserResponse>("User/newuser3");
            Console.WriteLine("Expected null: " + response.Exception);
            Console.WriteLine();
        }

        void Test12(JsonServiceClient client)
        {
            Console.WriteLine("~~~~~ DeleteUser (newuser3) ~~~~~~~~~");
            UserResponse response = client.Delete<XamarinEvolveSSLibrary.UserResponse>("User/newuser3");
            Console.WriteLine("Expected null: " + response.Exception);
            Console.WriteLine();
        }
    }
}
