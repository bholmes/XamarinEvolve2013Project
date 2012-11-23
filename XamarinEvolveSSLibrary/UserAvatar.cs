using System;
using ServiceStack.ServiceHost;


namespace XamarinEvolveSSLibrary
{
    [Route("/UserAvatar")]
    [Route("/UserAvatar/{UserName}/")]
    [Route("/UserAvatar/{UserName}/{Size}")]
    public class UserAvatar
    {
        public string UserName { get; set; }
        public int Size { get; set; }
        public byte[] Data { get; set; }
    }
}
