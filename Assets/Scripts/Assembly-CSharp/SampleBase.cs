using UnityEngine;

[RequireComponent(typeof(SampleUI))]
public class SampleBase : MonoBehaviour
{
	private SampleUI ui;

	public SampleUI UI
	{
		get
		{
			return ui;
		}
	}

	protected virtual string GetHelpText()
	{
		return string.Empty;
	}

	protected virtual void Awake()
	{
		ui = GetComponent<SampleUI>();
	}

	protected virtual void Start()
	{
		ui.helpText = GetHelpText();
	}

	public static Vector3 GetWorldPos(Vector2 screenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenPos);
		float distance = (0f - ray.origin.z) / ray.direction.z;
		return ray.GetPoint(distance);
	}
}
