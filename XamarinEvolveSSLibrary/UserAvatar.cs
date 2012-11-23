using System;
using ServiceStack.ServiceHost;


namespace XamarinEvolveSSLibrary
{
#if !IOS
    [Route("/UserAvatar")]
    [Route("/UserAvatar/{UserName}/")]
    [Route("/UserAvatar/{UserName}/{Size}")]
#endif
    public class UserAvatar
    {
        public string UserName { get; set; }
        public int Size { get; set; }
        public byte[] Data { get; set; }
    }
}
