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

		public PlaceList Copy ()
		{
			List<Place> list = new List<Place> ();
			CopyTo (list);

			PlaceList ret = new PlaceList (list);

			return ret;
		}
		
		public Place this[int index]
		{
			get {
				return _list[index];
			}
		}
		
		public Place Add (Place newPlace)
		{
			_list.Add (newPlace);
			return newPlace;
		}
		
		public void Delete (Place palceToDelete)
		{
			_list.Remove (_list.Find (e=> e.Id == palceToDelete.Id));
		}
		
		public int Count {get{return _list.Count;}}

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
	}
}

