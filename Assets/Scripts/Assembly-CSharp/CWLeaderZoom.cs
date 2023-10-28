using UnityEngine;

public class CWLeaderZoom : MonoBehaviour
{
	public LeaderItem leader;

	public string leaderName;

	private UITexture texture;

	private void OnEnable()
	{
		leaderName = leader.Form.Name;
		PanelManagerDeck instance = PanelManagerDeck.GetInstance();
		CWDeckHeroSelection.FillCardInfo(base.gameObject, leader);
		TweenTransform component = GetComponent<TweenTransform>();
		component.from = instance.activeCard.transform;
		texture = base.gameObject.GetComponentInChildren<UITexture>();
	}

	private void OnClick()
	{
		PanelManagerDeck instance = PanelManagerDeck.GetInstance();
		TweenAlpha component = instance.blackPanel.GetComponent<TweenAlpha>();
		component.Play(false);
		component.Reset();
		Invoke("DeactivateBlack", component.duration);
	}

	private void DeactivateBlack()
	{
		PanelManagerDeck instance = PanelManagerDeck.GetInstance();
		if (NGUITools.GetActive(instance.blackPanel))
		{
			NGUITools.SetActive(instance.blackPanel, false);
		}
	}

	private void OnDisable()
	{
		if (texture != null)
		{
			Texture mainTexture = texture.mainTexture;
			texture.mainTexture = null;
			Resources.UnloadAsset(mainTexture);
		}
	}
}
