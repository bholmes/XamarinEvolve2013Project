using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;

namespace XamarinEvolveIOS
{
	public class BusyView : UIView
	{
		UIActivityIndicatorView IndicatorView {get;set;}
		
		public BusyView (RectangleF frame) : base (frame)
		{
			float r,g,b,a;
			UIColor.Gray.GetRGBA (out r, out g, out b, out a); 
			this.BackgroundColor = UIColor.FromRGBA (r, g, b, .7f);
			this.AutoresizingMask = UIViewAutoresizing.All;
			
			IndicatorView = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.White);
			IndicatorView.Center = new PointF (Bounds.GetMidX (), Bounds.GetMidY ());
			IndicatorView.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			IndicatorView.StartAnimating ();
			
			this.Add (IndicatorView);
		}
		
		public bool Busy
		{
			get
			{
				return IndicatorView.IsAnimating;
			}
			
			set
			{
				if (value)
				{
					IndicatorView.StartAnimating ();
					Hidden = false;
				}
				else
				{
					IndicatorView.StopAnimating ();
					Hidden = true;
				}
			}
		}
	}

}

