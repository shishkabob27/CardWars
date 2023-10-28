using UnityEngine;

public class CWBattleRingSetLabel : MonoBehaviour
{
	public UILabel ringHitLabel;

	public string labelString;

	public Color color;

	public Color effectColor = Color.white;

	private void OnClick()
	{
		ringHitLabel.text = KFFLocalization.Get(labelString);
		ringHitLabel.color = color;
		ringHitLabel.effectColor = effectColor;
		ringHitLabel.gameObject.SetActive(true);
	}

	private void OnDisable()
	{
		ringHitLabel.text = string.Empty;
	}

	private void Update()
	{
	}
}
