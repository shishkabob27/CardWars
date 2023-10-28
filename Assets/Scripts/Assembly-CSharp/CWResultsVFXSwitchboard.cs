using UnityEngine;

public class CWResultsVFXSwitchboard : MonoBehaviour
{
	public GameObject RarityVFX_Low;

	public GameObject RarityVFX_Med;

	public GameObject RarityVFX_High;

	public TriggerVFX TriggerScript;

	public TweenPosition ParentTween;

	private static CWResultsVFXSwitchboard vfx_switchboard;

	public static CWResultsVFXSwitchboard GetInstance()
	{
		return vfx_switchboard;
	}

	private void Awake()
	{
		vfx_switchboard = this;
	}

	public void SetRarityLow()
	{
		if (TriggerScript != null && RarityVFX_Low != null)
		{
			TriggerScript.VFX = RarityVFX_Low;
		}
	}

	public void SetRarityMed()
	{
		if (TriggerScript != null && RarityVFX_Med != null)
		{
			TriggerScript.VFX = RarityVFX_Med;
		}
	}

	public void SetRarityHigh()
	{
		if (TriggerScript != null && RarityVFX_High != null)
		{
			TriggerScript.VFX = RarityVFX_High;
		}
	}

	public void ResetParent()
	{
		if (ParentTween != null)
		{
			ParentTween.eventReceiver = null;
			ParentTween.callWhenFinished = string.Empty;
		}
	}
}
