using System;
using MonoTouch.UIKit;
using XamarinEvolveSSLibrary.GoogleAPI;
using System.Collections.Generic;
using XamarinEvolveSSLibrary;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace XamarinEvolveIOS
{
	public class CheckInViewController : PlaceListViewController
	{
		public CheckInViewController () : base ()
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TableView.DataSource = new PlaceListViewDataSource (this);
			TableView.Delegate = new CheckInViewDelegate (this);
			
			GeolocationHelper.GetLocation ((result) => {
				LoadPlaceList (result);
			});
		}

		private void LoadPlaceList (GeolocationResult result)
		{
			Func<int> func = delegate {
				PlaceList lPlaceList = new PlaceList ();
				
				if (result.Position != null)
				{
					lPlaceList = new PlaceList (
						Engine.Instance.PlaceLocator.GetNearbyPlaces (
						(float)result.Position.Latitude, (float)result.Position.Longitude));
				}
				
				this.BeginInvokeOnMainThread (delegate {
					if (lPlaceList.Count > 0)
					{
						PlaceList = lPlaceList;

						TableView.ReloadData ();
						BusyView.Busy = false;
					}
					else
						ShowLocatinError (lPlaceList, result);
				});
				
				return 0;
			};

			func.BeginInvoke (null, null);
		}

		private class CheckInViewDelegate : PlaceListViewDelegate
		{
			CheckInViewController _viewController;

			public CheckInViewDelegate (CheckInViewController viewController) : base (viewController)
			{
				_viewController = viewController;
			}

			public override void RowSelected (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				Place place = PlaceList[indexPath.Row];

				UIAlertView alertView = new UIAlertView (
					"Check-in?", 
					string.Format ("Are you sure you want check-in at {0}?", place.Name), 
					null, null, new string [] {"Yes", "No"});
				
				alertView.CancelButtonIndex = 1;
				
				alertView.Clicked += (object sender2, UIButtonEventArgs e2) => {
					if (e2.ButtonIndex == 0)
					{
						Engine.Instance.CheckInAccess.CheckInUserAtPlace (place);
						_viewController.NavigationController.PopViewControllerAnimated (true);
					}
				};
				
				alertView.Show ();
			}
		}
	}
}

