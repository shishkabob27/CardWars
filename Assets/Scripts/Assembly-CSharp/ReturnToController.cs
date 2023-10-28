using UnityEngine;

public class ReturnToController : MonoBehaviour
{
	public UIButtonTween ReturnToShowTween;

	public void Start()
	{
	}

	public void OnClick()
	{
		if (null != ReturnToShowTween)
		{
			ReturnToShowTween.Play(true);
		}
	}
}
