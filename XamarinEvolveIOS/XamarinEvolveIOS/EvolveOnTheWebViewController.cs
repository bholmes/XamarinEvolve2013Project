using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace XamarinEvolveIOS
{
	public class EvolveOnTheWebViewController : UIViewController
	{
		BusyView _busyView;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "On the Web";
			
			UIWebView view = new UIWebView (this.View.Bounds);
			view.AutoresizingMask = UIViewAutoresizing.All;
			view.ScalesPageToFit = true;
			_busyView = new BusyView (this.View.Bounds);
			_busyView.Busy = true;
			
			this.View.Add (view);
			this.View.Add (_busyView);
			
			view.LoadRequest (new NSUrlRequest (new NSUrl ("http://xamarin.com/evolve")));
			
			view.LoadFinished += delegate {
				BeginInvokeOnMainThread (delegate {
					_busyView.Busy = false;
				});
			};
		}
	}
}

