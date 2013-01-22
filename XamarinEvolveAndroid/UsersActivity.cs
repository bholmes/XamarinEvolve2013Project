
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

		protected override void OnListItemClick(Android.Widget.ListView l, Android.Views.View v, int position, long id)
		{
			var intent = new Intent(this, typeof(UserDetailActivity));

			intent.PutExtra ("USER_NAME", _usersAdapter[position].UserName);

			intent.PutExtra ("USER_FULL_NAME", _usersAdapter[position].FullName);
			intent.PutExtra ("USER_CITY", _usersAdapter[position].City);

			intent.PutExtra ("USER_COMPANY", _usersAdapter[position].Company);
			intent.PutExtra ("USER_TITLE", _usersAdapter[position].Title);

			intent.PutExtra ("USER_EMAIL", _usersAdapter[position].Email);
			intent.PutExtra ("USER_PHONE", _usersAdapter[position].Phone);

			this.StartActivity(intent);
			return;
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
				var view = _context.LayoutInflater.Inflate(Resource.Layout.ImageAndSubtitleItem, null);
				
				view.FindViewById<TextView>(Resource.Id.Text1).Text = Users[position].FullName;
				view.FindViewById<TextView>(Resource.Id.Text2).Text = Users[position].City;

				var avatar = view.FindViewById<ImageView>(Resource.Id.Icon);

				Engine.Instance.AvatarAccess.GetAvararForUser (Users[position], avatar.Width, (result) => {
					_context.RunOnUiThread ( delegate {
						avatar.SetImageBitmap (
							BitmapFactory.DecodeByteArray (result.Data, 0, result.Data.Length));
					});
				});

				using (var stream = _context.Resources.OpenRawResource (Resource.Drawable.blankavatar))
				{
					byte [] buff;

					using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
					{
						stream.CopyTo(ms);
						buff = ms.ToArray();
					}

					avatar.SetImageBitmap (
						BitmapFactory.DecodeByteArray (buff, 0, buff.Length));
				}
				
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

