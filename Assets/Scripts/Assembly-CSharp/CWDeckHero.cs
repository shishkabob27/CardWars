using UnityEngine;

public class CWDeckHero : MonoBehaviour
{
	private bool selected;

	private bool grayed;

	private bool responsive = true;

	public GameObject RadioParentObject;

	public LeaderItem leader;

	public bool NonSelectable;

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
			if (selected)
			{
				UnsetAllSiblings();
			}
			SQUtils.SetActive(base.gameObject, "Panel/Panel/InUse", selected);
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
		if (!Grayed && Responsive && !NonSelectable)
		{
			Selected = !Selected;
		}
	}

	private void SetGray(bool isGray)
	{
		Color color = new Color(0.2f, 0.2f, 0.2f, 1f);
		Color color2 = ((!isGray) ? Color.white : color);
		UISprite[] componentsInChildren = base.gameObject.GetComponentsInChildren<UISprite>(true);
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			uISprite.color = color2;
		}
		UILabel[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<UILabel>(true);
		UILabel[] array2 = componentsInChildren2;
		foreach (UILabel uILabel in array2)
		{
			switch (uILabel.name)
			{
			case "HP_Label":
				uILabel.color = color2;
				break;
			}
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
		CWDeckHero[] componentsInChildren = RadioParentObject.GetComponentsInChildren<CWDeckHero>(true);
		CWDeckHero[] array = componentsInChildren;
		foreach (CWDeckHero cWDeckHero in array)
		{
			if (!(cWDeckHero == this))
			{
				cWDeckHero.Selected = false;
			}
		}
	}
}
