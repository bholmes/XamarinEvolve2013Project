using System;
using MonoTouch.UIKit;
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
}

