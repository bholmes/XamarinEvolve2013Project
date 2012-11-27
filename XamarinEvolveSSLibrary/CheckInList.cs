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

		public void GetCheckInsForPlace (Place place, int maxHoursForActive, 
		                                 out CheckInList activeList, out CheckInList recentList)
		{
			HashSet <string> activeHash = new HashSet<string> ();
			HashSet <string> totalHash = new HashSet<string> ();

			var sortList = _list.OrderByDescending (c=> c.Time);

			List<CheckIn> t_activeList = new List<CheckIn> ();
			List<CheckIn> t_recentList = new List<CheckIn> ();

			TimeSpan maxTimeSpan = new TimeSpan (0, maxHoursForActive, 0, 0, 0);
			bool maxTimeMet = false;

			foreach (CheckIn checkIn in sortList)
			{
				if (!maxTimeMet)
				{	
					TimeSpan ts = DateTime.Now - checkIn.Time;
					if (ts.CompareTo (maxTimeSpan) > 0)
					{
						maxTimeMet = true;
					}
				}

				if (checkIn.PlaceId == place.Id)
				{
					if (!maxTimeMet && !activeHash.Contains(checkIn.UserName))
					{
						t_activeList.Add (checkIn);
						totalHash.Add (checkIn.UserName);
					}
					else if (!totalHash.Contains(checkIn.UserName))
					{
						t_recentList.Add (checkIn);
						totalHash.Add (checkIn.UserName);
					}
				}
				   
				activeHash.Add (checkIn.UserName);
			}

			activeList = new CheckInList (t_activeList);
			recentList = new CheckInList (t_recentList);
		}
	}
}

