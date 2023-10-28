using UnityEngine;
using UnityEngine.EventSystems;

public class UGuiToNGuiEventProxy : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public void OnPointerDown(PointerEventData eventData)
	{
		base.gameObject.SendMessage("OnPress", true, SendMessageOptions.DontRequireReceiver);
		eventData.Use();
		UICamera.uguiConsume = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		base.gameObject.SendMessage("OnPress", false, SendMessageOptions.DontRequireReceiver);
		eventData.Use();
		UICamera.uguiConsume = true;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		base.gameObject.SendMessage("OnHover", true, SendMessageOptions.DontRequireReceiver);
		eventData.Use();
		UICamera.uguiConsume = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		base.gameObject.SendMessage("OnHover", false, SendMessageOptions.DontRequireReceiver);
		eventData.Use();
		UICamera.uguiConsume = true;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		base.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		eventData.Use();
		UICamera.uguiConsume = true;
	}
}
