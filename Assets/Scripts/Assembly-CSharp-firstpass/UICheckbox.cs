using UnityEngine;

public class UICheckbox : MonoBehaviour
{
	public UISprite checkSprite;
	public Animation checkAnimation;
	public bool instantTween;
	public bool startsChecked;
	public Transform radioButtonRoot;
	public bool optionCanBeNone;
	public GameObject eventReceiver;
	public string functionName;
	[SerializeField]
	private bool option;
}
