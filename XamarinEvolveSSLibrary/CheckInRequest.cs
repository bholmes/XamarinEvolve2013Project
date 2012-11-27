using System;
using ServiceStack.ServiceHost;

namespace XamarinEvolveSSLibrary
{
    [Route("/CheckIns")]
    public class CheckInRequest
    {
        public Place Place { get; set; }
    }
}
