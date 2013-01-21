using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace XamarinEvolveAndroid
{
	[Activity (MainLauncher = true)]
	public class HomeActivity1 : ListActivity
	{
		string[] items;
		
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			this.Title = "Xamarin Evolve 2013";
			this.Title = this.GetString (Resource.String.app_name);
			
			items = new string []{this.GetString (Resource.String.Attendees),
				this.GetString (Resource.String.EvolveontheWeb)
			};
			
			this.ListAdapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItem1, items);
		}

		protected override void OnListItemClick(Android.Widget.ListView l, Android.Views.View v, int position, long id)
		{
			var t = items[position];

			if (this.GetString (Resource.String.Attendees) == t)
			{
				var activity = new Intent(this, typeof(UsersActivity));
				this.StartActivity(activity);
				return;
			}

			if (this.GetString (Resource.String.EvolveontheWeb) == t)
			{
				var activity = new Intent(this, typeof(EvolveOnTheWebActivity));
				this.StartActivity(activity);
				return;
			}
		}
	}
}


