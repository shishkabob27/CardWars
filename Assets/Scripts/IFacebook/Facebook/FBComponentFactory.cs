using UnityEngine;

namespace Facebook
{
	public class FBComponentFactory
	{
		public const string gameObjectName = "UnityFacebookSDKPlugin";

		private static GameObject facebookGameObject;

		private static GameObject FacebookGameObject
		{
			get
			{
				if (facebookGameObject == null)
				{
					facebookGameObject = new GameObject("UnityFacebookSDKPlugin");
				}
				return facebookGameObject;
			}
		}

		public static T GetComponent<T>(IfNotExist ifNotExist = IfNotExist.AddNew) where T : MonoBehaviour
		{
			GameObject gameObject = FacebookGameObject;
			T val = gameObject.GetComponent<T>();
			if ((Object)val == (Object)null && ifNotExist == IfNotExist.AddNew)
			{
				val = gameObject.AddComponent<T>();
			}
			return val;
		}

		public static T AddComponent<T>() where T : MonoBehaviour
		{
			return FacebookGameObject.AddComponent<T>();
		}
	}
}
