using UnityEngine;

public class UICamera : MonoBehaviour
{
	public bool debug;
	public bool useMouse;
	public bool useTouch;
	public bool allowMultiTouch;
	public bool useKeyboard;
	public bool useController;
	public bool stickyPress;
	public LayerMask eventReceiverMask;
	public bool clipRaycasts;
	public float tooltipDelay;
	public bool stickyTooltip;
	public float mouseDragThreshold;
	public float mouseClickThreshold;
	public float touchDragThreshold;
	public float touchClickThreshold;
	public float rangeDistance;
	public string scrollAxisName;
	public string verticalAxisName;
	public string horizontalAxisName;
	public KeyCode submitKey0;
	public KeyCode submitKey1;
	public KeyCode cancelKey0;
	public KeyCode cancelKey1;
}
