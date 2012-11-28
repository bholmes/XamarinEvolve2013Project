using System;

namespace XamarinEvolveSSLibrary
{
	public class WebserviceCheckInAccess : CheckInAccess
	{
		private ClientWrapper _clientWrapper;

		public WebserviceCheckInAccess (ClientWrapper clientWrapper)
		{
			_clientWrapper = clientWrapper;
		}

		#region implemented abstract members of CheckInAccess		

		public override PlaceList GetPopularPlaceList (int limit)
		{
			PlacesRequestResponse response;

			lock (_clientWrapper.ClientLock)
			{
				response = _clientWrapper.Client.Get<PlacesRequestResponse>(
					string.Format("Places/?Method=Popular&Limit={0}", limit));
			}
			
			if (response.Places == null || response.Exception != null)
				return new PlaceList ();
			
			return new PlaceList (response.Places);
		}		

		public override PlaceList GetRecentPlaceList (int limit)
		{
			PlacesRequestResponse response;
			
			lock (_clientWrapper.ClientLock)
			{
				response = _clientWrapper.Client.Get<PlacesRequestResponse>(
					string.Format("Places/?Method=Recent&Limit={0}", limit));
			}
			
			if (response.Places == null || response.Exception != null)
				return new PlaceList ();
			
			return new PlaceList (response.Places);
		}

		public override PlaceList GetPlaceListNearLocation (float lat, float lng, int limit)
		{
			PlacesRequestResponse response;
			
			lock (_clientWrapper.ClientLock)
			{
				response = _clientWrapper.Client.Get<PlacesRequestResponse>(
					string.Format(
					"Places/?Method=DistanceFrom&Limit={0}&Latitude={1}&Longitude={2}", 
					limit, lat, lng));
			}
			
			if (response.Places == null || response.Exception != null)
				return new PlaceList ();
			
			return new PlaceList (response.Places);
		}

		public override void GetCheckinInfo (Place place, out System.Collections.Generic.List<CheckInUserPair> activeList, out System.Collections.Generic.List<CheckInUserPair> recentList, int recentLimit)
		{
			CheckInRequestResponse response;
			
			lock (_clientWrapper.ClientLock)
			{
				response = _clientWrapper.Client.Get<CheckInRequestResponse>(
					string.Format("CheckIns/?PlaceId={0}&RecentLimit={1}", place.Id, recentLimit));
			}

			if (response.ActivePairList != null)
				activeList = response.ActivePairList;
			else
				activeList = new System.Collections.Generic.List<CheckInUserPair> ();
			if (response.RecentPairList != null)
				recentList = response.RecentPairList;
			else
				recentList = new System.Collections.Generic.List<CheckInUserPair> ();
		}

		public override void CheckInUserAtPlace (Place place)
		{
			User currentUser = Engine.Instance.UserAccess.GetCurrentUser ();
			if (currentUser.IsAnonymousUser)
				return;
			
			lock (_clientWrapper.ClientLock)
			{
				var request = new CheckInRequest { Place = place };
				_clientWrapper.Client.Put<CheckInRequestResponse>("CheckIns", request);
			}
		}
		#endregion
	}
}

