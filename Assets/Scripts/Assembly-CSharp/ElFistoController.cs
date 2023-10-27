using UnityEngine;

public class ElFistoController : MonoBehaviour
{
	public enum ElFistoModes
	{
		Invalid = -1,
		Off = 0,
		On = 1,
		Always = 2,
		Count = 3,
	}

	public ElFistoModes ElFistoMode;
	public AudioClip IntroSound;
	public UIButtonTween ShowElFistoIntroTween;
	public UIButtonTween HideElFistoIntroTween;
	public float ElFistoShowIntroDelay;
	public string ElFistoNIS;
	public string ElFistoCardNIS;
	public string RoundTextKey;
}
