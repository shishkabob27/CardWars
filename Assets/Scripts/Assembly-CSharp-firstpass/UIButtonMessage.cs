using UnityEngine;

public class UIButtonMessage : MonoBehaviour
{
	public enum Trigger
	{
		OnClick = 0,
		OnMouseOver = 1,
		OnMouseOut = 2,
		OnPress = 3,
		OnRelease = 4,
		OnDoubleClick = 5,
	}

	public GameObject target;
	public string functionName;
	public Trigger trigger;
	public bool includeChildren;
}
