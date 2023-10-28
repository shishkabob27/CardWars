using UnityEngine;

public class TipPanelScript : MonoBehaviour
{
	public UILabel HeaderLabel;

	public UILabel MessageLabel;

	public TipContext Context;

	private TipManager tipMgr;

	private Tip tip;

	private void OnEnable()
	{
		tipMgr = TipManager.Instance;
		tip = tipMgr.GetRandomTipWithContext(Context);
		if (tip != null)
		{
			if (HeaderLabel != null)
			{
				HeaderLabel.text = KFFLocalization.Get(tip.Header);
			}
			if (MessageLabel != null)
			{
				MessageLabel.text = KFFLocalization.Get(tip.Message);
			}
		}
	}
}
