using UnityEngine;

internal class TextScrollerUI : MonoBehaviour
{
	public UILabel templateLabel;
	public UIDraggablePanel scrollerPanel;
	public float scrollSpeed;
	public string LocStringOrFilename;
	public bool isFilename;
	public bool allowWrap;
	public bool mustScrollToEnd;
	public GameObject acceptButton;
}
