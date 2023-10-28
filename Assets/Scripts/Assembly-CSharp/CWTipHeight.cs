using UnityEngine;

public class CWTipHeight : MonoBehaviour
{
	public float tweenHeight;

	public TweenPosition panelTween;

	public TipContext panelContext;

	private void OnClick()
	{
		if (panelTween != null)
		{
			panelTween.to = new Vector3(panelTween.to.x, tweenHeight, panelTween.to.z);
			TipPanelScript component = panelTween.gameObject.GetComponent<TipPanelScript>();
			if (component != null)
			{
				component.Context = panelContext;
			}
		}
	}
}
