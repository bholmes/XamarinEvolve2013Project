using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using ServiceStack.ServiceClient.Web;
using XamarinEvolveSSLibrary;

namespace TestSSAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            SSTests ssTests = new SSTests();
            ssTests.RunTests();

            //AvatarTest aTests = new AvatarTest();
            //aTests.RunTests();
        }
    }
}
