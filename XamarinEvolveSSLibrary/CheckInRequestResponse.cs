using System;
using System.Collections.Generic;


namespace XamarinEvolveSSLibrary
{
    public class CheckInRequestResponse
    {
        public Exception Exception { get; set; }
        public List<CheckInUserPair> ActivePairList {get;set;}
        public List<CheckInUserPair> RecentPairList { get; set; }
    }
}
