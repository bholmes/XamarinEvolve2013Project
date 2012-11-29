using System;
using MonoTouch.AddressBookUI;
using MonoTouch.AddressBook;
using XamarinEvolveSSLibrary;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace XamarinEvolveIOS
{
	public class ContactHelper
	{
		public static void OnAddContact (UINavigationController navigationController, User user)
		{
			ABNewPersonViewController abController = new ABNewPersonViewController ();
			
			ABPerson person = new ABPerson ();
			string firstName = string.Empty;
			string lastName = string.Empty;
			if (!string.IsNullOrEmpty (user.FullName))
			{
				string [] names = user.FullName.Split ();
				
				if (names.Length > 0)
					firstName = names[0];
				
				if (names.Length > 1)
					lastName = user.FullName.Substring (firstName.Length);
			}
			
			person.FirstName = firstName;
			person.LastName = lastName;
			
			if (!string.IsNullOrEmpty (user.Company))
			{
				person.Organization = user.Company;
			}
			
			if (!string.IsNullOrEmpty (user.Phone))
			{
				ABMutableMultiValue<string> phones = new ABMutableStringMultiValue();
				phones.Add(user.Phone, ABPersonPhoneLabel.Main);
				person.SetPhones(phones);
			}
			
			if (!string.IsNullOrEmpty (user.Email))
			{
				ABMutableMultiValue<string> emails = new ABMutableStringMultiValue();
				emails.Add(user.Email, null);
				person.SetEmails(emails);
			}
			
			// Get any image from cache
			byte [] data = Engine.Instance.ImageCache.FindAny (user);
			
			if (data != null && data.Length > 0)
			{
				person.Image = NSData.FromArray(data); 
			}
			
			abController.DisplayedPerson  = person;
			
			abController.NewPersonComplete += delegate {
				navigationController.PopViewControllerAnimated (true);
			};
			
			navigationController.PushViewController (abController, true);
		}

		public static bool CanCallPerson (string number)
		{
			if (string.IsNullOrEmpty (number))
				return false;

			return UIApplication.SharedApplication.CanOpenUrl(
				new NSUrl (string.Format ("tel://{0}", number)));
		}

		public static void CallPerson (string number)
		{
			if (!CanCallPerson (number))
				return;

			UIAlertView view = new UIAlertView ("Call Number?", number, 
			                                    null, null, new string [] {"Yes", "No"}); 
			view.CancelButtonIndex = 1;
			
			view.Clicked += (object sender, UIButtonEventArgs e) => {
				if (e.ButtonIndex == 0)
					UIApplication.SharedApplication.OpenUrl (
						new NSUrl (string.Format ("tel://{0}", number)));
			};
			
			view.Show ();
		}

		public static bool CanEMailPerson (string address)
		{
			if (string.IsNullOrEmpty (address))
				return false;
			
			return UIApplication.SharedApplication.CanOpenUrl(
				new NSUrl (string.Format ("mailto:?to={0}", address)));
		}

		static public void EmailPerson (string address)
		{
			if (!CanEMailPerson (address))
				return;

			UIAlertView view = new UIAlertView ("Create E-Mail?", address, 
			                                    null, null, new string [] {"Yes", "No"}); 
			view.CancelButtonIndex = 1;
			
			view.Clicked += (object sender, UIButtonEventArgs e) => {
				if (e.ButtonIndex == 0)
					UIApplication.SharedApplication.OpenUrl (
						new NSUrl (string.Format ("mailto:?to={0}", address)));
			};
			
			view.Show ();
		}

	}
}

