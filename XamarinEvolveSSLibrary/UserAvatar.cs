using System;
using ServiceStack.ServiceHost;


namespace XamarinEvolveSSLibrary
{
#if !IOS
    [Route("/useravatar")]
    [Route("/useravatar/{username}/")]
    [Route("/useravatar/{username}/{size}")]
#endif
    public class UserAvatar
    {
        public string username { get; set; }
        public int size { get; set; }
        public byte[] data { get; set; }
    }
}
