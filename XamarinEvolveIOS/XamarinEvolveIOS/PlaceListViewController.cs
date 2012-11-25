using System;
using MonoTouch.UIKit;
using XamarinEvolveSSLibrary;

namespace XamarinEvolveIOS
{
	public class PlaceListViewController : UIViewController
	{
		protected GeolocationHelper GeolocationHelper {get;set;}
		
		public UITableView TableView {get;set;}
		public BusyView BusyView {get;set;}
		
		public PlaceListViewController ()
		{
			PlaceList = new PlaceList ();
			GeolocationHelper = new GeolocationHelper ();
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			TableView = new UITableView (this.View.Bounds, UITableViewStyle.Grouped);
			TableView.AutoresizingMask = UIViewAutoresizing.All;
			this.View.Add (TableView);
			
			BusyView = new BusyView (View.Bounds);
			this.View.Add (BusyView);
		}
		
		protected void ShowLocatinError (PlaceList placeList, GeolocationResult result)
		{
			string message;
			
			if (result.Canceled)
				message = "The location request timed out.";
			else if (result.GeolocationNotAvailable)
				message = "Location services are not available on this device.";
			else if (result.GeolocationDisabled)
				message = "Location services are not enabled for the Evolve application.";
			else 
				message = "Did not find any places near your location.";
			
			BusyView.Busy = false;
			
			UIAlertView alertNew = new UIAlertView ("Error", message, null, "OK", null);
			alertNew.Show ();
		}
		
		protected class PlaceListViewDataSource : UITableViewDataSource
		{
			public PlaceList PlaceList {get;set;}
			
			public PlaceListViewDataSource (PlaceListViewController viewController)
			{
				PlaceList = new PlaceList ();
			}
			
			#region implemented abstract members of UITableViewDataSource
			public override int RowsInSection (UITableView tableView, int section)
			{
				return PlaceList.Count;
			}			
			
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell ("PlaceTableItem");
				
				if (cell == null)
				{
					cell = new UITableViewCell (UITableViewCellStyle.Subtitle, "PlaceTableItem");
				}
				
				Place place = PlaceList[indexPath.Row];
				cell.TextLabel.Text = place.Name;
				cell.DetailTextLabel.Text = place.Address;
				
				return cell;
			}		
#endregion
		}
		
		private PlaceList _placeList = new PlaceList ();
		
		protected PlaceList PlaceList 
		{
			get
			{
				return _placeList;
			}
			set
			{
				_placeList = value;
				
				if (TableView == null)
					return;
				
				if (TableView.DataSource != null)
					((PlaceListViewDataSource)(TableView.DataSource)).PlaceList = value;
				if (TableView.Delegate != null)
					((PlaceListViewDelegate)(TableView.Delegate)).PlaceList = value;
			}
		}
		
		protected class PlaceListViewDelegate : UITableViewDelegate
		{
			public PlaceList PlaceList {get;set;}
			
			public PlaceListViewDelegate (PlaceListViewController viewController)
			{
				PlaceList = new PlaceList ();
			}
		}
	}
}

