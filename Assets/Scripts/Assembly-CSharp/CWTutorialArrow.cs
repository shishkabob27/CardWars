using UnityEngine;

public class CWTutorialArrow : MonoBehaviour
{
	private GameObject target;

	private bool scaleTarget;

	private TweenScale tweenScale;

	public bool UpdatePosition { get; set; }

	public Transform SpriteGroup { get; set; }

	public string Corner { get; set; }

	public float OffsetX { get; set; }

	public float OffsetY { get; set; }

	public float OffsetZ { get; set; }

	public int Layer { get; set; }

	public GameObject Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
			UpdateTweenScale();
		}
	}

	public bool ScaleTarget
	{
		get
		{
			return scaleTarget;
		}
		set
		{
			scaleTarget = value;
			UpdateTweenScale();
		}
	}

	private void OnDisable()
	{
		RemoveTweenScale();
	}

	private void OnDestroy()
	{
		RemoveTweenScale();
	}

	private void UpdateTweenScale()
	{
		if (ScaleTarget && Target != null)
		{
			AddTweenScale(Target);
		}
		else
		{
			RemoveTweenScale();
		}
	}

	private void Update()
	{
		if (UpdatePosition)
		{
			UpdateArrowPosition();
		}
	}

	public void UpdateArrowPosition()
	{
		if (SpriteGroup != null)
		{
			SpriteGroup.position = getPos(Target, Corner, OffsetX, OffsetY, OffsetZ, Layer);
		}
	}

	public static Vector3 getPos(GameObject target, string position, float offsetX, float offsetY, float offsetZ, int layer)
	{
		float num = 0f;
		float num2 = 0f;
		float z = 0f;
		Collider component = target.GetComponent<Collider>();
		Bounds bounds;
		if (component != null)
		{
			bounds = component.bounds;
		}
		else
		{
			bounds = SLOTGame.GetRendererBoundsRecursive(target.transform);
			z = bounds.min.z;
		}
		switch (position)
		{
		case "RightTop":
			num = bounds.max.x;
			num2 = bounds.max.y;
			break;
		case "LeftTop":
			num = bounds.min.x;
			num2 = bounds.max.y;
			break;
		case "RightBottom":
			num = bounds.max.x;
			num2 = bounds.min.y;
			break;
		case "LeftBottom":
			num = bounds.min.x;
			num2 = bounds.min.y;
			break;
		case "Top":
			num = bounds.min.x + (bounds.max.x - bounds.min.x) * 0.5f;
			num2 = bounds.max.y;
			break;
		case "Bottom":
			num = bounds.min.x + (bounds.max.x - bounds.min.x) * 0.5f;
			num2 = bounds.min.y;
			break;
		case "Left":
			num = bounds.min.x;
			num2 = bounds.min.y + (bounds.max.y - bounds.min.y) * 0.5f;
			break;
		case "Right":
			num = bounds.max.x;
			num2 = bounds.min.y + (bounds.max.y - bounds.min.y) * 0.5f;
			break;
		default:
			num = bounds.min.x + (bounds.max.x - bounds.min.x) * 0.5f;
			num2 = bounds.min.y + (bounds.max.y - bounds.min.y) * 0.5f;
			z = bounds.min.z + (bounds.max.z - bounds.min.z) * 0.5f;
			break;
		}
		Vector3 vector = new Vector3(num, num2, z);
		if (layer >= 0 && target.layer != layer)
		{
			Camera camera = NGUITools.FindCameraForLayer(target.layer);
			Camera camera2 = NGUITools.FindCameraForLayer(layer);
			if (camera != camera2)
			{
				vector = camera.WorldToScreenPoint(vector);
				vector.z = camera2.nearClipPlane;
				vector = camera2.ScreenToWorldPoint(vector);
			}
		}
		vector.x += offsetX;
		vector.y += offsetY;
		vector.z += offsetZ;
		return vector;
	}

	private void AddTweenScale(GameObject obj)
	{
		RemoveTweenScale();
		if (obj != null)
		{
			tweenScale = obj.AddComponent(typeof(TweenScale)) as TweenScale;
			if (tweenScale != null)
			{
				tweenScale.style = UITweener.Style.PingPong;
				tweenScale.duration = 0.25f;
				tweenScale.tweenGroup = 123;
				Vector3 localScale = obj.transform.localScale;
				tweenScale.from = localScale;
				tweenScale.to = new Vector3(localScale.x * 1.1f, localScale.y * 1.1f, localScale.z * 1.1f);
			}
		}
	}

	private void RemoveTweenScale()
	{
		if (tweenScale != null)
		{
			tweenScale.style = UITweener.Style.Once;
			tweenScale.Play(true);
			tweenScale.Reset();
			Object.Destroy(tweenScale);
			tweenScale = null;
		}
	}
}
