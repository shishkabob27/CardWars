using UnityEngine;

public class VOButtonTrigger : MonoBehaviour
{
	public VOEvent VOE;

	public int player;

	public void OnClick()
	{
		if (base.enabled)
		{
			VOManager.Instance.PlayEvent(player, VOE);
		}
	}
}
