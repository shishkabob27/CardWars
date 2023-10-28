using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class KeyItemController : MonoBehaviour
{
	public UILabel Label;

	public UISprite Icon;

	public GameObject Highlight;

	[HideInInspector]
	public KeyRingItem keyRingItem;

	[method: MethodImpl(32)]
	public event Action<KeyItemController> OnSelectEvent;

	public void OnClick()
	{
		if (this.OnSelectEvent != null)
		{
			this.OnSelectEvent(this);
		}
	}

	public void EnableHighlight(bool b)
	{
		Highlight.SetActive(b);
	}

	public void SetData(KeyRingItem ringItem)
	{
		keyRingItem = ringItem;
		Label.text = KFFLocalization.Get(ringItem.Name);
		Icon.spriteName = ringItem.Icon;
	}
}
