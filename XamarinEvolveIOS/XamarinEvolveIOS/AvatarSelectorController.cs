using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;
using Xamarin.Media;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace XamarinEvolveIOS
{
	public class AvatarSelectorController : UIViewController
	{
		public AvatarSelectorView SelectorView {get; private set;}
		UIImage _originalImage;

		public AvatarSelectorController (UIImage originalImage)
		{
			_originalImage = originalImage;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.View.Add (SelectorView = new AvatarSelectorView (this.View.Bounds, _originalImage));
			SelectorView.ImageApplied += delegate {
				NavigationController.PopToViewController (
					NavigationController.ViewControllers[NavigationController.ViewControllers.Length-2], true);
			};
		}
	}

	public class AvatarSelectorView : UIView
	{
		UIImageView _imageView = null;
		UILabel _tipLabel = null;
		UIScrollView _scrollView = null;
		UIButton _selectNewPhoto;
		UIButton _applyChanges;
		const float _windowWidth = 200.0f;

		public delegate void ImageAppliedDelegate (UIImage image);
		public event ImageAppliedDelegate ImageApplied;

		public AvatarSelectorView (RectangleF rect, UIImage originalImage) : base (rect)
		{
			this.BackgroundColor = UIColor.Black;
			this.AutoresizingMask = UIViewAutoresizing.All;
			this.MultipleTouchEnabled = true;

			_tipLabel = new UILabel (new RectangleF (this.Frame.GetMidX () - 150, 10, 300, 40));
			_tipLabel.BackgroundColor = UIColor.Clear;
			_tipLabel.TextColor = UIColor.LightTextColor;
			_tipLabel.Text = "Move and Scale";
			_tipLabel.Font = UIFont.BoldSystemFontOfSize (30);
			_tipLabel.TextAlignment = UITextAlignment.Center;

			RectangleF scrollRect = new RectangleF (this.Frame.GetMidX () - 100, 70, _windowWidth, _windowWidth);;

			_scrollView = new UIScrollView (scrollRect);
			_scrollView.ClipsToBounds = true;
			_scrollView.ShowsHorizontalScrollIndicator = false;
			_scrollView.ShowsVerticalScrollIndicator = false;

			UIImage image = originalImage;

			_imageView = new UIImageView (new RectangleF (new PointF (), image.Size));

			_scrollView.ContentSize = image.Size;
			_imageView.Image = image;

			_scrollView.Delegate = new foo () {View = _imageView};

			SetScrollZoomInfo (scrollRect, true);

			_scrollView.Add(_imageView);
			this.Add (_scrollView);

			_applyChanges = AddNewCustomButton ();
			_applyChanges.Frame = new RectangleF (20,rect.Height-63*2,
			                                        rect.Width-40, 44);
			_applyChanges.SetTitle ("Apply Changes", UIControlState.Normal);
			_applyChanges.TouchUpInside += ApplyChangesTap;

			this.Add (_applyChanges);

			_selectNewPhoto = AddNewCustomButton ();
			_selectNewPhoto.Frame = new RectangleF (20,rect.Height-63,
			                                  rect.Width-40, 44);
			_selectNewPhoto.SetTitle ("Change Photo", UIControlState.Normal);
			_selectNewPhoto.TouchUpInside += ChangePhotoTap;

			this.Add (_selectNewPhoto);
			this.Add (_tipLabel);
		}

		private UIButton AddNewCustomButton ()
		{
			UIButton newButton = UIButton.FromType (UIButtonType.Custom);

			newButton.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | 
				UIViewAutoresizing.FlexibleTopMargin;
			
			List<UIImage> colorImages = ImagesFromColors (new UIColor [] {UIColor.Black, UIColor.White});
			
			newButton.SetBackgroundImage (colorImages[0], UIControlState.Normal);
			newButton.SetBackgroundImage (colorImages[1], UIControlState.Highlighted);
			newButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			newButton.SetTitleColor (UIColor.LightGray, UIControlState.Highlighted);
			
			newButton.Layer.MasksToBounds = true;
			newButton.Layer.BorderWidth = 2f;
			newButton.Layer.CornerRadius = 10.0f;
			newButton.Layer.BorderColor = UIColor.White.CGColor;

			return newButton;
		}

		public override UIView HitTest (PointF point, UIEvent uievent)
		{
			if (_selectNewPhoto.Frame.Contains (point))
			    return _selectNewPhoto;

			if (_applyChanges.Frame.Contains (point))
				return _applyChanges;

			return _scrollView;
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			_scrollView.Frame = new RectangleF (this.Frame.GetMidX () - 100, 70, _windowWidth, _windowWidth);
			_tipLabel.Frame = new RectangleF (this.Frame.GetMidX () - 150, 10, 300, 40);

			SetScrollZoomInfo (_scrollView.Frame, false);
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
			MediaPicker picker = new MediaPicker ();

			if (!picker.IsCameraAvailable)
			{
				PickPhotoFromLibrary (picker);
				return;
			}

			UIActionSheet actionSheet = new UIActionSheet ("Select Source");
			actionSheet.AddButton ("From Camera");

			actionSheet.AddButton ("From Library");
			actionSheet.AddButton ("Cancel");
			actionSheet.CancelButtonIndex = 2;

			actionSheet.Clicked += ActionSheetTapped;

			actionSheet.ShowInView (this);
		}

		void ApplyChangesTap (object sender, EventArgs e)
		{
			UIImage imaage = _imageView.Image;

			float scale;

			if (imaage.Size.Width < imaage.Size.Height)
			{
				scale = imaage.Size.Width / _scrollView.ContentSize.Width;
			}
			else
			{
				scale = imaage.Size.Height / _scrollView.ContentSize.Height;
			}

			RectangleF cropRect = new RectangleF () {
				X =_scrollView.ContentOffset.X * scale,
				Y =_scrollView.ContentOffset.Y * scale,
				Width = scale * _windowWidth,
				Height = scale * _windowWidth,
			};

			UIImage newImage = CropImage (imaage, cropRect);

			if (ImageApplied != null)
				ImageApplied (newImage);
		}

		UIImage CropImage (UIImage originalImage, RectangleF rect)
		{

			try
			{
				// I found I needed to draw the image to remove rotation
				// I can probably add scaling here as well to keep image
				// sizes at 200 x 200

				UIGraphics.BeginImageContext (originalImage.Size);
				CGContext context = UIGraphics.GetCurrentContext ();
				
				originalImage.Draw (new PointF ());
				
				UIImage rotateImage = UIGraphics.GetImageFromCurrentImageContext ();
				{
					return new UIImage (rotateImage.CGImage.WithImageInRect (rect));
				}
			}
			finally
			{
				UIGraphics.EndImageContext ();
			}
		}

		void SetNewImage (UIImage image)
		{
			_scrollView.MinimumZoomScale = 1;
			_scrollView.MaximumZoomScale =  5;
			_scrollView.SetZoomScale (1, false);
			
			_imageView.Frame = new RectangleF (new PointF (), image.Size);
			_imageView.Image = image;
			_scrollView.ContentSize = image.Size;
			
			_scrollView.Frame = new RectangleF (this.Frame.GetMidX () - 100, 40, _windowWidth, _windowWidth);

			SetScrollZoomInfo (_scrollView.Frame, true);
		}

		void ActionSheetTapped (object sender, UIButtonEventArgs e)
		{
			MediaPicker picker = new MediaPicker ();

			if (e.ButtonIndex == 0)
			{
				Task<MediaFile> task = picker.TakePhotoAsync (new StoreCameraMediaOptions ()
				                                              {DefaultCamera= CameraDevice.Front});
				HandlePick (task);
			}
			else if (e.ButtonIndex == 1)
			{
				PickPhotoFromLibrary (picker);
			}
		}

		private void PickPhotoFromLibrary (MediaPicker picker)
		{
			Task<MediaFile> task = picker.PickPhotoAsync ();
			HandlePick (task);
		}

		void HandlePick (Task<MediaFile> task)
		{
			task.ContinueWith (delegate {
				if (task.Status == TaskStatus.RanToCompletion) {
					NSData nsData = NSData.FromStream (task.Result.GetStream ());
					this.BeginInvokeOnMainThread (delegate {
						SetNewImage (UIImage.LoadFromData (nsData));
						try
						{
							// try remove the temp files from disk
							System.IO.File.Delete (task.Result.Path);
						}
						catch (Exception) {}
						task.Result.Dispose ();
					});
				}
			});
		}

		private List<UIImage> ImagesFromColors (UIColor [] colors)
		{
			List<UIImage> ret = new List<UIImage> ();
			RectangleF rect = new RectangleF (0, 0, 1, 1);
			try 
			{
				UIGraphics.BeginImageContext (rect.Size);
				CGContext context = UIGraphics.GetCurrentContext ();

				foreach (UIColor color in colors)
				{
					context.SetFillColor (color.CGColor);
					context.FillRect (rect);
					ret.Add (UIGraphics.GetImageFromCurrentImageContext ());
				}
			}
			finally
			{
				UIGraphics.EndImageContext ();
			}
			
			return ret;
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

