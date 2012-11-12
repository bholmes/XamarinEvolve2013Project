using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

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
}

