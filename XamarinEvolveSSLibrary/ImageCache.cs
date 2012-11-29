using System;
using System.Collections.Generic;

namespace XamarinEvolveSSLibrary
{
	public class ImageCache
	{
		Dictionary<ImageCacheItem, ImageCacheItem> _cache = new Dictionary<ImageCacheItem, ImageCacheItem> ();
		object _cacheLock = new object ();
		TimeSpan _timeSpan = new TimeSpan (0, 5, 0);
		DateTime _ancient = new DateTime ();

		public byte [] FindOrLoad (User user, int size, Action<AvatarAccess.GetAvatarResult> onComplete)
		{
			ImageCacheItem item;
			ImageCacheItem newItem = new ImageCacheItem {
				UserName = user.UserName, Size = size};

			if (!string.IsNullOrEmpty(user.UserName))
			{
				lock (_cacheLock)
				{
					if(_cache.TryGetValue(newItem, out item))
					{
						if (item.Time > (DateTime.UtcNow - _timeSpan))
							return item.Data;
					}
				}
			}

			return Engine.Instance.AvatarAccess.GetAvararForUser (user, size, (result) => {
				AddImage (newItem, result, onComplete);
			});
		}

		public byte [] FindAny (User user)
		{
			byte [] data = new byte[0];
			int bestSize = 0;

			lock (_cacheLock)
			{
				foreach (KeyValuePair <ImageCacheItem, ImageCacheItem> iter in _cache)
				{
					if (iter.Key.UserName.Equals (user.UserName))
					{
						if (iter.Key.Size > bestSize)
						{
							bestSize = iter.Key.Size;
							data = iter.Key.Data;
						}
					}
				}
			}

			return data;
		}

		public void TouchUser (User user)
		{
			lock (_cacheLock)
			{
				foreach (KeyValuePair <ImageCacheItem, ImageCacheItem> iter in _cache)
				{
					if (iter.Key.UserName.Equals (user.UserName))
						iter.Key.Time = _ancient;
				}
			}
		}

		void AddImage (ImageCacheItem newItem, AvatarAccess.GetAvatarResult result, Action<AvatarAccess.GetAvatarResult> onComplete)
		{
			ImageCacheItem item;
			
			if (result.Data != null && !string.IsNullOrEmpty(newItem.UserName))
			{
				lock (_cacheLock)
				{
					if(!_cache.TryGetValue(newItem, out item))
					{
						newItem.Data = result.Data;
						newItem.Time = DateTime.UtcNow;

						_cache.Add (newItem, newItem);
					}
					else
					{
						item.Data = result.Data;
						item.Time = DateTime.UtcNow;
					}
				}
			}

			if (onComplete != null)
				onComplete (result);
		}

		private class ImageCacheItem
		{
			public string UserName {get;set;}
			public int Size {get;set;}
			public DateTime Time {get;set;}
			public byte [] Data {get;set;}

			public override bool Equals (object obj)
			{
				ImageCacheItem item = obj as ImageCacheItem;
				if (item != null)
				{
					return Size == item.Size &&
						UserName.Equals (item.UserName);
				}

				return false;
			}

			public override int GetHashCode ()
			{
				return UserName.GetHashCode ();
			}
		}
	}
}

