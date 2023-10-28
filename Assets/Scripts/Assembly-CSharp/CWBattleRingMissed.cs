using System.Collections;
using UnityEngine;

public class CWBattleRingMissed : MonoBehaviour
{
	public bool timeOutFlag;

	public UILabel undefendedLabel;

	private void OnClick()
	{
		CWBattleSequenceController instance = CWBattleSequenceController.GetInstance();
		instance.critAreaSprite.color = instance.critAreaSprite.color * 0.3f;
		instance.hitAreaSprite.color = instance.hitAreaSprite.color * 0.3f;
		instance.baseSprite.color = Color.black * 0.5f;
		if (undefendedLabel != null)
		{
			undefendedLabel.gameObject.SetActive(false);
			undefendedLabel.enabled = false;
		}
		CWBattleRingSetLabel component = GetComponent<CWBattleRingSetLabel>();
		if (instance.undefendedFlag)
		{
			instance.undefendedSprite.enabled = true;
			StartCoroutine(DelayDisableSprite(instance.undefendedSprite));
			component.labelString = string.Empty;
			if (undefendedLabel != null)
			{
				undefendedLabel.gameObject.SetActive(true);
				undefendedLabel.enabled = true;
			}
		}
		else if (instance.noAttackFlag)
		{
			component.labelString = KFFLocalization.Get("!!Q_5_NOATTACK");
		}
		else
		{
			component.labelString = KFFLocalization.Get("!!Q_5_MISSED");
		}
	}

	private IEnumerator DelayDisableSprite(UISprite sp)
	{
		yield return new WaitForSeconds(1f);
		sp.enabled = false;
	}

	private void Update()
	{
		CWBattleSequenceController instance = CWBattleSequenceController.GetInstance();
		if (undefendedLabel != null)
		{
			undefendedLabel.enabled = instance.undefendedFlag;
		}
	}
}
