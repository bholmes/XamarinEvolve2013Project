using System;
using MonoTouch.UIKit;
using XamarinEvolveSSLibrary;

namespace XamarinEvolveIOS
{
	public class PlaceInfoViewController : UIViewController
	{
		public UITableView TableView { get; private set;}
		public Place Place { get; private set;}
		public CheckInList ActiveCheckInList {get; private set;}
		public CheckInList RecentCheckInList {get;private set;}

		public BusyView BusyView {get; private set;}

		public PlaceInfoViewController (Place place)
		{
			this.Title = "Place Info";
			Place = place;
			ActiveCheckInList = new CheckInList ();
			RecentCheckInList = new CheckInList ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			BusyView = new XamarinEvolveIOS.BusyView (this.View.Bounds);

			NavigationItem.BackBarButtonItem =  new UIBarButtonItem (
				"Back", UIBarButtonItemStyle.Bordered, null, null);

			TableView = new UITableView (View.Bounds, UITableViewStyle.Grouped);
			TableView.DataSource = new PlaceInfoViewDataSource (this);
			TableView.Delegate = new PlaceInfoViewDelegate (this);

			View.Add (TableView);

			this.View.Add (BusyView);
			BusyView.Busy = true;

			LoadPlaceCheckInInfo ();
		}

		private void LoadPlaceCheckInInfo ()
		{
			Func<int> func = delegate {

				CheckInList recentList;
				CheckInList activeList;

				Engine.Instance.CheckInAccess.GetCheckinInfo (Place, out activeList, 
				                                              out recentList, SystemConstants.MaxPlacesPerRequest);

				BeginInvokeOnMainThread (delegate {
					RecentCheckInList = recentList;
					ActiveCheckInList = activeList;
					BusyView.Busy = false;
					TableView.ReloadData ();
				});

				return 0;
			};
			func.BeginInvoke (null, null);
		}

		public bool HasActiveCheckIns
		{
			get
			{
				if (ActiveCheckInList.Count > 0)
					return true;

				return false;
			}
		}

		public bool HasRecentCheckIns
		{
			get
			{
				if (RecentCheckInList.Count > 0)
					return true;

				return false;
			}
		}

		protected class PlaceInfoViewDataSource : UITableViewDataSource
		{
			public PlaceInfoViewController ViewController{get;private set;}

			public PlaceInfoViewDataSource (PlaceInfoViewController viewController)
			{
				ViewController = viewController;
			}

			#region implemented members of UITableViewDataSource

			public override int NumberOfSections (UITableView tableView)
			{
				int count = 1;

				if (ViewController.HasActiveCheckIns)
					count ++;
				if (ViewController.HasRecentCheckIns)
					count ++;

				return count;
			}

			public override int RowsInSection (UITableView tableView, int section)
			{
				switch (section)
				{
				case 0 :
					return 1;
				case 1 :
					if (ViewController.HasActiveCheckIns)
						return ViewController.ActiveCheckInList.Count;
					return ViewController.RecentCheckInList.Count;
				case 2 :
					return ViewController.RecentCheckInList.Count;
				}

				throw new NotImplementedException ();
			}

			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				switch (indexPath.Section)
				{
				case 0 :
					return GetCellForTopSection (tableView, indexPath);
				case 1 :
					if (ViewController.HasActiveCheckIns)
						return GetCellForActiveCheckIns (tableView, indexPath);
					return GetCellForRecentCheckIns (tableView, indexPath);
				case 2 :
					return GetCellForRecentCheckIns (tableView, indexPath);
				}
				
				throw new NotImplementedException ();
			}

			public override string TitleForHeader (UITableView tableView, int section)
			{
				switch (section)
				{
				case 0 :
					return string.Empty;
				case 1 :
					if (ViewController.HasActiveCheckIns)
						return "Active Check-ins";
					return "Recent Check-ins";
				case 2 :
					return "Recent Check-ins";
				}
				
				throw new NotImplementedException ();
			}

			#endregion

			private UITableViewCell GetCellForTopSection (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell ("PlaceInfoViewTopCell");

				if (cell == null)
				{
					cell = new UITableViewCell (UITableViewCellStyle.Subtitle, "PlaceInfoViewTopCell");
				}

				cell.TextLabel.Text = ViewController.Place.Name;
				cell.DetailTextLabel.Text = ViewController.Place.Address;

				return cell;
			}

			private UITableViewCell GetCellForActiveCheckIns (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell ("PlaceInfoViewActiveCheckInCell");
				
				if (cell == null)
				{
					cell = new UITableViewCell (UITableViewCellStyle.Default, "PlaceInfoViewActiveCheckInCell");
				}
				
				cell.TextLabel.Text = ViewController.ActiveCheckInList[indexPath.Row].UserName;
				
				return cell;
			}

			private UITableViewCell GetCellForRecentCheckIns (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell ("PlaceInfoViewRecentCheckInCell");
				
				if (cell == null)
				{
					cell = new UITableViewCell (UITableViewCellStyle.Default, "PlaceInfoViewRecentCheckInCell");
				}
				
				cell.TextLabel.Text = ViewController.RecentCheckInList[indexPath.Row].UserName;
				
				return cell;
			}
		}

		protected class PlaceInfoViewDelegate : UITableViewDelegate
		{
			public PlaceInfoViewController ViewController{get;private set;}

			public PlaceInfoViewDelegate (PlaceInfoViewController viewController)
			{
				ViewController = viewController;
			}
		}
	}
}

