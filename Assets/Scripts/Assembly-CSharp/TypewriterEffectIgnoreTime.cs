using UnityEngine;

public class TypewriterEffectIgnoreTime : MonoBehaviour
{
	public string CWtutorialText;
	public int CWtextLength;
	public CWTutorialTapDelegate tapDelegateScript;
	public int charsPerSecond;
	public bool ignoreTimeScale;
	public float duration;
}
