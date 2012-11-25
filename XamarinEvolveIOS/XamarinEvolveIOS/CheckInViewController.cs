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
			TableView.Delegate = new PlaceListViewDelegate (this);
			
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
	}
}

