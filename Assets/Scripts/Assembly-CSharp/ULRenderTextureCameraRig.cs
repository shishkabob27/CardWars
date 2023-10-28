using UnityEngine;

public class ULRenderTextureCameraRig
{
	public delegate void RelativeCamDelegate(GameObject subject, Camera cam);

	private GameObject rig;

	private Camera camera;

	private int layer;

	public int Layer
	{
		get
		{
			return layer;
		}
		set
		{
			Camera.main.cullingMask |= 1 << layer;
			camera.cullingMask &= ~(1 << layer);
			layer = value;
			Camera.main.cullingMask &= ~(1 << layer);
			camera.cullingMask |= 1 << layer;
		}
	}

	public Camera RigCamera
	{
		get
		{
			return camera;
		}
	}

	public GameObject RigGameObject
	{
		get
		{
			return rig;
		}
	}

	public ULRenderTextureCameraRig()
	{
		rig = new GameObject("ULRenderTextureCameraRig");
		rig.AddComponent<Camera>();
		camera = rig.GetComponent<Camera>();
		camera.enabled = false;
		camera.targetTexture = null;
		camera.cullingMask = 0;
		camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		camera.clearFlags = CameraClearFlags.Color;
	}

	public ULRenderTextureCameraRig(int layer)
		: this()
	{
		Layer = layer;
	}

	public static void SetRenderLayer(GameObject gameObject, int layer)
	{
		gameObject.layer = layer;
		int childCount = gameObject.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			SetRenderLayer(gameObject.transform.GetChild(i).gameObject, layer);
		}
	}

	public void RenderSubjectToTexture(GameObject subject, ULRenderTexture renderTexture, RelativeCamDelegate camDelegate)
	{
		int num = subject.layer;
		SetRenderLayer(subject, layer);
		camera.targetTexture = renderTexture.RTexture;
		if (camDelegate != null)
		{
			camDelegate(subject, camera);
		}
		camera.Render();
		SetRenderLayer(subject, num);
	}
}
