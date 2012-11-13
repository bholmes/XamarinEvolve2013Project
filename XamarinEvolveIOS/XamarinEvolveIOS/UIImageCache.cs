using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace XamarinEvolveIOS
{
	public class UIImageCache
	{
		static UIImageCache _instance = new UIImageCache ();
		
		public delegate void OnImageLoadedDelegate (UIImage image);
		private List<UIImageCacheItem> _list = new List<UIImageCacheItem> ();
		object _lock = new object ();
		
		private class UIImageCacheItem
		{
			public UIImageCacheItem (string url, OnImageLoadedDelegate OnImageLoaded, OnImageLoadedDelegate finalOnImageLoaded, UIImage defaultImage)
			{
				URL = url;
				
				if (OnImageLoaded != null)
					_delList.Add (OnImageLoaded);
				
				_finalOnImageLoaded = finalOnImageLoaded;
				
				new System.Threading.ThreadStart( () => {
					NSData data = NSData.FromUrl (new NSUrl (url));
					UIImage image = data == null ?
						defaultImage : UIImage.LoadFromData (data);
						
					List<OnImageLoadedDelegate> oldDelList;
					
					lock (_lock)
					{
						_image = image;
						oldDelList = _delList;
						_delList = new List<OnImageLoadedDelegate> ();
					}
					
					foreach (OnImageLoadedDelegate onImageLoaded in oldDelList)
					{
						onImageLoaded (_image);
					}
					
					if (_finalOnImageLoaded != null)
						_finalOnImageLoaded (_image);
					
				}).BeginInvoke (null, null);
			}
			
			public UIImage GetOrLoadImage (OnImageLoadedDelegate OnImageLoaded)
			{
				if (_image != null)
					return _image;
				
				lock (_lock)
				{
					if (_image != null)
						return _image;
					
					_delList.Add (OnImageLoaded);
				}
				
				return null;
			}
			
			public string URL {get;private set;}
			private UIImage _image;
			private List<OnImageLoadedDelegate> _delList = new List<OnImageLoadedDelegate> ();
			private OnImageLoadedDelegate _finalOnImageLoaded;
			object _lock = new object();
		}
		
		private UIImage internalGetOrLoadImage (string url, OnImageLoadedDelegate OnImageLoaded, UIImage defaultImage)
		{
			lock (_lock)
			{
				UIImageCacheItem foundItem = _list.Find (e=>e.URL.Equals (url));
				if(foundItem != null)
				{
					_list.Remove (foundItem);
					_list.Add (foundItem);
					return foundItem.GetOrLoadImage (OnImageLoaded);
				}
				
				_list.Add (new UIImageCacheItem (url, OnImageLoaded, (image) =>{
					cleanupLongList ();
				}, defaultImage));
			}
			
			return defaultImage;	
		}
		
		private void cleanupLongList ()
		{
			lock (_lock)
			{
				if (_list.Count > 250)
				{
					_list.RemoveRange (0, 250);
				}
			}
		}
		
		static public UIImage GetOrLoadImage (string url, OnImageLoadedDelegate OnImageLoaded, UIImage defaultImage)
		{
			return _instance.internalGetOrLoadImage (url, OnImageLoaded, defaultImage);	
		}
	}
}

