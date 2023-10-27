using UnityEngine;

public class UITextList : MonoBehaviour
{
	public enum Style
	{
		Text = 0,
		Chat = 1,
	}

	public Style style;
	public UILabel textLabel;
	public float maxWidth;
	public float maxHeight;
	public int maxEntries;
	public bool supportScrollWheel;
}
