using System;
using ServiceStack.ServiceHost;

namespace XamarinEvolveSSLibrary
{
    [Route("/CheckIns")]
    public class CheckInRequest
    {
        public Place Place { get; set; }
        public int PlaceId { get; set; }
        public int RecentLimit { get; set; }
    }
}
