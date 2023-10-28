using UnityEngine;

public class Menu3Dto2DScript : MonoBehaviour
{
	public GameObject Object3D;

	public Camera GameCamera;

	public bool useCenter;

	private TweenPosition posTween;

	private void Awake()
	{
		if (!base.enabled)
		{
			return;
		}
		posTween = base.gameObject.GetComponent<TweenPosition>();
		if (posTween != null && Object3D != null && GameCamera != null)
		{
			BoxCollider component = Object3D.GetComponent<BoxCollider>();
			if (component != null)
			{
				Vector3 from = ((!useCenter) ? GameCamera.WorldToScreenPoint(Object3D.transform.position) : component.center);
				posTween.from = from;
			}
		}
	}

	private void Update()
	{
	}
}
