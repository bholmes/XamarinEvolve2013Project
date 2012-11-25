using System;
using System.Collections.Generic;

namespace XamarinEvolveSSLibrary
{
	public class CheckInList
	{
		List<CheckIn> _list;
		
		public CheckInList ()
		{
			_list = new List<CheckIn> ();
		}
		
		public CheckInList (List<CheckIn> list)
		{
			_list = list;
		}

		public CheckInList Copy ()
		{
			CheckInList ret = new CheckInList ();
			
			foreach (CheckIn checkIn in _list)
			{
				ret._list.Add (checkIn);
			}
			
			return ret;
		}
		
		public CheckIn this[int index]
		{
			get {
				return _list[index];
			}
		}
		
		public CheckIn Add (CheckIn newCheckIn)
		{
			if (_list.Count > 0)
				newCheckIn.Id = _list[_list.Count-1].Id + 1;
			else
				newCheckIn.Id = 1;

			_list.Add (newCheckIn);
			return newCheckIn;
		}
		
		public void Delete (CheckIn checkInToDelete)
		{
			_list.Remove (_list.Find (e=> e.Id == checkInToDelete.Id));
		}
		
		public int Count {get{return _list.Count;}}
	}
}

