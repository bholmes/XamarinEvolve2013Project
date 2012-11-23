using System;
#if IOS
namespace XamarinEvolveSSLibrary
{
	internal class AliasAttribute : Attribute
	{
		internal AliasAttribute (string alias){}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	internal class RouteAttribute : Attribute
	{
		internal RouteAttribute (string route){}
	}
}
#endif

