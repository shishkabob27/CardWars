using UnityEngine;

public class TriggerVFX : MonoBehaviour
{
	public GameObject VFX;

	public float Scale = 1f;

	public Transform Anchor;

	public bool useCustomCoords;

	public Vector3 CustomCoords;

	public bool ChildOfAnchor;

	public void SpawnVFX()
	{
		GameObject gameObject = (GameObject)SLOTGame.InstantiateFX(VFX);
		if (!(gameObject != null))
		{
			return;
		}
		if (useCustomCoords)
		{
			gameObject.transform.position = CustomCoords;
		}
		else if (Anchor != null)
		{
			gameObject.transform.position = Anchor.position;
			if (ChildOfAnchor)
			{
				gameObject.transform.parent = Anchor.transform;
			}
		}
		else
		{
			gameObject.transform.position = base.gameObject.transform.position;
		}
		gameObject.transform.localScale = new Vector3(Scale, Scale, Scale);
	}

	private void OnClick()
	{
		SpawnVFX();
	}
}
