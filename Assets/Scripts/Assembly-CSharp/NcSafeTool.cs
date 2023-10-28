using UnityEngine;

public class NcSafeTool : MonoBehaviour
{
	public static bool m_bShuttingDown;

	public static bool m_bLoadLevel;

	private static NcSafeTool s_Instance;

	private static void Instance()
	{
		if (s_Instance == null)
		{
			GameObject gameObject = GameObject.Find("_GlobalManager");
			if (gameObject == null)
			{
				gameObject = new GameObject("_GlobalManager");
			}
			else
			{
				s_Instance = (NcSafeTool)gameObject.GetComponent(typeof(NcSafeTool));
			}
			if (s_Instance == null)
			{
				s_Instance = (NcSafeTool)gameObject.AddComponent(typeof(NcSafeTool));
			}
		}
	}

	public static bool IsSafe()
	{
		return !m_bShuttingDown && !m_bLoadLevel;
	}

	public static Object SafeInstantiate(Object original)
	{
		if (m_bShuttingDown)
		{
			return null;
		}
		if (s_Instance == null)
		{
			Instance();
		}
		return Object.Instantiate(original);
	}

	public static Object SafeInstantiate(Object original, Vector3 position, Quaternion rotation)
	{
		if (m_bShuttingDown)
		{
			return null;
		}
		if (s_Instance == null)
		{
			Instance();
		}
		return Object.Instantiate(original, position, rotation);
	}

	public static void LoadLevel(int nLoadLevel)
	{
		if (!m_bShuttingDown)
		{
			if (s_Instance == null)
			{
				Instance();
			}
			m_bLoadLevel = true;
			Application.LoadLevel(nLoadLevel);
			m_bLoadLevel = false;
		}
	}

	public void OnApplicationQuit()
	{
		m_bShuttingDown = true;
	}
}
