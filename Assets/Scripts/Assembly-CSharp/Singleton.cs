using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T mInstance;

	public static T Instance
	{
		get
		{
			if (!(Object)mInstance)
			{
				mInstance = (T)Object.FindObjectOfType(typeof(T));
				if (!(Object)mInstance)
				{
					GameObject gameObject = new GameObject();
					if (gameObject != null)
					{
						mInstance = (T)gameObject.AddComponent(typeof(T));
						gameObject.transform.position = new Vector3(999999f, 999999f, 999999f);
						gameObject.name = "Automatically_" + typeof(T);
						Object.DontDestroyOnLoad(gameObject);
					}
				}
			}
			return mInstance;
		}
	}

	public void Destroy()
	{
		mInstance = (T)null;
	}
}
