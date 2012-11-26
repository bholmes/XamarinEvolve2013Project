using System;
using System.Collections.Generic;

namespace XamarinEvolveSSLibrary
{
	public class PlaceList
	{
		List<Place> _list;
		
		public PlaceList ()
		{
			_list = new List<Place> ();
		}
		
		public PlaceList (List<Place> list)
		{
			_list = list;
		}

		private void CopyTo (List<Place> list)
		{
			foreach (Place place in _list)
			{
				list.Add (place);
			}
		}

		private void CopyTo (List<Place> list, int limit)
		{
			int count = 0;

			foreach (Place place in _list)
			{
				if (++count > limit)
					break;

				list.Add (place);
			}
		}

		public PlaceList Copy ()
		{
			List<Place> list = new List<Place> ();
			CopyTo (list);

			PlaceList ret = new PlaceList (list);

			return ret;
		}

		public PlaceList Copy (int limit)
		{
			List<Place> list = new List<Place> ();
			CopyTo (list, limit);
			
			PlaceList ret = new PlaceList (list);
			
			return ret;
		}
		
		public Place this[int index]
		{
			get {
				return _list[index];
			}
		}
		
		public Place AddIfNew (Place newPlace)
		{
			Place returnPlace = _list.Find (p => p.Name == newPlace.Name && p.Address == newPlace.Address);

			if (returnPlace != null)
				return returnPlace;

			if (_list.Count > 0)
				newPlace.Id = _list[_list.Count-1].Id + 1;
			else
				newPlace.Id = 1;

			_list.Add (newPlace);
			return newPlace;
		}
		
		public void Delete (Place palceToDelete)
		{
			_list.Remove (_list.Find (e=> e.Id == palceToDelete.Id));
		}
		
		public int Count {get{return _list.Count;}}

		public IEnumerator<Place> GetEnumerator()
		{
			return _list.GetEnumerator ();
		}

		public PlaceList SortByPopularity ()
		{
			List<Place> sortList = new List<Place> ();
			CopyTo (sortList);

			sortList.Sort ((a,b) => {
				return a.NumberOfCheckIns == b.NumberOfCheckIns ? 
					0 : a.NumberOfCheckIns < b.NumberOfCheckIns ? 
						-1 : 1;
			});

			return new PlaceList (sortList);
		}

		public PlaceList SortByDistance (float lat, float lng)
		{
			List <DistanceSortHelper> dList = new List<DistanceSortHelper> ();

			foreach (Place place in _list)
			{
				dList.Add (new DistanceSortHelper (place, lat, lng));
			}

			dList.Sort ((a,b) => {
				return a.Distance < b.Distance ? -1 : 1;
			});

			List<Place> sortList = new List<Place> ();

			foreach (DistanceSortHelper dHelper in dList)
			{
				sortList.Add (dHelper.Place);
			}
			
			return new PlaceList (sortList);
		}

		private class DistanceSortHelper
		{
			public Place Place {get;private set;}
			public float Distance {get;private set;}
			public DistanceSortHelper (Place place, float lat, float lng)
			{
				Place = place;
				Distance = place.DistanceFrom (lat, lng);
			}
		}

		public PlaceList SortByRecentCheckIns (CheckInList checkInList) 
		{
			List<Place> sortList = new List<Place> ();
			var hash = new HashSet<int>();

			foreach (CheckIn checkin in checkInList)
			{
				if(!hash.Contains(checkin.PlaceId))
				{
					hash.Add(checkin.PlaceId);

					Place place = _list.Find (p => p.Id == checkin.PlaceId);
					if (place != null)
						sortList.Add (place);
				}
			}
			
			return new PlaceList (sortList);
		}
	}
}

