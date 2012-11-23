using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using XamarinEvolveSSLibrary;

namespace XamarinEvolveIOS
{
	public class UsersViewController : UITableViewController
	{
		UserList Users {get;set;}
		public UsersViewController ()
		{

		}

		public override void LoadView ()
		{
			base.LoadView ();

			Title = "Attendees";
			Users = Engine.Instance.UserAccess.GetUsers ();
			TableView.DataSource = new UsersViewDataSource (Users);
			TableView.Delegate = new UsersViewDelegate (Users, NavigationController);
		}

		private class UsersViewDataSource : UITableViewDataSource
		{
			UserList Users {get;set;}

			public UsersViewDataSource(UserList users)
			{
				Users = users;
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
			UserList Users {get;set;}
			UINavigationController _navigationController;

			public UsersViewDelegate(UserList users, UINavigationController navigationController)
			{
				Users = users;
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

