using System;
using System.Collections.Generic;
using System.Linq;

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

		public List<Place> Places {get {return _list;}}
		
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

		public PlaceList SortByDistance (float lat, float lng, int limit)
		{
			List <DistanceSortHelper> dList = new List<DistanceSortHelper> ();

			foreach (Place place in _list)
			{
				dList.Add (new DistanceSortHelper (place, lat, lng));
			}

			List<Place> sortList = new List<Place> ();

			foreach (DistanceSortHelper dHelper in dList.OrderBy (h=>h.Distance))
			{
				if (sortList.Count < limit)
					sortList.Add (dHelper.Place);
				else
					break;
			}
			
			return new PlaceList (sortList);
		}

		public class DistanceSortHelper
		{
			public Place Place {get;private set;}
			public float Distance {get;private set;}
			public DistanceSortHelper (Place place, float lat, float lng)
			{
				Place = place;
				Distance = place.DistanceFrom (lat, lng);
			}
		}
	}
}

