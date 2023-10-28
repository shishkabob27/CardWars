using UnityEngine;

public class CWFuseCard : MonoBehaviour
{
	public static CardItem chosenCard;

	private bool grayed;

	public CardItem card;

	public string cardName;

	public bool Grayed
	{
		get
		{
			return grayed;
		}
		set
		{
			grayed = value;
			SetGray(grayed);
		}
	}

	public void OnClick()
	{
		if (card.IsNew)
		{
			card.IsNew = false;
			CWDeckCard.ShowNewFlag(base.gameObject, false);
		}
		if (!Grayed)
		{
			chosenCard = card;
		}
	}

	private void SetGray(bool isGray)
	{
		Color color = new Color(0.2f, 0.2f, 0.2f, 1f);
		UISprite[] componentsInChildren = base.gameObject.GetComponentsInChildren<UISprite>(true);
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			uISprite.color = ((!isGray) ? Color.white : color);
		}
	}

	private void SetEnabled(string name, bool isEnabled)
	{
		Transform transform = base.transform.Find(name);
		if (!(transform == null))
		{
			UISprite component = transform.gameObject.GetComponent<UISprite>();
			if (!(component == null))
			{
				component.enabled = isEnabled;
			}
		}
	}
}
