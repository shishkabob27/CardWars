using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Orthographic Camera")]
public class UIOrthoCamera : MonoBehaviour
{
	private Camera mCam;

	private Transform mTrans;

	private void Start()
	{
		mCam = GetComponent<Camera>();
		mTrans = base.transform;
		mCam.orthographic = true;
	}

	private void Update()
	{
		float num = mCam.rect.yMin * (float)Screen.height;
		float num2 = mCam.rect.yMax * (float)Screen.height;
		float num3 = (num2 - num) * 0.5f * mTrans.lossyScale.y;
		if (!Mathf.Approximately(mCam.orthographicSize, num3))
		{
			mCam.orthographicSize = num3;
		}
	}
}
