using System;

namespace Steamworks
{
	internal class CallbackIdentities
	{
		public static int GetCallbackIdentity(Type callbackStruct)
		{
			object[] customAttributes = callbackStruct.GetCustomAttributes(typeof(CallbackIdentityAttribute), false);
			int num = 0;
			if (num < customAttributes.Length)
			{
				CallbackIdentityAttribute callbackIdentityAttribute = (CallbackIdentityAttribute)customAttributes[num];
				return callbackIdentityAttribute.Identity;
			}
			throw new Exception("Callback number not found for struct " + callbackStruct);
		}
	}
}
