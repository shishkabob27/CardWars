using UnityEngine;

public class CWBattleDamageFXLabel : MonoBehaviour
{
	public UILabel CritDamageLabel;

	public UILabel CritDamageLabelScale;

	private CWBattleSequenceController battleSqCtrlr;

	private void Start()
	{
		battleSqCtrlr = CWBattleSequenceController.GetInstance();
		if (CritDamageLabel != null)
		{
			CritDamageLabel.text += battleSqCtrlr.damageModifierCrit;
		}
		if (CritDamageLabelScale != null)
		{
			CritDamageLabelScale.text = CritDamageLabel.text + battleSqCtrlr.damageModifierCrit;
		}
	}

	private void Update()
	{
	}
}
