using System;
using MonoTouch.AddressBookUI;
using MonoTouch.AddressBook;
using XamarinEvolveSSLibrary;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Contacts;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace XamarinEvolveIOS
{
	public class ContactHelper
	{
		static bool _canAccessAddress = false;

		public delegate void OnAddContactCompletedDelegate();
		public OnAddContactCompletedDelegate OnAddContactCompleted {get;set;}

		public void AddContact (UINavigationController navigationController, User user)
		{
			if (!_canAccessAddress)
				StartOnAddContactAction (navigationController, user);
			else
			{
				new Func<int> (delegate {
					CheckForExistingAndContinue (navigationController, user, true);
					return 0;
				}).BeginInvoke (null,null);
			}
		}

		private void StartOnAddContactAction (UINavigationController navigationController, User user)
		{
			AddressBook book = new AddressBook ();
			book.RequestPermission ().ContinueWith (task => {
				navigationController.BeginInvokeOnMainThread (delegate {
					if (task.IsFaulted || task.IsCanceled || !task.Result)
					{
						ShowNoContactAccess ();
					}

					else 
					{
						_canAccessAddress = true;
						CheckForExistingAndContinue (navigationController, user, false);
					}
				});
				
			},TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void ShowNoContactAccess ()
		{
			UIAlertView alert = new UIAlertView ("Permission denied", "Contact access is not enabled for the Evolve application", null, "Close");
			alert.Show();

			if (OnAddContactCompleted != null)
				OnAddContactCompleted ();
		}

		private void CheckForExistingAndContinue (UINavigationController navigationController, 
		                                          User user, bool mainThreadRequired)
		{
			KeyValuePair <string, string> namePair = GetFirstAndLastName (user);

			string firstName = namePair.Key;
			string lastName = namePair.Value;

			AddressBook book = new AddressBook ();

			foreach (Contact contact in book)
			{
				if (contact.FirstName == firstName &&
				    contact.LastName == lastName)
				{
					if (!mainThreadRequired)
						AskShouldAddDuplicateAndContinue (navigationController, user);
					else
					{
						navigationController.BeginInvokeOnMainThread (delegate {
							AskShouldAddDuplicateAndContinue (navigationController, user);
						});
					}
					return;
				}
			}

			if (!mainThreadRequired)
				ShowAddContactController (navigationController, user);
			else
			{
				navigationController.BeginInvokeOnMainThread (delegate {
					ShowAddContactController (navigationController, user);
				});
			}
		}

		private bool AskShouldAddDuplicateAndContinue (UINavigationController navigationController, User user)
		{
			string message = string.Format 
				("You have a contact with the name {0}.  Do you still want to add?",
				 user.FullName);

			UIAlertView view = new UIAlertView ("Duplicate Found", message, 
			                                    null, null, new string [] {"Yes", "No"}); 
			view.CancelButtonIndex = 1;
			
			view.Clicked += (object sender, UIButtonEventArgs e) => {
				if (e.ButtonIndex == 0) 
				{
					ShowAddContactController (navigationController, user);
				}
				else
				{
					if (OnAddContactCompleted != null)
						OnAddContactCompleted ();
				}
			};
			
			view.Show ();

			return false;
		}

		public void ShowAddContactController (UINavigationController navigationController, User user)
		{
			ABNewPersonViewController abController = new ABNewPersonViewController ();
			
			ABPerson person = new ABPerson ();

			KeyValuePair <string, string> namePair = GetFirstAndLastName (user);
			
			person.FirstName = namePair.Key;
			person.LastName = namePair.Value;
			
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

			if (OnAddContactCompleted != null)
				OnAddContactCompleted ();
		}

		public static KeyValuePair<string, string> GetFirstAndLastName (User user)
		{
			string firstName = string.Empty;
			string lastName = string.Empty;
			if (!string.IsNullOrEmpty (user.FullName))
			{
				string [] names = user.FullName.Split ();
				
				if (names.Length > 0)
					firstName = names[0];
				
				if (names.Length > 1)
					lastName = user.FullName.Substring (firstName.Length);
				lastName = lastName.Trim ();
			}

			return new KeyValuePair<string, string> (firstName, lastName);
		}

		public static bool CanCallPerson (string number)
		{
			if (string.IsNullOrEmpty (number))
				return false;

			number = number.Replace (" ", "");

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

				number = number.Replace (" ", "");

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

			address = address.Replace (" ", "");
			
			return UIApplication.SharedApplication.CanOpenUrl(
				new NSUrl (string.Format ("mailto:?to={0}", address)));
		}

		public static void EmailPerson (string address)
		{
			if (!CanEMailPerson (address))
				return;

			UIAlertView view = new UIAlertView ("Create E-Mail?", address, 
			                                    null, null, new string [] {"Yes", "No"}); 
			view.CancelButtonIndex = 1;
			
			view.Clicked += (object sender, UIButtonEventArgs e) => {

				address = address.Replace (" ", "");

				if (e.ButtonIndex == 0)
					UIApplication.SharedApplication.OpenUrl (
						new NSUrl (string.Format ("mailto:?to={0}", address)));
			};
			
			view.Show ();
		}
	}
}

