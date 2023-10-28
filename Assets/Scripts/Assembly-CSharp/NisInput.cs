using System;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NisInput : Selectable
{
	[method: MethodImpl(32)]
	public event Action onClick;

	public override void OnPointerDown(PointerEventData eventData)
	{
		eventData.Use();
		UICamera.uguiConsume = true;
		SignalClick();
	}

	private void SignalClick()
	{
		if (this.onClick != null)
		{
			this.onClick();
		}
	}

	private void OnSelect(bool selected)
	{
	}
}
