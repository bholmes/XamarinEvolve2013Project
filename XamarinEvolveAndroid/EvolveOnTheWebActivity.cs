
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
using Android.Webkit;

namespace XamarinEvolveAndroid
{
	[Activity (Label = "EvolveOnTheWebActivity")]			
	public class EvolveOnTheWebActivity : Activity
	{
		string url;
		WebView mWebView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.Title = this.GetString (Resource.String.EvolveontheWeb);
			SetContentView (Resource.Layout.EvolveOnTheWebLayout);

			url = "http://xamarin.com/evolve"; 
			mWebView = (WebView)FindViewById<WebView> (Resource.Id.webView1);
			//mWebView.SetWebViewClient (new ViewClient (this));
			mWebView.Settings.JavaScriptEnabled = (true);
			mWebView.Settings.PluginsEnabled = (true);
			mWebView.Settings.LoadWithOverviewMode = true;
			mWebView.Settings.UseWideViewPort = true;
			mWebView.Settings.BuiltInZoomControls = true;
			mWebView.LoadUrl (url);
		}
	}
}

