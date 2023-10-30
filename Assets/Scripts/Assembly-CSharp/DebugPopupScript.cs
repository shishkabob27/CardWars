using System.IO;
using UnityEngine;

public class DebugPopupScript : MonoBehaviour
{
	public static int NumPopups = 0;

	private static Vector3 SavePopupPosition = new Vector3(75f, 25f, 0f);

	private void Awake()
	{
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (!Application.isEditor || Application.isPlaying)
		{
			switch (type)
			{
			case LogType.Error:
			case LogType.Assert:
			case LogType.Exception:
				CreatePopup(logString + "\n" + stackTrace);
				break;
			case LogType.Warning:
			case LogType.Log:
				break;
			}
		}
	}

	private static Camera FindMyCamera(int objLayer)
	{
		int num = 1 << objLayer;
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera in allCameras)
		{
			if ((camera.cullingMask & num) != 0)
			{
				return camera;
			}
		}
		return null;
	}

	private GameObject CreatePopup(string errorMsg)
	{
		if (NumPopups > 3)
		{
			return null;
        }
		Object original = Resources.Load("Debug/Debug_Popup", typeof(GameObject)) as GameObject;
		GameObject gameObject = Instantiate(original) as GameObject;
		Camera camera = FindMyCamera(gameObject.layer);
		if (camera != null)
		{
			float z = camera.transform.position.z + camera.nearClipPlane + 0.5f;
			gameObject.transform.position = new Vector3(0f, 0f, z);
		}
		SQUtils.SetLabel(gameObject.transform, "Text", errorMsg);
		NumPopups++;
		return gameObject;
	}

	public static void CreateSavePopup(bool isSuccessful)
	{
		string path = ((!isSuccessful) ? "Debug/Save_Bad" : "Debug/Save_Good");
		Object original = Resources.Load(path, typeof(GameObject)) as GameObject;
		GameObject gameObject = Instantiate(original) as GameObject;
		Camera camera = FindMyCamera(gameObject.layer);
		if (camera != null)
		{
			Vector3 position = camera.ScreenToWorldPoint(SavePopupPosition);
			position.z = camera.transform.position.z + camera.nearClipPlane + 0.5f;
			gameObject.transform.position = position;
		}
	}
}
