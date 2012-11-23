using System;
using ServiceStack.ServiceHost;


namespace XamarinEvolveSSLibrary
{
    [Route("/useravatar")]
    [Route("/useravatar/{username}/")]
    [Route("/useravatar/{username}/{size}")]
    public class UserAvatar
    {
        public string username { get; set; }
        public int size { get; set; }
        public byte[] data { get; set; }
    }
}
