using System.Collections;
using UnityEngine;

public class CWiTweenLandscapeTrigger : MonoBehaviour
{
	public GameObject[] tweenTargets;

	public string tweenName;

	public string tweenName2;

	public float delay;

	public GameObject readyButton;

	private LandscapeManagerScript landscapeMagr;

	private GameState GameInstance;

	private void Start()
	{
		landscapeMagr = LandscapeManagerScript.GetInstance();
		GameInstance = GameState.Instance;
	}

	private void OnClick()
	{
		StartCoroutine(PlayTween(delay));
	}

	private IEnumerator PlayTween(float waitTime)
	{
		for (int i = 0; i < tweenTargets.Length; i++)
		{
			GameObject tw = tweenTargets[i];
			if (tw != null && tweenName != string.Empty)
			{
				iTweenEvent tweenEvent2 = iTweenEvent.GetEvent(tw, tweenName);
				if (tweenEvent2 != null)
				{
					tweenEvent2.Play();
				}
			}
			if (tw != null && tweenName2 != string.Empty)
			{
				iTweenEvent tweenEvent = iTweenEvent.GetEvent(tw, tweenName2);
				if (tweenEvent != null)
				{
					tweenEvent.Play();
				}
			}
			TweenScale tweenScale = tw.GetComponent<TweenScale>();
			if (tweenScale != null)
			{
				tweenScale.Play(true);
			}
			AudioSource audio = tw.GetComponent<AudioSource>();
			if (audio != null)
			{
				SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(audio);
			}
			yield return new WaitForSeconds(waitTime);
			TweenAlpha tweenAlpha = tw.GetComponent<TweenAlpha>();
			if (tweenAlpha != null)
			{
				tweenAlpha.Play(true);
			}
			CWLandscapeCardDragOld cardScript = tw.GetComponent<CWLandscapeCardDragOld>();
			for (int j = 0; j < 4; j++)
			{
				if (GameInstance.GetLandscapeType(PlayerType.User, i) == LandscapeType.None)
				{
					GameInstance.SetLandscape(PlayerType.User, i, cardScript.CurrentType);
					landscapeMagr.UpdateLandscapes();
					break;
				}
			}
		}
		readyButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
	}
}
