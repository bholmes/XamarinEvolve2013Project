using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using System.Drawing;

namespace XamarinEvolveIOS
{
	[Register("LoginView")]
	public partial class LoginView : UIView
	{
		public LoginView (IntPtr ptr) : base (ptr)
		{

		}

		private void InitMembers ()
		{
			UsernameField.ShouldReturn = delegate {
				UsernameField.ResignFirstResponder ();
				return true;
			};
			
			PasswordField.ShouldReturn = delegate {
				PasswordField.ResignFirstResponder ();
				return true;
			};
			
			RetypePasswordField.ShouldReturn = delegate {
				RetypePasswordField.ResignFirstResponder ();
				return true;
			};

			ScrollView.ContentSize = new SizeF (Frame.Width, CancelButton.Frame.Bottom + 20);
			ScrollView.ShowsVerticalScrollIndicator = false;
		}

		public static LoginView Create (NSObject owner)
		{
			NSArray array = NSBundle.MainBundle.LoadNib ("LoginView", owner, null);
			LoginView returnView = Runtime.GetNSObject (array.ValueAt (0)) as LoginView;

			returnView.InitMembers ();			

			return returnView;
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
		}
	}
}

