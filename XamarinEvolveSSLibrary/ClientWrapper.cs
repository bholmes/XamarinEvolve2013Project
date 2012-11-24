using System;
using ServiceStack.ServiceClient.Web;

namespace XamarinEvolveSSLibrary
{
	public class ClientWrapper
	{
		object _clientLock = new object ();
		JsonServiceClient _client = new JsonServiceClient(SystemConstants.WebServiceBaseURL);
		
		public object ClientLock 
		{
			get 
			{
				return _clientLock;
			}
		}
		
		public JsonServiceClient Client 
		{
			get 
			{
				return _client;
			}
		}
	}
}

