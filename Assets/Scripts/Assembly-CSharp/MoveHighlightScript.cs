using UnityEngine;

public class MoveHighlightScript : MonoBehaviour
{
	public GameObject Highlight;

	private void OnClick()
	{
		Highlight.transform.position = base.transform.position;
	}
}
