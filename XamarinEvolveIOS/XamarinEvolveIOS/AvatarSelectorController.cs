using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;
using Xamarin.Media;
using System.Threading.Tasks;
using MonoTouch.Foundation;

namespace XamarinEvolveIOS
{
	public class AvatarSelectorController : UIViewController
	{
		AvatarSelectorView _view;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.View.Add (_view = new AvatarSelectorView (this.View.Bounds));
		}
	}

	public class AvatarSelectorView : UIView
	{
		UIImageView _imageView = null;
		UIScrollView _scrollView = null;
		UIView _screenView1, _screenView2;

		public AvatarSelectorView (RectangleF rect) : base (rect)
		{
			this.BackgroundColor = UIColor.Black;
			this.AutoresizingMask = UIViewAutoresizing.All;
		

			RectangleF scrollRect = this.Frame;
			RectangleF screenRect1 = this.Frame;
			RectangleF screenRect2;
			
			CalculateRects (out scrollRect, out screenRect1, out screenRect2);

			_scrollView = new UIScrollView (scrollRect);
			_scrollView.ClipsToBounds = false;
			_scrollView.ShowsHorizontalScrollIndicator = false;
			_scrollView.ShowsVerticalScrollIndicator = false;


			UIImage image = UIImage.FromFile ("blankavatar.jpg");

			_imageView = new UIImageView (new RectangleF (new PointF (), image.Size));

			_scrollView.ContentSize = image.Size;
			_imageView.Image = image;

			_scrollView.Delegate = new foo () {View = _imageView};

			SetScrollZoomInfo (scrollRect, true);

			_scrollView.Add(_imageView);
			this.Add (_scrollView);

			_screenView1 = new UIView (screenRect1);
			_screenView1.BackgroundColor = UIColor.DarkGray;
			_screenView1.Alpha = .95f;
			this.Add (_screenView1);
			
			_screenView2 = new UIView (screenRect2);
			_screenView2.BackgroundColor = UIColor.DarkGray;
			_screenView2.Alpha = .95f;
			this.Add (_screenView2);

			UIButton changePhoto = UIButton.FromType (UIButtonType.Custom);
			changePhoto.Frame = new RectangleF (20,rect.Height-63,
			                                    rect.Width-40, 44);
			changePhoto.SetTitle ("Change Photo", UIControlState.Normal);
			changePhoto.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | 
				UIViewAutoresizing.FlexibleTopMargin;

			changePhoto.SetBackgroundImage (ImageFromColor (UIColor.Black), UIControlState.Normal);
			changePhoto.SetBackgroundImage (ImageFromColor (UIColor.White), UIControlState.Highlighted);
			changePhoto.SetTitleColor (UIColor.White, UIControlState.Normal);
			changePhoto.SetTitleColor (UIColor.LightGray, UIControlState.Highlighted);

			changePhoto.Layer.MasksToBounds = true;
			changePhoto.Layer.BorderWidth = 2f;
			changePhoto.Layer.CornerRadius = 10.0f;
			changePhoto.Layer.BorderColor = UIColor.White.CGColor;

			changePhoto.TouchUpInside += ChangePhotoTap;

			this.Add (changePhoto);
		}

		private void CalculateRects (out RectangleF scrollRect, out RectangleF screenRect1, 
		                             out RectangleF screenRect2)
		{
			scrollRect = screenRect1 = this.Frame;
			
			if (scrollRect.Width > scrollRect.Height)
			{
				scrollRect.Width = scrollRect.Height;
				scrollRect.X = this.Frame.GetMidX () - (scrollRect.Width / 2.0f);
				screenRect1.Width = scrollRect.X;
				screenRect2 = screenRect1;
				screenRect2.X = scrollRect.Right;
			}
			else
			{
				scrollRect.Height = scrollRect.Width;
				screenRect1.Height = scrollRect.Y = 30;
				screenRect2 = screenRect1;
				screenRect2.Y = scrollRect.Bottom;
				screenRect2.Height = this.Frame.Height - screenRect2.Y;
			}
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			RectangleF scrollRect = this.Frame;
			RectangleF screenRect1 = this.Frame;
			RectangleF screenRect2;
			
			CalculateRects (out scrollRect, out screenRect1, out screenRect2);

			_scrollView.Frame = scrollRect;
			_screenView1.Frame = screenRect1;
			_screenView2.Frame = screenRect2;

			SetScrollZoomInfo (scrollRect, false);
		}

		void SetScrollZoomInfo (RectangleF scrollRect, bool forceScale)
		{
			float factor, currentFactor;
			SizeF imageSize = _imageView.Image.Size;

			if (_scrollView.ContentSize.Height > _scrollView.ContentSize.Width) {
				factor = scrollRect.Width / imageSize.Width;
				currentFactor = _scrollView.ContentSize.Width / scrollRect.Width;
			}
			else {
				factor = scrollRect.Height / imageSize.Height;
				currentFactor = _scrollView.ContentSize.Height / scrollRect.Height;
			}
			_scrollView.MinimumZoomScale = factor;
			_scrollView.MaximumZoomScale = factor * 5;
			if (forceScale || currentFactor < 1)
				_scrollView.SetZoomScale (factor, false);
		}

		void ChangePhotoTap (object sender, EventArgs e)
		{
			UIActionSheet actionSheet = new UIActionSheet ("Select Source");
			actionSheet.AddButton ("From Camera");
			actionSheet.AddButton ("From Library");
			actionSheet.AddButton ("Cancel");
			actionSheet.CancelButtonIndex = 2;

			actionSheet.Clicked += ActionSheetTapped;

			actionSheet.ShowInView (this);
		}

		void SetNewImage (UIImage image)
		{
			RectangleF scrollRect;
			RectangleF screenRect1;
			RectangleF screenRect2;

			_scrollView.MinimumZoomScale = 1;
			_scrollView.MaximumZoomScale =  5;
			_scrollView.SetZoomScale (1, false);
			
			_imageView.Frame = new RectangleF (new PointF (), image.Size);
			_imageView.Image = image;
			_scrollView.ContentSize = image.Size;
			
			CalculateRects (out scrollRect, out screenRect1, out screenRect2);

			SetScrollZoomInfo (scrollRect, true);
		}

		void ActionSheetTapped (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 0)
			{
				MediaPicker picker = new MediaPicker ();
				Task<MediaFile> task = picker.TakePhotoAsync (new StoreCameraMediaOptions ());
				task.ContinueWith (delegate {
					if (task.Status == TaskStatus.RanToCompletion)
					{
						NSData nsData = NSData.FromStream (task.Result.GetStream ());
						this.BeginInvokeOnMainThread (delegate {
							SetNewImage (UIImage.LoadFromData (nsData));	
						});
					}
				});
			}
			else if (e.ButtonIndex == 1)
			{
				MediaPicker picker = new MediaPicker ();
				Task<MediaFile> task = picker.PickPhotoAsync ();
				task.ContinueWith (delegate {
					if (task.Status == TaskStatus.RanToCompletion)
					{
						NSData nsData = NSData.FromStream (task.Result.GetStream ());
						this.BeginInvokeOnMainThread (delegate {
							SetNewImage (UIImage.LoadFromData (nsData));
						});
					}
				});
			}
		}

		private UIImage ImageFromColor (UIColor color)
		{
			RectangleF rect = new RectangleF (0, 0, 1, 1);
			UIGraphics.BeginImageContext (rect.Size);
			CGContext context = UIGraphics.GetCurrentContext ();
			context.SetFillColor (color.CGColor);
			context.FillRect (rect);
			UIImage image = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return image;
		}

		private class foo : UIScrollViewDelegate
		{
			public UIView View {get;set;}
			public override UIView ViewForZoomingInScrollView (UIScrollView scrollView)
			{
				return View;
			}
		}
	}
}

