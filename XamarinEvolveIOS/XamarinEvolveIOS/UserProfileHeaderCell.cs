using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using XamarinEvolveSSLibrary;

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
		public UILabel ImageEditTip {get;private set;}

		public UILabel FullNameLabel {get;private set;}
		public UILabel CityLabel {get;private set;}

		public UITextField FullNameTextView {get;private set;}
		public UITextField CityTextView {get;private set;}

		public delegate void OnImageChangeRequestDelegate (UIImage originalImage);
		public event OnImageChangeRequestDelegate OnImageChangeRequest;

		private User _userProfile;
		private bool _editing = false;
		
		public UserProfileHeaderCell (User userProfile)
		{
			_userProfile = userProfile;

			this.BackgroundColor = UIColor.Clear;
			this.Frame = new RectangleF (0, 0, 200, 100);
			
			ImageView = new UIImageView (new RectangleF (10, 10, 80, 80));
			UITapGestureRecognizer imageTap = 
				new UITapGestureRecognizer (() => {
					if (_editing && OnImageChangeRequest != null)
					{
						OnImageChangeRequest(ImageView.Image);
					}
			});

			ImageView.AddGestureRecognizer (imageTap);
			ImageView.UserInteractionEnabled = true;

			this.Add (ImageView);
			
			int labelXPos = 100;
			FullNameLabel = new UILabel (new RectangleF (
				labelXPos, 10,  this.Frame.Width-(labelXPos+10), 30));
			FullNameLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			FullNameLabel.BackgroundColor = UIColor.Clear;
			FullNameLabel.Font = UIFont.BoldSystemFontOfSize (22);
			FullNameLabel.AdjustsFontSizeToFitWidth = true;
			FullNameLabel.Text = _userProfile.FullName;
			this.Add (FullNameLabel);

			FullNameTextView = new UITextField (new RectangleF (
				labelXPos, 12,  this.Frame.Width-(labelXPos+10), 30));
			FullNameTextView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			FullNameTextView.BackgroundColor = UIColor.Clear;
			FullNameTextView.Font = UIFont.BoldSystemFontOfSize (22);
			FullNameTextView.AdjustsFontSizeToFitWidth = true;
			FullNameTextView.Text = _userProfile.FullName;
			FullNameTextView.ClearButtonMode = UITextFieldViewMode.Always;
			FullNameTextView.AutocapitalizationType = UITextAutocapitalizationType.Words;
			FullNameTextView.AutocorrectionType = UITextAutocorrectionType.No;
			FullNameTextView.Placeholder = "Full Name";
			FullNameTextView.ReturnKeyType = UIReturnKeyType.Done;
			FullNameTextView.ShouldReturn = delegate {
				FullNameTextView.ResignFirstResponder ();
				return true;
			};
			this.Add (FullNameTextView);
			
			CityLabel = new UILabel (new RectangleF (
				labelXPos, 50,  this.Frame.Width-(labelXPos+10), 24));
			CityLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			CityLabel.BackgroundColor = UIColor.Clear;
			CityLabel.Font = UIFont.BoldSystemFontOfSize (17);
			CityLabel.AdjustsFontSizeToFitWidth = true;
			CityLabel.Text = _userProfile.City;
			this.Add (CityLabel);

			CityTextView = new UITextField (new RectangleF (
				labelXPos, 52,  this.Frame.Width-(labelXPos+10), 24));
			CityTextView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			CityTextView.BackgroundColor = UIColor.Clear;
			CityTextView.Font = UIFont.BoldSystemFontOfSize (17);
			CityTextView.AdjustsFontSizeToFitWidth = true;
			CityTextView.Text = _userProfile.City;
			CityTextView.Placeholder = "City";
			CityTextView.ReturnKeyType = UIReturnKeyType.Done;
			CityTextView.ClearButtonMode = UITextFieldViewMode.Always;
			CityTextView.AutocapitalizationType = UITextAutocapitalizationType.Words;
			CityTextView.ShouldReturn = delegate {
				CityTextView.ResignFirstResponder ();
				return true;
			};
			this.Add (CityTextView);

			ImageEditTip = new UILabel (new RectangleF (
				10, 75,  80, 15));
			ImageEditTip.AutoresizingMask = UIViewAutoresizing.None;
			ImageEditTip.BackgroundColor = UIColor.Clear;
			ImageEditTip.Font = UIFont.SystemFontOfSize (13);
			ImageEditTip.AdjustsFontSizeToFitWidth = true;
			ImageEditTip.Text = "Tap to edit";
			ImageEditTip.Hidden = true;
			ImageEditTip.TextAlignment = UITextAlignment.Center;
			this.Add (ImageEditTip);


			RefreshImageFromData ();
		}

		public void RefreshImageFromData ()
		{
			string imageURL = _userProfile.Avatar;
			if (string.IsNullOrEmpty (imageURL) && !string.IsNullOrEmpty (_userProfile.Email))
				imageURL = GravatarHelper.GetGravatarURL (_userProfile.Email, 80);

			UIImage image = UIImageCache.GetOrLoadImage (imageURL, imagearg =>  {
				MonoTouch.UIKit.UIApplication.SharedApplication.BeginInvokeOnMainThread (() =>  {
					ImageView.Image = imagearg;
				});
			}, DefaultImage);
			ImageView.Image = image;
		}

		protected override void EditStyleChanged (bool editing, bool animated)
		{
			base.EditStyleChanged (editing, animated);
			_editing = editing;
			
			if (editing)
			{
				RectangleF rect = ImageView.Frame;
				if (rect.Width == 80)
				{
					rect.X += 10; rect.Width -= 20; rect.Height -= 20;
					ImageView.Frame = rect;
				}

				ImageEditTip.Hidden = false;

				FullNameTextView.Hidden = false;
				FullNameLabel.Hidden = true;

				CityTextView.Hidden = false;
				CityLabel.Hidden = true;
			}
			else
			{
				RectangleF rect = ImageView.Frame;
				if (rect.Width == 60)
				{
					rect.X -= 10; rect.Width += 20; rect.Height += 20;
					ImageView.Frame = rect;
				}

				ImageEditTip.Hidden = true;

				FullNameLabel.Text = _userProfile.FullName = FullNameTextView.Text;
				CityLabel.Text = _userProfile.City = CityTextView.Text;

				FullNameTextView.ResignFirstResponder ();
				FullNameLabel.Hidden = false;
				FullNameTextView.Hidden = true;

				CityTextView.ResignFirstResponder ();
				CityLabel.Hidden = false;
				CityTextView.Hidden = true;
			}
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
				label2XPos, 11,  this.Frame.Width-(label2XPos+10), 21));

			ValueTextField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			ValueTextField.BackgroundColor = UIColor.Clear;
			ValueTextField.Font = UIFont.BoldSystemFontOfSize (17);
			ValueTextField.AdjustsFontSizeToFitWidth = true;
			ValueTextField.Text = valueGet.Invoke ();
			ValueTextField.ReturnKeyType = UIReturnKeyType.Done;
			ValueTextField.ClearButtonMode = UITextFieldViewMode.Always;
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

