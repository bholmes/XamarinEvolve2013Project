
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

namespace XamarinEvolveAndroid
{
	[Activity (Label = "UsersActivity")]			
	public class UsersActivity : ListActivity
	{
		UsersAdapter _usersAdapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.Title = this.GetString (Resource.String.Attendees);

			ListAdapter = _usersAdapter = new UsersAdapter (this);
		}

		private class UsersAdapter : BaseAdapter<User>
		{
			readonly Activity _context;
			
			UserList Users {get;set;}
			
			public UsersAdapter (Activity context)
			{
				_context = context;
				
				Users = new UserList ();

				ProgressDialog mDialog = new ProgressDialog(_context);
				mDialog.SetMessage(_context.GetString (Resource.String.Loading));
				mDialog.SetCancelable(false);
				mDialog.Show();
				
				Engine.Instance.UserAccess.GetUsers ((userList) => {
					_context.RunOnUiThread ( delegate {
						Users = userList.UserList;
						NotifyDataSetChanged ();
						mDialog.Cancel ();
					});
				});
			}
			
			#region implemented abstract members of BaseAdapter		
			public override long GetItemId (int position)
			{
				return position;
			}		
			
			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				var view = _context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
				
				view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = Users[position].FullName;
				
				return view;
			}		
			
			public override int Count {
				get 
				{
					return Users.Count;
				}
			}		
			#endregion		
			#region implemented abstract members of BaseAdapter		
			public override User this [int position] {
				get 
				{
					return Users[position];
				}
			}
			#endregion
		}
	}
}

