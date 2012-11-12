using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;

namespace XamarinEvolveIOS
{
	[Register("CustomUITableViewCell")]
	public partial class CustomUITableViewCell : UITableViewCell
	{
		public UIView CustomView {get;private set;}

		public CustomUITableViewCell (IntPtr ptr) : base (ptr)
		{
			this.SelectionStyle = UITableViewCellSelectionStyle.None;
		}

		public void SetInnerView (UIView view)
		{
			if (CustomView != null)
				throw new Exception ("This should not happen");

			CustomView = view;
			CustomView.Frame = HostView.Bounds;
			HostView.Add (view);

		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			if (CustomView != null)
			{
				CustomView.Frame = HostView.Bounds;
			}
		}
	}

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
				ImageView.Image = UIImage.FromFile ("blankavatar.png");
		}

		public CustomUITableViewCell LoadCell (UITableView tableView)
		{
			NSArray array = NSBundle.MainBundle.LoadNib ("CustomUITableViewCell", tableView, null);
			CustomUITableViewCell customCell = Runtime.GetNSObject (array.ValueAt (0)) as CustomUITableViewCell;

			customCell.SetInnerView (this);

			return customCell;
		}

		private class UIImageCache
		{
			static UIImageCache _instance = new UIImageCache ();

			public delegate void OnImageLoadedDelegate (UIImage image);
			private List<UIImageCacheItem> _list = new List<UIImageCacheItem> ();
			object _lock = new object ();

			private class UIImageCacheItem
			{
				public UIImageCacheItem (string url, OnImageLoadedDelegate OnImageLoaded, OnImageLoadedDelegate finalOnImageLoaded)
				{
					URL = url;

					if (OnImageLoaded != null)
						_delList.Add (OnImageLoaded);

					_finalOnImageLoaded = finalOnImageLoaded;

					new System.Threading.ThreadStart( () => {
						NSData data = NSData.FromUrl (new NSUrl (url));
						UIImage image = UIImage.LoadFromData (data);

						List<OnImageLoadedDelegate> oldDelList;

						lock (_lock)
						{
							_image = image;
							oldDelList = _delList;
							_delList = new List<OnImageLoadedDelegate> ();
						}

						foreach (OnImageLoadedDelegate onImageLoaded in oldDelList)
						{
							onImageLoaded (_image);
						}

						if (_finalOnImageLoaded != null)
							_finalOnImageLoaded (_image);

					}).BeginInvoke (null, null);
				}

				public UIImage GetOrLoadImage (OnImageLoadedDelegate OnImageLoaded)
				{
					if (_image != null)
						return _image;

					lock (_lock)
					{
						if (_image != null)
							return _image;

						_delList.Add (OnImageLoaded);
					}

					return null;
				}

				public string URL {get;private set;}
				private UIImage _image;
				private List<OnImageLoadedDelegate> _delList = new List<OnImageLoadedDelegate> ();
				private OnImageLoadedDelegate _finalOnImageLoaded;
				object _lock = new object();
			}

			private UIImage internalGetOrLoadImage (string url, OnImageLoadedDelegate OnImageLoaded)
			{
				lock (_lock)
				{
					UIImageCacheItem foundItem = _list.Find (e=>e.URL.Equals (url));
					if(foundItem != null)
					{
						_list.Remove (foundItem);
						_list.Add (foundItem);
						return foundItem.GetOrLoadImage (OnImageLoaded);
					}

					_list.Add (new UIImageCacheItem (url, OnImageLoaded, (image) =>{
						cleanupLongList ();
					}));
				}

				return null;	
			}

			private void cleanupLongList ()
			{
				lock (_lock)
				{
					if (_list.Count > 250)
					{
						_list.RemoveRange (0, 250);
					}
				}
			}

			static public UIImage GetOrLoadImage (string url, OnImageLoadedDelegate OnImageLoaded)
			{
				return _instance.internalGetOrLoadImage (url, OnImageLoaded);	
			}
		}
	}
}

