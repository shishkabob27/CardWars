using UnityEngine;

[RequireComponent(typeof(TapRecognizer))]
[AddComponentMenu("FingerGestures/Toolbox/Camera/Look At Tap")]
public class TBLookAtTap : MonoBehaviour
{
	private TBDragView dragView;

	private void Awake()
	{
		dragView = GetComponent<TBDragView>();
	}

	private void Start()
	{
		if (!GetComponent<TapRecognizer>())
		{
			base.enabled = false;
		}
	}

	private void OnTap(TapGesture gesture)
	{
		Ray ray = Camera.main.ScreenPointToRay(gesture.Position);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo))
		{
			if ((bool)dragView)
			{
				dragView.LookAt(hitInfo.point);
			}
			else
			{
				base.transform.LookAt(hitInfo.point);
			}
		}
	}
}
