using System.Collections;
using UnityEngine;

public class CWQuestMapFlyGem : MonoBehaviour
{
	public enum EarningType
	{
		GEM,
		HEART
	}

	public GameObject flyingObj;

	public Transform parentTr;

	public Transform start;

	public Transform dest;

	public float time;

	public Vector3 heartDestinationSize = new Vector3(0.1f, 0.1f, 0.1f);

	public Vector3 gemDestinationSize = new Vector3(0.1f, 0.1f, 0.1f);

	public AudioClip gemEarnedSound;

	private CWUpdatePlayerStats playerStats;

	public EarningType earningType;

	private bool _keyPressed;

	private void Start()
	{
	}

	private void OnEnable()
	{
		GetWaitTimeFromPlayTween(0);
		StartCoroutine(WaitThenFlyGem());
		playerStats = CWUpdatePlayerStats.GetInstance();
	}

	public virtual void SetResumeFlag(bool resume)
	{
		MapControllerBase.GetInstance().resumeFlag = resume;
	}

	public virtual void ComputeStartAndDest()
	{
		if (playerStats != null)
		{
			Transform transform = ((earningType != EarningType.HEART) ? playerStats.gemSprite.gameObject.transform : playerStats.heartSprite.gameObject.transform);
			Vector3 position = PanelManager.GetInstance().uiCamera.WorldToScreenPoint(transform.position);
			Vector3 position2 = MapControllerBase.GetInstance().uiCameraMap.ScreenToWorldPoint(position);
			dest.transform.position = position2;
		}
	}

	private IEnumerator WaitThenFlyGem()
	{
		yield return new WaitForSeconds(1f);
		yield return StartCoroutine(WaitForKeyPress());
		ComputeStartAndDest();
		GameObject spawnObj = GetSpawnObj(start);
		iTween.MoveTo(spawnObj, iTween.Hash("position", dest, "time", time));
		Vector3 sc = ((earningType != 0) ? heartDestinationSize : gemDestinationSize);
		iTween.ScaleTo(spawnObj, iTween.Hash("scale", sc, "time", time));
		yield return new WaitForSeconds(2f);
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(gemEarnedSound);
		UILabel earningTarget = null;
		if (playerStats != null)
		{
			earningTarget = ((earningType != 0) ? playerStats.staminaMaxLabel : playerStats.gemLabel);
			int currentCount = int.Parse(earningTarget.text);
			currentCount++;
			earningTarget.text = currentCount.ToString();
			if (earningType == EarningType.HEART)
			{
				UILabel staminaLabel = playerStats.staminaLabel;
				int staminCountPrev = int.Parse(staminaLabel.text);
				staminaLabel.text = Mathf.Max(staminCountPrev, currentCount).ToString();
			}
		}
		yield return new WaitForSeconds(1f);
		float waitTime = GetWaitTimeFromPlayTween(1);
		yield return new WaitForSeconds(waitTime);
		SetResumeFlag(true);
		base.gameObject.SetActive(false);
	}

	protected virtual GameObject GetSpawnObj(Transform start)
	{
		GameObject gameObject = null;
		if (parentTr == null)
		{
			parentTr = base.transform;
		}
		if (flyingObj != null)
		{
			gameObject = SLOTGame.InstantiateFX(flyingObj, start.position, start.rotation) as GameObject;
			gameObject.transform.parent = parentTr;
		}
		return gameObject;
	}

	private IEnumerator WaitForKeyPress()
	{
		while (!_keyPressed)
		{
			if (Input.GetMouseButtonDown(0))
			{
				yield return null;
				break;
			}
			yield return 0;
		}
	}

	private float GetWaitTimeFromPlayTween(int tweenGroup)
	{
		float result = 0f;
		TweenPosition[] components = GetComponents<TweenPosition>();
		TweenPosition[] array = components;
		foreach (TweenPosition tweenPosition in array)
		{
			if (tweenPosition.tweenGroup == tweenGroup)
			{
				tweenPosition.Reset();
				tweenPosition.Play(true);
				result = tweenPosition.duration;
			}
		}
		return result;
	}
}
