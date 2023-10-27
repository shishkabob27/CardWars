using UnityEngine;

public class GestureRecognizer : MonoBehaviour
{
	public enum SelectionType
	{
		Default = 0,
		StartSelection = 1,
		CurrentSelection = 2,
		None = 3,
	}

	[SerializeField]
	private int requiredFingerCount;
	public int MaxSimultaneousGestures;
	public GestureResetMode ResetMode;
	public ScreenRaycaster Raycaster;
	public FingerClusterManager ClusterManager;
	public GestureRecognizerDelegate Delegate;
	public bool UseSendMessage;
	public string EventMessageName;
	public GameObject EventMessageTarget;
	public SelectionType SendMessageToSelection;
	public bool IsExclusive;
}
