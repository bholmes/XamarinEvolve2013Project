using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace XamarinEvolveIOS
{
	public class LocalProfileViewController : ProfileViewController
	{
		UIView _loginView;
		UIBarButtonItem _editButton;
		string _originalTitle;

		UITextField _usernameField;
		UITextField _passwordField;

		public LocalProfileViewController () : base (Engine.Instance.GetCurrentUser())
		{
		}

		public override void LoadView ()
		{
			base.LoadView ();

			//Creatre the login view and hide it
			_loginView = new UIView (this.View.Bounds);
			_loginView.BackgroundColor = UIColor.White;
			_loginView.AutoresizingMask = UIViewAutoresizing.All;
			_loginView.Hidden = true;
			this.View.Add (_loginView);

			//Add username text field
			_usernameField = new UITextField (new RectangleF (20, 20, 
			                                                  this.View.Bounds.Width - 40, 30));
			_usernameField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | 
				UIViewAutoresizing.FlexibleBottomMargin;
			_usernameField.Text = "name";
			_loginView.Add (_usernameField);

			// Add login button
			UIButton loginButton = UIButton.FromType (UIButtonType.RoundedRect);
			loginButton.SetTitle ("Login", UIControlState.Normal);
			loginButton.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | 
				UIViewAutoresizing.FlexibleTopMargin;
			loginButton.Frame = new RectangleF (20, this.View.Bounds.Height - 64, 
			                                    this.View.Bounds.Width - 40, 44);
			loginButton.TouchUpInside += LoginButtonClicked;
			_loginView.Add (loginButton);

			ShowLoginScreen ();
		}

		void LoginButtonClicked (object sender, EventArgs e)
		{
			HideLoginScreen ();
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

