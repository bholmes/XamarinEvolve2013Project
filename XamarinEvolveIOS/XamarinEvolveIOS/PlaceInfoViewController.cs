using System;
using MonoTouch.UIKit;
using XamarinEvolveSSLibrary;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.CoreLocation;

namespace XamarinEvolveIOS
{
	public class PlaceInfoViewController : UIViewController
	{
		public UITableView TableView { get; private set;}
		public Place Place { get; private set;}
		public List<CheckInUserPair> ActiveCheckInList {get; private set;}
		public List<CheckInUserPair> RecentCheckInList {get;private set;}

		public BusyView BusyView {get; private set;}

		public PlaceInfoViewController (Place place)
		{
			this.Title = "Place Info";
			Place = place;
			ActiveCheckInList = new List<CheckInUserPair> ();
			RecentCheckInList = new List<CheckInUserPair> ();
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
			TableView.RowHeight = 50;

			View.Add (TableView);

			this.View.Add (BusyView);
			BusyView.Busy = true;

			LoadPlaceCheckInInfo ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			if (this.TableView != null)
			{
				MonoTouch.Foundation.NSIndexPath path = TableView.IndexPathForSelectedRow;
				if (path != null)
					TableView.DeselectRow (path, animated);
			}
		}

		private void LoadPlaceCheckInInfo ()
		{
			Func<int> func = delegate {

				List<CheckInUserPair> recentList;
				List<CheckInUserPair> activeList;

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

				//cell.SelectionStyle = UITableViewCellSelectionStyle.None;

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

				SetupCellForUser (cell, ViewController.ActiveCheckInList[indexPath.Row].User);
				
				return cell;
			}

			private UITableViewCell GetCellForRecentCheckIns (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell ("PlaceInfoViewRecentCheckInCell");
				
				if (cell == null)
				{
					cell = new UITableViewCell (UITableViewCellStyle.Default, "PlaceInfoViewRecentCheckInCell");
				}

				SetupCellForUser (cell, ViewController.RecentCheckInList[indexPath.Row].User);
				
				return cell;
			}

			private void SetupCellForUser (UITableViewCell cell, User user)
			{
				cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
				cell.TextLabel.Text = user.FullName;
				cell.ImageView.Layer.MasksToBounds = true;
				cell.ImageView.Layer.CornerRadius = 5.0f;
				
				byte [] data = Engine.Instance.ImageCache.FindOrLoad  
					(user, 50, (avatarResult) => {
						if (avatarResult.Exceptin == null && avatarResult.Data != null)
							this.BeginInvokeOnMainThread (delegate {
								LoadImageForCell (cell, avatarResult.Data);
							});
					});
				
				LoadImageForCell (cell, data);
			}

			private void LoadImageForCell (UITableViewCell cell, byte [] data)
			{
				if (cell.ImageView.Image != null)
					cell.ImageView.Image.Dispose ();
				
				using (NSData nsData = NSData.FromArray (data))
				{
					cell.ImageView.Image = UIImage.LoadFromData (nsData);
				}
			}
		}

		protected class PlaceInfoViewDelegate : UITableViewDelegate
		{
			public PlaceInfoViewController ViewController{get;private set;}

			public PlaceInfoViewDelegate (PlaceInfoViewController viewController)
			{
				ViewController = viewController;
			}

			public override void RowSelected (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				switch (indexPath.Section)
				{
				case 0:
					OpenInMaps (tableView, indexPath);
					return;
				case 1 :
					if (ViewController.HasActiveCheckIns)
						ViewController.NavigationController.PushViewController (
								new ProfileViewController (
									ViewController.ActiveCheckInList[indexPath.Row].User), true
							);
					else
						ViewController.NavigationController.PushViewController (
							new ProfileViewController (
								ViewController.RecentCheckInList[indexPath.Row].User), true
							);
					return;
				case 2 :
					ViewController.NavigationController.PushViewController (
						new ProfileViewController (
							ViewController.RecentCheckInList[indexPath.Row].User), true
						);
					return;
				}
				
				throw new NotImplementedException ();
			}

			void OpenInMaps (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UIAlertView alertView = new UIAlertView (
					"Open in maps?", string.Format("Do you want to view {0} in maps?",
				        ViewController.Place.Name), 
						null, null, new string [] {"Yes", "No"});
				
				alertView.CancelButtonIndex = 1;
				
				alertView.Clicked += (object sender2, UIButtonEventArgs e2) => {
					if (e2.ButtonIndex == 0)
					{
						OpenInMaps ();
					}
				
					tableView.DeselectRow (indexPath, false);
				};
				
				alertView.Show ();
			}

			void OpenInMaps ()
			{
				bool canDoIOS6Maps = false;
				
				using (NSString curVer = new NSString (UIDevice.CurrentDevice.SystemVersion))
				{
					using (NSString targetVar = new NSString ("6.0"))
					{
						canDoIOS6Maps = curVer.Compare (
							targetVar, NSStringCompareOptions.NumericSearch) != NSComparisonResult.Ascending;
						
					}
				}

				if (canDoIOS6Maps)
					OpenIOSMap ();
				else
					OpenGoogleMap ();
			}

			void OpenIOSMap ()
			{
				double latitude = ViewController.Place.Latitude;
				double longitude = ViewController.Place.Longitude;
				CLLocationCoordinate2D center = new CLLocationCoordinate2D (latitude, longitude);
				
				MKPlacemark placemark = new MKPlacemark (center, null);
				MKMapItem mapItem = new MKMapItem (placemark);
				mapItem.Name = ViewController.Place.Name;
				MKLaunchOptions options = new MKLaunchOptions ();
				options.MapSpan = MKCoordinateRegion.FromDistance (center, 200, 200).Span;
				mapItem.OpenInMaps (options);
			}

			void OpenGoogleMap ()
			{
				UIApplication.SharedApplication
					.OpenUrl (new NSUrl (string.Format("https://maps.google.com/?q={0},{1}",
					                                   ViewController.Place.Latitude,
					                                   ViewController.Place.Longitude)));
			}
		}
	}
}

