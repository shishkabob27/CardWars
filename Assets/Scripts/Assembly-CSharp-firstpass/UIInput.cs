using UnityEngine;

public class UIInput : MonoBehaviour
{
	public enum KeyboardType
	{
		Default = 0,
		ASCIICapable = 1,
		NumbersAndPunctuation = 2,
		URL = 3,
		NumberPad = 4,
		PhonePad = 5,
		NamePhonePad = 6,
		EmailAddress = 7,
	}

	public UILabel label;
	public int maxChars;
	public string caratChar;
	public KeyboardType type;
	public bool isPassword;
	public bool autoCorrect;
	public bool useLabelTextAtStart;
	public Color activeColor;
	public GameObject selectOnTab;
	public GameObject eventReceiver;
	public string functionName;
}
