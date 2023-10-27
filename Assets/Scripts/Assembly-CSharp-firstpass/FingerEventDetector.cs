using UnityEngine;

public class FingerEventDetector : MonoBehaviour
{
	public int FingerIndexFilter;
	public ScreenRaycaster Raycaster;
	public bool UseSendMessage;
	public bool SendMessageToSelection;
	public GameObject MessageTarget;
}
