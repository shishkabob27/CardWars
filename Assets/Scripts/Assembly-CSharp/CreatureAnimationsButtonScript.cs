using UnityEngine;

public class CreatureAnimationsButtonScript : MonoBehaviour
{
	public bool OnButton;

	private void OnClick()
	{
		GlobalFlags.Instance.SkipAnims = !OnButton;
	}
}
