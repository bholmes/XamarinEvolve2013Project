using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using XamarinEvolveSSLibrary;
using MonoTouch.AddressBook;
using MonoTouch.AddressBookUI;

namespace XamarinEvolveIOS
{
	public partial class ProfileViewController : UITableViewController
	{
		private User _currentUser;

		public ProfileViewController (User user) : base (UITableViewStyle.Grouped)
		{
			CurrentUser = user;
		}

		public override void LoadView ()
		{
			base.LoadView ();

			this.Title = CurrentUser.IsLocalUser && this is LocalProfileViewController? "My Profile" : "Profile";

			TableView.DataSource = new LocalUserProfileDataSource (
				this, CurrentUser);
			TableView.Delegate = new LocalUserProfileDelegate (
				this, CurrentUser);
			SetupEditButton ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			if (this.TableView != null)
			{
				MonoTouch.Foundation.NSIndexPath path = TableView.IndexPathForSelectedRow;
				if (path != null)
					TableView.DeselectRow (path, animated);
			}
		}

		void SetupEditButton ()
		{
			if (!CurrentUser.IsLocalUser || !(this is LocalProfileViewController))
				return;

			UIBarButtonItem editButton = new UIBarButtonItem ("Edit", UIBarButtonItemStyle.Done, delegate {
				this.SetEditing (!this.Editing, true);
			});
			this.NavigationItem.RightBarButtonItem = editButton;
		}

		public override bool Editing {
			get {
				return base.Editing;
			}
			set {
				base.Editing = value;
				TableView.Editing = value;
			}
		}

		public override void SetEditing (bool editing, bool animated)
		{
			base.SetEditing (editing, animated);
			TableView.SetEditing (editing, animated);

			if (this.Editing) {
				this.NavigationItem.RightBarButtonItem.Title = "Done";
				this.NavigationItem.RightBarButtonItem.Style = UIBarButtonItemStyle.Done;
			}
			else {
				this.NavigationItem.RightBarButtonItem.Title = "Edit";
				this.NavigationItem.RightBarButtonItem.Style = UIBarButtonItemStyle.Plain;
				RefreshHeaderCell ();
				Engine.Instance.UserAccess.CommitCurrentUserChanges ();
				Engine.Instance.ImageCache.TouchUser (_currentUser);
			}
		}

		public void RefreshHeaderCell ()
		{
			CustomUITableViewCell headerCell = 
				this.TableView.CellAt (NSIndexPath.FromRowSection (0, 0)) as CustomUITableViewCell;

			if (headerCell != null) {
				UserProfileHeaderCell headerInnerCell = headerCell.CustomView as UserProfileHeaderCell;
				if (headerInnerCell != null) {
					MonoTouch.UIKit.UIApplication.SharedApplication.BeginInvokeOnMainThread (delegate {
						headerInnerCell.RefreshImageFromData ();
					});
				}
			}
		}

		protected User CurrentUser {
			get{
				return _currentUser;
			}
			set{
				_currentUser = value;

				if (TableView == null)
					return;

				LocalUserProfileDataSource dataSrc = TableView.DataSource as LocalUserProfileDataSource;
				if (dataSrc != null)
					dataSrc.UserProfile = value;

				LocalUserProfileDelegate tableDelegate = TableView.Delegate as LocalUserProfileDelegate;
				if (tableDelegate != null)
					tableDelegate.UserProfile = value;

			}
		}

		private bool _hideTheDamnButtons = false;
		public bool HideTheDamnButtons 
		{
			get {return _hideTheDamnButtons;}
			set {
				_hideTheDamnButtons = value;
				LocalUserProfileDelegate tvDel = TableView.Delegate as LocalUserProfileDelegate;
				tvDel.HideButtons (value);
			}
		}
	}

	public class LocalUserProfileDelegate : UITableViewDelegate
	{
		ProfileViewController _controller;
		UIButton _logoutButton;
		UIButton _deleteButton;
		UIButton _addContactButton;

		public void HideButtons (bool hide)
		{
			if (_logoutButton != null)
				_logoutButton.Hidden = hide;
			
			if (_deleteButton != null)
				_deleteButton.Hidden = hide;
		}
		
		public LocalUserProfileDelegate (ProfileViewController controller, User profile)
		{
			UserProfile = profile;
			_controller = controller;
		}
		
		public User UserProfile {get; set;}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == 0 && indexPath.Row == 0)
				return 100;

			return 44;
		}

		public override float GetHeightForFooter (UITableView tableView, int section)
		{
			if (section == 2)
				return 60;

			return 0;
		}

		public override UIView GetViewForFooter (UITableView tableView, int section)
		{

			if (section == 2 )
			{
				float tableMargin = UIDevice.CurrentDevice.UserInterfaceIdiom == 
					UIUserInterfaceIdiom.Phone ?  9 : 45;
				float tableWidth = tableView.Bounds.Width;
				float contentWidth = tableWidth - (tableMargin * 2.0f);
				float tableCenter = tableWidth /2.0f;

				if (_controller is LocalProfileViewController && this.UserProfile.IsLocalUser && !this.UserProfile.IsAnonymousUser)
				{
					float buttonWidth = (contentWidth / 2.0f) - 3;

					_logoutButton = UIButton.FromType (UIButtonType.RoundedRect);
					_logoutButton.SetTitle ("Logout", UIControlState.Normal);
					_logoutButton.Frame = new RectangleF (tableMargin, 15, buttonWidth, 44);
					_logoutButton.AutoresizingMask  = UIViewAutoresizing.FlexibleBottomMargin | 
						UIViewAutoresizing.FlexibleRightMargin |
						UIViewAutoresizing.FlexibleWidth;
					_logoutButton.TouchUpInside += OnLogout;
					_logoutButton.Hidden = _controller.HideTheDamnButtons;

					_deleteButton = UIButton.FromType (UIButtonType.RoundedRect);
					_deleteButton.SetTitle ("Delete", UIControlState.Normal);
					_deleteButton.Frame = new RectangleF (tableCenter + 3, 15, buttonWidth, 44);
					_deleteButton.AutoresizingMask  = UIViewAutoresizing.FlexibleBottomMargin | 
						UIViewAutoresizing.FlexibleLeftMargin |
							UIViewAutoresizing.FlexibleWidth;
					_deleteButton.TouchUpInside += OnDeleteUser;
					_deleteButton.Hidden = _controller.HideTheDamnButtons;


					UIView newView = new UIView (new RectangleF (0, 0, tableWidth, 44));
					newView.AutoresizingMask = UIViewAutoresizing.All;
					newView.Add (_logoutButton);
					newView.Add (_deleteButton);

					return newView;
				}
				else
				{
					if (!this.UserProfile.IsLocalUser)
					{
						float buttonWidth = contentWidth;
						
						_addContactButton = UIButton.FromType (UIButtonType.RoundedRect);
						_addContactButton.SetTitle ("Add Contact", UIControlState.Normal);
						_addContactButton.Frame = new RectangleF (tableMargin, 15, buttonWidth, 44);
						_addContactButton.AutoresizingMask  = UIViewAutoresizing.FlexibleBottomMargin | 
							UIViewAutoresizing.FlexibleRightMargin |
								UIViewAutoresizing.FlexibleWidth;
						_addContactButton.TouchUpInside += OnAddContact;;
						_addContactButton.Hidden = _controller.HideTheDamnButtons;
						
						UIView newView = new UIView (new RectangleF (0, 0, tableWidth, 44));
						newView.AutoresizingMask = UIViewAutoresizing.All;
						newView.Add (_addContactButton);
						
						return newView;
					}
				}
			}

			return null;
		}

		void OnAddContact (object sender, EventArgs e)
		{
			ABNewPersonViewController abController = new ABNewPersonViewController ();

			ABPerson person = new ABPerson ();
			string firstName = string.Empty;
			string lastName = string.Empty;
			if (!string.IsNullOrEmpty (UserProfile.FullName))
			{
				string [] names = UserProfile.FullName.Split ();

				if (names.Length > 0)
					firstName = names[0];

				if (names.Length > 1)
					lastName = UserProfile.FullName.Substring (firstName.Length);
			}

			person.FirstName = firstName;
			person.LastName = lastName;

			if (!string.IsNullOrEmpty (UserProfile.Company))
			{
				person.Organization = UserProfile.Company;
			}

			if (!string.IsNullOrEmpty (UserProfile.Phone))
			{
				ABMutableMultiValue<string> phones = new ABMutableStringMultiValue();
				phones.Add(UserProfile.Phone, ABPersonPhoneLabel.Main);
				person.SetPhones(phones);
			}

			if (!string.IsNullOrEmpty (UserProfile.Email))
			{
				ABMutableMultiValue<string> emails = new ABMutableStringMultiValue();
				emails.Add(UserProfile.Email, null);
				person.SetEmails(emails);
			}

			// Get any image from cache
			byte [] data = Engine.Instance.ImageCache.FindAny (UserProfile);

			if (data != null && data.Length > 0)
			{
				person.Image = NSData.FromArray(data); 
			}

			abController.DisplayedPerson  = person;

			abController.NewPersonComplete += delegate {
				_controller.NavigationController.PopViewControllerAnimated (true);
			};

			_controller.NavigationController.PushViewController (abController, true);
		}

		void OnDeleteUser (object sender, EventArgs e)
		{
			UIAlertView alertView = new UIAlertView (
				"Delete User?", "Are you sure you want to delete your accout?", 
				null, null, new string [] {"Yes", "No"});

			alertView.CancelButtonIndex = 1;
			
			alertView.Clicked += (object sender2, UIButtonEventArgs e2) => {
				if (e2.ButtonIndex == 0)
				{
					Engine.Instance.UserAccess.DeleteUser ();
					this._controller.NavigationController.PopViewControllerAnimated (true);
				}
			};

			alertView.Show ();
		}

		void OnLogout (object sender, EventArgs e)
		{
			UIAlertView alertView = new UIAlertView (
				"Logout User?", "Are you sure you want to logout?", 
				null, null, new string [] {"Yes", "No"});
			
			alertView.CancelButtonIndex = 1;
			
			alertView.Clicked += (object sender2, UIButtonEventArgs e2) => {
				if (e2.ButtonIndex == 0)
				{
					Engine.Instance.UserAccess.Logout ();
					this._controller.NavigationController.PopViewControllerAnimated (true);
				}
			};

			alertView.Show ();
		}

		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.None;
		}

		public override bool ShouldIndentWhileEditing (UITableView tableView, NSIndexPath indexPath)
		{
			return false;
		}

		public static bool canUsePhone ()
		{
			return UIApplication.SharedApplication.CanOpenUrl(
				new NSUrl ("tel://412-867-5309"));
		}

		public static bool canUseEMail ()
		{
			return UIApplication.SharedApplication.CanOpenUrl(
				new NSUrl ("mailto:?to=fun@xamarin.com"));
		}

		bool validEMail (string value)
		{
			if (string.IsNullOrWhiteSpace (value))
				return false;

			return true;
		}

		bool validPhone (string value)
		{
			if (string.IsNullOrWhiteSpace (value))
				return false;

			return true;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == 2 && indexPath.Row == 1)
			{
				if (!canUsePhone ())
					return;

				CustomUITableViewCell cell = tableView.CellAt (indexPath) as CustomUITableViewCell;
				NameValueCell nameCell = cell.CustomView as NameValueCell;

				cell.Selected = false;
				string value = nameCell.ValueLabel.Text;

				if (!validPhone (value))
					return;

				UIAlertView view = new UIAlertView ("Call Number?", value, 
				                                    null, null, new string [] {"Yes", "No"}); 
				view.CancelButtonIndex = 1;

				view.Clicked += (object sender, UIButtonEventArgs e) => {
					if (e.ButtonIndex == 0)
					UIApplication.SharedApplication.OpenUrl (
							new NSUrl (string.Format ("tel://{0}", value)));
				};

				view.Show ();
			}
			else if (indexPath.Section == 2 && indexPath.Row == 0)
			{
				if (!canUseEMail ())
					return;

				CustomUITableViewCell cell = tableView.CellAt (indexPath) as CustomUITableViewCell;
				NameValueCell nameCell = cell.CustomView as NameValueCell;

				cell.Selected = false;
				string value = nameCell.ValueLabel.Text;
				
				if (!validEMail (value))
					return;

				UIAlertView view = new UIAlertView ("Create E-Mail?", value, 
				                                    null, null, new string [] {"Yes", "No"}); 
				view.CancelButtonIndex = 1;
				
				view.Clicked += (object sender, UIButtonEventArgs e) => {
					if (e.ButtonIndex == 0)
						UIApplication.SharedApplication.OpenUrl (
							new NSUrl (string.Format ("mailto:?to={0}", value)));
				};
				
				view.Show ();
			}
		}
	}

	public class LocalUserProfileDataSource : UITableViewDataSource
	{
		ProfileViewController _controller;

		public LocalUserProfileDataSource (ProfileViewController controller, User profile)
		{
			UserProfile = profile;
			_controller = controller;
		}
		
		public User UserProfile {get; set;}
		
		#region UITableViewDataSource	

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 3;
		}
		
		public override int RowsInSection (UITableView tableView, int section)
		{
			if (section == 0)
				return 1;

			if (section == 1)
				return 2;

			if (section == 2)
				return 2;

			throw new IndexOutOfRangeException ("section out of range");
		}		
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell;

			switch (indexPath.Section)
			{
			case 0:
				cell = GetCellSectionOne (tableView, indexPath);
				break;
			case 1:
				cell = GetCellSectionTwo (tableView, indexPath);
				break;
			case 2:
				cell = GetCellSectionThree (tableView, indexPath);
				break;
				
			default:
				throw new IndexOutOfRangeException ("indexPath.Section out of range.");
			}

			return cell;
		}
		
		#endregion

		private UITableViewCell GetCellSectionOne (UITableView tableView, NSIndexPath indexPath)
		{
			UserProfileHeaderCell headerCell = new UserProfileHeaderCell (UserProfile);

			headerCell.OnImageChangeRequest += (originalImage) => {
				AvatarSelectorController ctrl;
				this._controller.NavigationController.PushViewController (ctrl = new AvatarSelectorController (originalImage), true);
				ctrl.SelectorView.ImageApplied += (image) => {
					headerCell.ImageView.Image = image; 
					PostNewAvatar (image);
				};
			};

			return headerCell.LoadCell (tableView);
		}

		void PostNewAvatar (UIImage image)
		{
			NSData data = image.AsPNG ();

			byte[] dataBytes = new byte[data.Length];
			System.Runtime.InteropServices.Marshal.Copy(data.Bytes, dataBytes, 0, Convert.ToInt32(data.Length));

			Engine.Instance.AvatarAccess.PostNewAvatar (dataBytes, (result) => {
				if (result.Exceptin != null)
				{
					_controller.BeginInvokeOnMainThread (delegate {
						_controller.RefreshHeaderCell ();
					});
				}
			});
		}

		private UITableViewCell GetCellSectionTwo (UITableView tableView, NSIndexPath indexPath)
		{
			NameValueCell cell;
			
			switch (indexPath.Row)
			{
			case 0:
				cell = new NameValueCell ("company", () => UserProfile.Company, v => UserProfile.Company= v);
				cell.ValueTextField.AutocapitalizationType = UITextAutocapitalizationType.Words;
				cell.ValueTextField.AutocorrectionType = UITextAutocorrectionType.No;
				break;
			case 1:
				cell = new NameValueCell ("title", () => UserProfile.Title, v => UserProfile.Title= v);
				cell.ValueTextField.AutocapitalizationType = UITextAutocapitalizationType.Words;
				cell.ValueTextField.AutocorrectionType = UITextAutocorrectionType.No;
				break;
				
			default:
				throw new IndexOutOfRangeException ("indexPath.Row out of range.");
			}

			return cell.LoadCell (tableView);
		}

		private UITableViewCell GetCellSectionThree (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell;
			NameValueCell nameValueCell;
			
			switch (indexPath.Row)
			{
			case 0:
				nameValueCell = new NameValueCell ("e-mail", () => UserProfile.Email, v => UserProfile.Email= v);
				nameValueCell.ValueTextField.KeyboardType = UIKeyboardType.EmailAddress;
				nameValueCell.ValueTextField.AutocapitalizationType = UITextAutocapitalizationType.None;
				nameValueCell.ValueTextField.AutocorrectionType = UITextAutocorrectionType.No;
				cell = nameValueCell.LoadCell (tableView);
				if (LocalUserProfileDelegate.canUsePhone ())
					cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
				break;
			case 1:
				nameValueCell = new NameValueCell ("phone", () => UserProfile.Phone, v => UserProfile.Phone= v);
				nameValueCell.ValueTextField.KeyboardType = UIKeyboardType.PhonePad;
				nameValueCell.ValueTextField.AutocorrectionType = UITextAutocorrectionType.No;
				nameValueCell.ValueTextField.ShouldChangeCharacters = PhoneNumberHelper.ShoudChange;
				cell = nameValueCell.LoadCell (tableView);
				if (LocalUserProfileDelegate.canUseEMail ())
					cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
				break;
				
			default:
				throw new IndexOutOfRangeException ("indexPath.Row out of range.");
			}
			
			return cell;
		}
	}
}
