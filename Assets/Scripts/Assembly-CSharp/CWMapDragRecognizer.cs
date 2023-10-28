using System.Collections.Generic;
using UnityEngine;

public class CWMapDragRecognizer : DragRecognizer
{
	public LayerMask layers;

	private CWScreenRaycaster rayCaster;

	private void CreateRaycaster()
	{
		if (rayCaster == null)
		{
			rayCaster = base.gameObject.GetComponent(typeof(CWScreenRaycaster)) as CWScreenRaycaster;
			if (rayCaster == null)
			{
				rayCaster = base.gameObject.AddComponent(typeof(CWScreenRaycaster)) as CWScreenRaycaster;
			}
		}
		if (!(rayCaster != null))
		{
			return;
		}
		Object[] array = Object.FindObjectsOfType(typeof(Camera));
		List<Camera> list = new List<Camera>();
		Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Camera camera = (Camera)array2[i];
			if (camera != null && (layers.value & (1 << camera.gameObject.layer)) != 0)
			{
				list.Add(camera);
			}
		}
		rayCaster.Cameras = list.ToArray();
	}

	protected override bool CanBegin(DragGesture gesture, FingerGestures.IFingerList touches)
	{
		if (!base.CanBegin(gesture, touches))
		{
			return false;
		}
		if (rayCaster == null)
		{
			CreateRaycaster();
		}
		if (rayCaster != null)
		{
			int count = touches.Count;
			for (int i = 0; i < count; i++)
			{
				FingerGestures.Finger finger = touches[i];
				if (finger == null)
				{
					continue;
				}
				GameObject gameObject = gesture.PickObject(rayCaster, finger.StartPosition);
				if (gameObject != null)
				{
					CWCameraCollider cWCameraCollider = gameObject.GetComponent(typeof(CWCameraCollider)) as CWCameraCollider;
					if (cWCameraCollider == null)
					{
						return false;
					}
				}
			}
		}
		return true;
	}
}
