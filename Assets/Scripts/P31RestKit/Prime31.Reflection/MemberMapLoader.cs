using System;

namespace Prime31.Reflection
{
	public delegate void MemberMapLoader(Type type, SafeDictionary<string, CacheResolver.MemberMap> memberMaps);
}
