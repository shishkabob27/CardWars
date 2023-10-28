using System;
using System.Collections;
using UnityEngine;

public class GachaMenu : MonoBehaviour
{
	public UITweener partyBar;

	public UILabel partyLabel;

	public UILabel descriptionLabel;

	public UILabel expiration;

	private PartyInfo current;

	private DateTime? end;

	private bool clear;

	public IEnumerator SetupForParty(PartyInfo info)
	{
		if (info != null && current != info)
		{
			if (current != null)
			{
				clear = true;
				partyBar.Play(false);
				yield return new WaitForSeconds(partyBar.duration);
			}
			CWGachaController.GetInstance().carnivalString = info.title;
			descriptionLabel.text = KFFLocalization.Get(info.description);
			partyLabel.text = KFFLocalization.Get(info.title);
			expiration.text = string.Format(KFFLocalization.Get("!!PARTY_EXPIRATION_DATE"), info.end.ToLocalTime().ToString("MMM d h:mm tt"));
			clear = false;
			partyBar.gameObject.SetActive(true);
			partyBar.Play(true);
			current = info;
			end = info.end;
		}
	}

	public void Update()
	{
		if (end.HasValue && TFUtils.ServerTime.CompareTo(end.Value) > 0)
		{
			CWGachaController.GetInstance().carnivalString = null;
			current = null;
			end = null;
			clear = true;
			partyBar.Play(false);
			descriptionLabel.text = KFFLocalization.Get("!!E_0_INFOBAR");
		}
	}

	public void OnEnable()
	{
		GachaManager instance = GachaManager.Instance;
		PartyInfo currentPartyInfo = instance.GetCurrentPartyInfo();
		if (currentPartyInfo != null)
		{
			StartCoroutine(SetupForParty(currentPartyInfo));
		}
		else
		{
			descriptionLabel.text = KFFLocalization.Get("!!E_0_INFOBAR");
		}
	}

	public void ToggleTweenCompleted()
	{
		partyBar.gameObject.SetActive(!clear);
	}

	public void CheckPartyStatus()
	{
		if (current != null)
		{
			partyBar.gameObject.SetActive(true);
			partyBar.Play(true);
		}
	}
}
