using UnityEngine;

public class CWDiscardCard : MonoBehaviour
{
	private bool selected;

	private bool xed;

	private bool grayed;

	private bool responsive = true;

	public bool SelectShowsX;

	public GameObject RadioParentObject;

	public CardItem card;

	public CardScript filterScript;

	public bool Responsive
	{
		get
		{
			return responsive;
		}
		set
		{
			responsive = value;
		}
	}

	public bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
			xed = false;
			if (selected)
			{
				UnsetAllSiblings();
			}
			SetEnabled("Panel/Panel/Selected", selected);
			SetEnabled("Panel/Panel/Xed", xed);
		}
	}

	public bool Xed
	{
		get
		{
			return xed;
		}
		set
		{
			selected = false;
			xed = value;
			if (xed)
			{
				UnsetAllSiblings();
			}
			SetEnabled("Panel/Panel/Selected", selected);
			SetEnabled("Panel/Panel/Xed", xed);
		}
	}

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
		if (filterScript != null && !filterScript.CardSelected)
		{
			filterScript.CardSelection(card);
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

	private void UnsetAllSiblings()
	{
		if (RadioParentObject == null)
		{
			return;
		}
		CWDeckCard[] componentsInChildren = RadioParentObject.GetComponentsInChildren<CWDeckCard>(true);
		CWDeckCard[] array = componentsInChildren;
		foreach (CWDeckCard cWDeckCard in array)
		{
			if (!(cWDeckCard == this))
			{
				cWDeckCard.Selected = false;
				cWDeckCard.Xed = false;
			}
		}
	}
}
