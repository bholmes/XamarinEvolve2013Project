using System;
using System.Collections.Generic;
using ServiceStack.ServiceHost;

namespace XamarinEvolveSSLibrary
{
    [Route("/Places")]
    public class PlacesRequest
    {
        public string Method {get;set;}
        public int Limit { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
