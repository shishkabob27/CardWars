using UnityEngine;
using System.Collections.Generic;

public class UIPopupList : MonoBehaviour
{
	public enum Position
	{
		Auto = 0,
		Above = 1,
		Below = 2,
	}

	public UIAtlas atlas;
	public UIFont font;
	public UILabel textLabel;
	public string backgroundSprite;
	public string highlightSprite;
	public Position position;
	public List<string> items;
	public Vector2 padding;
	public float textScale;
	public Color textColor;
	public Color backgroundColor;
	public Color highlightColor;
	public bool isAnimated;
	public bool isLocalized;
	public GameObject eventReceiver;
	public string functionName;
	[SerializeField]
	private string mSelectedItem;
}
