using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace XamarinEvolveIOS
{
	public class LocalProfileViewController : ProfileViewController
	{
		enum Mode
		{
			Login,
			NewUser
		}

		LoginView _loginView;
		UIBarButtonItem _editButton;
		string _originalTitle;
		Mode _mode = Mode.Login;

		public LocalProfileViewController () : base (Engine.Instance.GetCurrentUser())
		{
		}

		public override void LoadView ()
		{
			base.LoadView ();

			//Create the login view and hide it
			_loginView = LoginView.Create (this);
			_loginView.Frame = View.Bounds;
			_loginView.AutoresizingMask = UIViewAutoresizing.All;
			_loginView.Hidden = true;
			_loginView.LoginButton.TouchUpInside += LoginButtonClicked;

			_loginView.CancelButton.TouchUpInside += CancelButtonClicked;
			this.View.Add (_loginView);

			if (Engine.Instance.GetCurrentUser().IsAnonymousUser)
				ShowLoginScreen ();
		}

		void ChangeMode (Mode newMode)
		{
			if (newMode == _mode)
				return;

			_mode = newMode;

			if (_mode == Mode.Login)
			{
				this.Title = "Login";
				this._loginView.LoginButton.SetTitle ("Login", UIControlState.Normal);
				this._loginView.CancelButton.SetTitle ("New User", UIControlState.Normal);
				_loginView.RetypePasswordLabel.Hidden = true;
				_loginView.RetypePasswordField.Hidden = true;
			}
			else
			{
				this.Title = "New User";
				this._loginView.LoginButton.SetTitle ("Create", UIControlState.Normal);
				this._loginView.CancelButton.SetTitle ("Cancel", UIControlState.Normal);
				_loginView.RetypePasswordLabel.Hidden = false;
				_loginView.RetypePasswordField.Hidden = false;
			}
		}

		void SetBusyState (bool busy)
		{
			_loginView.LoginButton.Enabled = !busy;
			_loginView.CancelButton.Enabled = !busy;
			_loginView.UsernameField.Enabled = !busy;
			_loginView.PasswordField.Enabled = !busy;
			_loginView.RetypePasswordField.Enabled = !busy;
			this.NavigationItem.SetHidesBackButton (busy, true);

			if (busy)
			{
				_loginView.BusyIndicator.StartAnimating ();
			}
			else
			{
				_loginView.BusyIndicator.StopAnimating ();
			}
		}

		void LoginButtonClicked (object sender, EventArgs e)
		{
			if (_mode == Mode.Login)
				TryLoginUser ();
			else
				TryCreateUser ();
		}

		void CancelButtonClicked (object sender, EventArgs e)
		{
			if (_mode == Mode.Login)
				ChangeMode (Mode.NewUser);
			else
				ChangeMode (Mode.Login);
		}

		void TryLoginUser()
		{
			SetBusyState (true);

			Engine.Instance.UserLogin (_loginView.UsernameField.Text,
		                                       _loginView.PasswordField.Text,
			                           (loginResult) => {
				this.InvokeOnMainThread (delegate {
					AsyncLoginComplete (loginResult);
				});
			});
		}

		void AsyncLoginComplete (Engine.UserLoginResult loginResult)
		{
			SetBusyState (false);

			if (loginResult.User != null) 
			{
				HideLoginScreen ();
				this.CurrentUser = loginResult.User;
				this.TableView.ReloadData ();
				this.SetEditing (false, false);
				return;
			}

			UIActionSheet actionSheet = new UIActionSheet ("Login Failed");
			actionSheet.AddButton ("Retry");
			
			actionSheet.AddButton ("Register New User");
			
			actionSheet.Clicked += (object sender, UIButtonEventArgs e) => {
				if (e.ButtonIndex == 1)
				{
					ChangeMode (Mode.NewUser);
				}
			};
			
			actionSheet.ShowInView (_loginView);
		}

		void TryCreateUser ()
		{
			SetBusyState (true);
			
			Engine.Instance.CreateNewUser (_loginView.UsernameField.Text,
			                           _loginView.PasswordField.Text,
			                           (loginResult) => {
				this.InvokeOnMainThread (delegate {
					AsyncNewUserComplete (loginResult);
				});
			});
		}

		void AsyncNewUserComplete (Engine.UserLoginResult loginResult)
		{
			SetBusyState (false);
			
			if (loginResult.User != null) 
			{
				HideLoginScreen ();
				this.CurrentUser = loginResult.User;
				this.TableView.ReloadData ();
				this.SetEditing (true, false);
				return;
			}
			
			using (UIAlertView alert = new UIAlertView ("Error", "Something went wrong", null, "OK", null))
			{
				alert.Show ();
			}
		}

		void ShowLoginScreen ()
		{
			_loginView.Hidden = false;
			_originalTitle = this.Title;
			this.Title = "Login";
			this.TableView.ScrollEnabled = false;
			_editButton = this.NavigationItem.RightBarButtonItem;
			this.NavigationItem.RightBarButtonItem = null;
		}

		void HideLoginScreen ()
		{
			UIView.Animate (.5f, 0, UIViewAnimationOptions.CurveEaseInOut, delegate {
				_loginView.Frame = new RectangleF (_loginView.Frame.X,
				                                   _loginView.Frame.Height,
				                                   _loginView.Frame.Width,
				                                   0);
			}, 
			delegate {
				_loginView.Hidden = true;
				_loginView.Frame = this.View.Bounds;
			});
			this.TableView.ScrollEnabled = true;
			this.NavigationItem.RightBarButtonItem = _editButton;
			this.Title = _originalTitle;
		}
	}
}

