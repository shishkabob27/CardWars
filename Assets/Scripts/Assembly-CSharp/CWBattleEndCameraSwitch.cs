using UnityEngine;

public class CWBattleEndCameraSwitch : MonoBehaviour
{
	public Camera[] disableCameras;

	public Camera expandCamera;

	public Camera shrinkCamera;

	public bool shrinkP2;

	private bool cameraExpand;

	private Rect expdOriginalRect;

	private Rect shrkOriginalRect;

	private float expdPrevX;

	private float expdNewX;

	private float expdPrevW;

	private float expdNewW = 1f;

	private float shrkPrevX;

	private float shrkNewX;

	private float shrkPrevW;

	private float shrkNewW;

	private float timer;

	private void OnClick()
	{
		if (disableCameras != null)
		{
			Camera[] array = disableCameras;
			foreach (Camera camera in array)
			{
				if (camera != null)
				{
					camera.enabled = false;
				}
			}
		}
		if (expandCamera != null)
		{
			expdOriginalRect = expandCamera.rect;
			expdPrevX = expdOriginalRect.x;
			expdPrevW = expdOriginalRect.width;
			expandCamera.enabled = true;
		}
		if (shrinkCamera != null)
		{
			shrkOriginalRect = shrinkCamera.rect;
			shrkPrevX = shrkOriginalRect.x;
			shrkPrevW = shrkOriginalRect.width;
			if (shrinkP2)
			{
				shrkNewX = 1f;
			}
			shrinkCamera.enabled = true;
		}
		cameraExpand = true;
	}

	private void Update()
	{
		if (!cameraExpand)
		{
			return;
		}
		timer += Time.deltaTime;
		if ((double)Mathf.Abs(expdNewW - expdPrevW) < 0.001)
		{
			expdPrevX = expdNewX;
			expdPrevW = expdNewW;
			cameraExpand = false;
			if (shrinkCamera != null)
			{
				shrinkCamera.enabled = false;
			}
		}
		if (expandCamera != null)
		{
			expdPrevX = GetNewRectValue(expdPrevX, expdNewX);
			expdPrevW = GetNewRectValue(expdPrevW, expdNewW);
			expandCamera.rect = new Rect(expdPrevX, 0f, expdPrevW, 1f);
		}
		if (shrinkCamera != null)
		{
			shrkPrevX = GetNewRectValue(shrkPrevX, shrkNewX);
			shrkPrevW = GetNewRectValue(shrkPrevW, shrkNewW);
			shrinkCamera.rect = new Rect(shrkPrevX, 0f, shrkPrevW, 1f);
		}
	}

	private float GetNewRectValue(float before, float after)
	{
		float num = 0f;
		return Mathf.Lerp(before, after, timer * 0.1f);
	}
}
