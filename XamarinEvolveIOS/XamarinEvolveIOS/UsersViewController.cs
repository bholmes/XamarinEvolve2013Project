using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using XamarinEvolveSSLibrary;
using MonoTouch.CoreGraphics;

namespace XamarinEvolveIOS
{
	public class UsersViewController : UITableViewController
	{
		UserList Users {get;set;}
		public BusyView BusyView {get; private set;}

		public UsersViewController ()
		{

		}

		public override void LoadView ()
		{
			base.LoadView ();

			BusyView = new XamarinEvolveIOS.BusyView (this.View.Bounds);

			Title = "Attendees";
			Users = new UserList ();
			this.TableView = new UITableView (this.View.Bounds, UITableViewStyle.Grouped);
			TableView.DataSource = new UsersViewDataSource (UserListGetter);
			TableView.Delegate = new UsersViewDelegate (UserListGetter, NavigationController);

			this.View.Add (BusyView);
			BusyView.Busy = true;

			Engine.Instance.UserAccess.GetUsers ((userList) => {
				this.InvokeOnMainThread (delegate {
					Users = userList.UserList;
					BusyView.Busy = false;
					this.TableView.ReloadData ();
				});
			});
		}

		private UserList UserListGetter ()
		{
			return Users;
		}

		private class UsersViewDataSource : UITableViewDataSource
		{
			Func <UserList> _userListGet;

			public UsersViewDataSource(Func <UserList> userListGet)
			{
				_userListGet = userListGet;
			}

			UserList Users 
			{
				get
				{
					return _userListGet ();
				}
			}

			#region implemented abstract members of UITableViewDataSource			
			public override int RowsInSection (UITableView tableView, int section)
			{
				return Users.Count;
			}			
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UserProfileHeaderCell innerCell = new UserProfileHeaderCell (Users[indexPath.Row]);

				UITableViewCell cell = innerCell.LoadCell (tableView);
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
				return cell;
			}			
			#endregion
		}


		private class UsersViewDelegate : UITableViewDelegate
		{
			Func <UserList> _userListGet;
			
			UserList Users 
			{
				get
				{
					return _userListGet ();
				}
			}
			UINavigationController _navigationController;

			public UsersViewDelegate(Func <UserList> userListGet, UINavigationController navigationController)
			{
				_userListGet = userListGet;
				_navigationController = navigationController;
			}

			public override void RowSelected (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				_navigationController.PushViewController (
					new ProfileViewController (Users[indexPath.Row]), true);
			}

			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return 100;
			}
		}
	}
}

