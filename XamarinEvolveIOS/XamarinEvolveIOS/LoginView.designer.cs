// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace XamarinEvolveIOS
{
	partial class LoginView
	{
		[Outlet]
		public MonoTouch.UIKit.UITextField UsernameField { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UITextField PasswordField { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UILabel RetypePasswordLabel { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UITextField RetypePasswordField { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UIButton LoginButton { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UIButton CancelButton { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UIScrollView ScrollView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (UsernameField != null) {
				UsernameField.Dispose ();
				UsernameField = null;
			}

			if (PasswordField != null) {
				PasswordField.Dispose ();
				PasswordField = null;
			}

			if (RetypePasswordLabel != null) {
				RetypePasswordLabel.Dispose ();
				RetypePasswordLabel = null;
			}

			if (RetypePasswordField != null) {
				RetypePasswordField.Dispose ();
				RetypePasswordField = null;
			}

			if (LoginButton != null) {
				LoginButton.Dispose ();
				LoginButton = null;
			}

			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}
		}
	}
}
