using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

namespace XamarinEvolveIOS
{
	public class CustomUITableViewCellSubView : UIView
	{
		public CustomUITableViewCell LoadCell (UITableView tableView)
		{
			NSArray array = NSBundle.MainBundle.LoadNib ("CustomUITableViewCell", tableView, null);
			CustomUITableViewCell customCell = Runtime.GetNSObject (array.ValueAt (0)) as CustomUITableViewCell;

			customCell.EditStyleChanged += EditStyleChanged;

			customCell.SetInnerView (this);
			
			return customCell;
		}

		protected virtual void EditStyleChanged (bool editing, bool animated)
		{

		}
	}

	public class UserProfileHeaderCell : CustomUITableViewCellSubView
	{
		public UIImageView ImageView {get; private set;}
		public UILabel FullNameLabel {get;private set;}
		public UILabel CityLabel {get;private set;}
		
		public UserProfileHeaderCell (string fullName, string cityName, string imageURL)
		{
			this.BackgroundColor = UIColor.Clear;
			this.Frame = new RectangleF (0, 0, 200, 100);
			
			ImageView = new UIImageView (new RectangleF (10, 10, 80, 80));
			this.Add (ImageView);
			
			int labelXPos = 100;
			FullNameLabel = new UILabel (new RectangleF (
				labelXPos, 10,  this.Frame.Width-(labelXPos+10), 40));
			FullNameLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			FullNameLabel.BackgroundColor = UIColor.Clear;
			FullNameLabel.Font = UIFont.BoldSystemFontOfSize (22);
			FullNameLabel.AdjustsFontSizeToFitWidth = true;
			FullNameLabel.Text = fullName;
			this.Add (FullNameLabel);
			
			CityLabel = new UILabel (new RectangleF (
				labelXPos, 50,  this.Frame.Width-(labelXPos+10), 40));
			CityLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			CityLabel.BackgroundColor = UIColor.Clear;
			CityLabel.Font = UIFont.BoldSystemFontOfSize (17);
			CityLabel.AdjustsFontSizeToFitWidth = true;
			CityLabel.Text = cityName;
			this.Add (CityLabel);
			
			UIImage image = UIImageCache.GetOrLoadImage (imageURL, (imagearg) => {
				MonoTouch.UIKit.UIApplication.SharedApplication.BeginInvokeOnMainThread (()=>{
					ImageView.Image = imagearg;
				});
			}, DefaultImage);
			
			ImageView.Image = image;
		}

		static UIImage _defaultImage;
		static UIImage DefaultImage 
		{
			get 
			{
				if (_defaultImage == null)
					_defaultImage = UIImage.FromFile ("blankavatar.jpg");

				return _defaultImage;
			}
		}
	}

	public class NameValueCell : CustomUITableViewCellSubView
	{	
		public UILabel NameLabel {get;private set;}
		public UILabel ValueLabel {get;private set;}
		public UITextField ValueTextField {get;private set;}
		Func<string> _valueGet;
		Action<string> _valueSet;

		public NameValueCell (string name, Func<string> valueGet, Action<string> valueSet)
		{
			this.BackgroundColor = UIColor.Clear;
			this.Frame = new RectangleF (0, 0, 100, 100);
			
			int labelXPos = 10;
			int label2XPos = 75;
			NameLabel = new UILabel (new RectangleF (
				labelXPos, 11,  label2XPos-(labelXPos+5), 21));
			NameLabel.AutoresizingMask = UIViewAutoresizing.None;
			NameLabel.BackgroundColor = UIColor.Clear;
			NameLabel.Font = UIFont.SystemFontOfSize (14);
			NameLabel.AdjustsFontSizeToFitWidth = true;
			NameLabel.TextAlignment = UITextAlignment.Right;
			NameLabel.Text = name;
			NameLabel.TextColor = UIColor.Blue;
			this.Add (NameLabel);

			ValueLabel = new UILabel (new RectangleF (
				label2XPos, 11,  this.Frame.Width-(label2XPos+10), 21));
			ValueLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			ValueLabel.BackgroundColor = UIColor.Clear;
			ValueLabel.Font = UIFont.BoldSystemFontOfSize (17);
			ValueLabel.AdjustsFontSizeToFitWidth = true;
			ValueLabel.Text = valueGet.Invoke ();
			this.Add (ValueLabel);

			ValueTextField = new UITextField (new RectangleF (
				label2XPos, 11,  this.Frame.Width-(label2XPos+10), 30));

			ValueTextField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			ValueTextField.BackgroundColor = UIColor.Clear;
			ValueTextField.Font = UIFont.BoldSystemFontOfSize (17);
			ValueTextField.AdjustsFontSizeToFitWidth = true;
			ValueTextField.Text = valueGet.Invoke ();
			ValueTextField.ReturnKeyType = UIReturnKeyType.Done;
			ValueTextField.ShouldReturn = delegate {
				ValueTextField.ResignFirstResponder ();
				return true;
			};
			this.Add (ValueTextField);

			_valueGet = valueGet;
			_valueSet = valueSet;
		}

		protected override void EditStyleChanged (bool editing, bool animated)
		{
			base.EditStyleChanged (editing, animated);

			if (editing)
			{
				ValueTextField.Hidden = false;
				ValueLabel.Hidden = true;
			}
			else
			{
				_valueSet.Invoke (ValueTextField.Text);
				ValueLabel.Text = _valueGet.Invoke ();
				ValueTextField.ResignFirstResponder ();
				ValueLabel.Hidden = false;
				ValueTextField.Hidden = true;
			}
		}
	}
}

