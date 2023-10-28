using UnityEngine;

public class SLOTGameSingleton<T> : MonoBehaviour where T : Component
{
	private static T the_instance;

	public static T GetInstance()
	{
		if ((Object)the_instance == (Object)null)
		{
			the_instance = (T)Object.FindObjectOfType(typeof(T));
		}
		if (Application.isPlaying && (Object)the_instance == (Object)null)
		{
			SLOTGame.GetInstance();
			the_instance = (T)Object.FindObjectOfType(typeof(T));
			if ((Object)the_instance == (Object)null)
			{
				GameObject gameObject = new GameObject();
				if ((bool)gameObject)
				{
					the_instance = (T)gameObject.AddComponent(typeof(T));
				}
				if (gameObject != null)
				{
					gameObject.transform.position = new Vector3(999999f, 999999f, 999999f);
					gameObject.name = "AutomaticallyCreated" + typeof(T).ToString();
					Object.DontDestroyOnLoad(gameObject);
				}
			}
		}
		return the_instance;
	}
}
