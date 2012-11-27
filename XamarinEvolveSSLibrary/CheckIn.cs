using System;
using ServiceStack.DataAnnotations;

namespace XamarinEvolveSSLibrary
{
    [Alias("evolve_checkins")]
	public class CheckIn
	{
		public int Id {get;set;}
		public int PlaceId {get;set;}
		public DateTime Time {get;set;}
		public int UserId {get;set;}
	}
}

