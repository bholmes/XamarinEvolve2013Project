
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XamarinEvolveSSLibrary;
using Android.Graphics;

namespace XamarinEvolveAndroid
{
	[Activity (Label = "UserDetailActivity")]			
	public class UserDetailActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			this.Title = this.GetString (Resource.String.Profile);

			SetContentView (Resource.Layout.UserDetailLayout);

			this.FindViewById<TextView> (Resource.Id.UserNameText).Text = Intent.GetStringExtra ("USER_FULL_NAME");
			this.FindViewById<TextView> (Resource.Id.UserCityText).Text = Intent.GetStringExtra ("USER_CITY");

			this.FindViewById<TextView> (Resource.Id.UserCompanyText).Text = Intent.GetStringExtra ("USER_COMPANY");
			this.FindViewById<TextView> (Resource.Id.UserTitleText).Text = Intent.GetStringExtra ("USER_TITLE");

			this.FindViewById<TextView> (Resource.Id.UserEMailText).Text = Intent.GetStringExtra ("USER_EMAIL");
			this.FindViewById<TextView> (Resource.Id.UserPhoneText).Text = Intent.GetStringExtra ("USER_PHONE");

			var avatar = FindViewById<ImageView>(Resource.Id.avatarImageView);
			
			Engine.Instance.AvatarAccess.GetAvararForUser (new User 
			                                               {UserName=Intent.GetStringExtra ("USER_NAME")},
			avatar.Width, (result) => {
				RunOnUiThread ( delegate {
					avatar.SetImageBitmap (
						BitmapFactory.DecodeByteArray (result.Data, 0, result.Data.Length));
				});
			});
		}
	}
}

