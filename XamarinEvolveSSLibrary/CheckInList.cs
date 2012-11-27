using System;
using System.Collections.Generic;
using System.Linq;

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

		public List<CheckIn> CheckIns {get {return _list;}}
		
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

