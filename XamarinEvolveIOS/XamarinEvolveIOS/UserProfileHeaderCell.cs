using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

namespace XamarinEvolveIOS
{
	public class UserProfileHeaderCell : UIView
	{
		public UIImageView ImageView {get; private set;}
		public UILabel FullNameLabel {get;private set;}
		public UILabel CityLabel {get;private set;}
		
		public UserProfileHeaderCell (string fullName, string cityName, string imageURL)
		{
			this.BackgroundColor = UIColor.Clear;
			this.Frame = new RectangleF (0, 0, 200, 1000);
			
			ImageView = new UIImageView (new RectangleF (10, 10, 80, 80));
			this.Add (ImageView);
			
			int lableXPos = 100;
			FullNameLabel = new UILabel (new RectangleF (
				lableXPos, 10,  this.Frame.Width-(lableXPos+10), 40));
			FullNameLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			FullNameLabel.BackgroundColor = UIColor.Clear;
			FullNameLabel.Font = UIFont.BoldSystemFontOfSize (22);
			FullNameLabel.AdjustsFontSizeToFitWidth = true;
			FullNameLabel.Text = fullName;
			this.Add (FullNameLabel);
			
			CityLabel = new UILabel (new RectangleF (
				lableXPos, 50,  this.Frame.Width-(lableXPos+10), 40));
			CityLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			CityLabel.BackgroundColor = UIColor.Clear;
			CityLabel.Font = UIFont.BoldSystemFontOfSize (17);
			CityLabel.AdjustsFontSizeToFitWidth = true;
			CityLabel.Text = cityName;
			this.Add (CityLabel);
			
			UIImage image = UIImageCache.GetOrLoadImage (imageURL, (imagearg) => {
				MonoTouch.UIKit.UIApplication.SharedApplication.BeginInvokeOnMainThread (()=>{
					ImageView.Image = imagearg;
				} );
			});
			
			if (image != null)
				ImageView.Image = image;
			else 
				ImageView.Image = UIImage.FromFile ("blankavatar.jpg");
		}
		
		public CustomUITableViewCell LoadCell (UITableView tableView)
		{
			NSArray array = NSBundle.MainBundle.LoadNib ("CustomUITableViewCell", tableView, null);
			CustomUITableViewCell customCell = Runtime.GetNSObject (array.ValueAt (0)) as CustomUITableViewCell;
			
			customCell.SetInnerView (this);
			
			return customCell;
		}
	}
}

