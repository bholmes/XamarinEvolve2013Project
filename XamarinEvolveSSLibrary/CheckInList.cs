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

		private void CopyTo (List<CheckIn> list)
		{
			foreach (CheckIn checkIn in _list)
			{
				list.Add (checkIn);
			}
		}
		
		public CheckInList Copy ()
		{
			List<CheckIn> list = new List<CheckIn> ();
			CopyTo (list);
			
			CheckInList ret = new CheckInList (list);
			
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

		public IEnumerator<CheckIn> GetEnumerator()
		{
			return _list.GetEnumerator ();
		}

		public CheckInList GetRecentCheckIns(int hours)
		{
			List<CheckIn> sortList = new List<CheckIn> ();
			CopyTo (sortList);
			
			sortList.Sort ((a,b) => {
				return b.Time.CompareTo (a.Time);
			});

			TimeSpan maxTimeSpan = new TimeSpan (0, hours, 0, 0, 0);

			for (int i=0;i<sortList.Count; i++)
			{
				TimeSpan ts = DateTime.Now - sortList[i].Time;
				if (ts.CompareTo (maxTimeSpan) > 0)
				{
					sortList.RemoveRange(i, sortList.Count-i);
					break;
				}
			}

			List<CheckIn> distinctList = new List<CheckIn> ();

			foreach (CheckIn checkIn in sortList.Distinct( new CheckInUserNameComparer ()))
			{
				distinctList.Add (checkIn);
			}

			return new CheckInList (distinctList);
		}

		public void GetCheckInsForPlace (Place place, int maxHoursForActive, 
		                                 out CheckInList activeList, out CheckInList recentList)
		{
			List<CheckIn> sortList = new List<CheckIn> ();
			HashSet <string> hash = new HashSet<string> ();

			sortList.Sort ((a,b) => {
				return b.Time.CompareTo (a.Time);
			});

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

				if(!maxTimeMet && !hash.Contains(checkIn.UserName))
				{
					hash.Add(checkIn.UserName);

					if (checkIn.PlaceId == place.Id)
						t_activeList.Add (checkIn);
				}
				else
				{
					if (checkIn.PlaceId == place.Id)
						t_recentList.Add (checkIn);
				}
			}

			activeList = new CheckInList (t_activeList);
			recentList = new CheckInList (t_recentList);
		}

		class CheckInUserNameComparer : IEqualityComparer<CheckIn>
		{
			public bool Equals(CheckIn x, CheckIn y)
			{
				return x.UserName.Equals(y.UserName);
			}
			
			public int GetHashCode(CheckIn obj)
			{
				return obj.UserName.GetHashCode();
			}
		}
	}
}

