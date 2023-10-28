using System.Collections.Generic;
using UnityEngine;

public class KeyListController : MonoBehaviour
{
	public UIButtonTween ShowTween;

	public UIButtonTween HideTween;

	public GameObject TemplateItem;

	public UIGrid Grid;

	public UISprite ItemIcon;

	public UILabel ItemType;

	public UILabel ItemInfo;

	public GameObject BackCollider;

	private KeyItemController currentController;

	private bool KeyRingActive;

	private static void TestKeys()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		instance.GachaKeys.AddKey("Key_Gold");
		instance.GachaKeys.AddKey("Key_Gold");
		instance.GachaKeys.AddKey("Gold1");
		instance.GachaKeys.AddKey("Key_Black");
		instance.GachaKeys.AddKey("Key_Black");
		instance.GachaKeys.AddKey("Key_Red");
		instance.Save();
	}

	public void Show()
	{
		ClearList(Grid);
		KeyRingActive = true;
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance.GachaKeys._GachaKeys == null || instance.GachaKeys._GachaKeys.Count == 0)
		{
			return;
		}
		Dictionary<string, KeyRingItem> keyRingItems = KeyRingDataManager.Instance.GetKeyRingItems();
		foreach (GachaKey gachaKey in instance.GachaKeys._GachaKeys)
		{
			KeyRingItem value;
			if (keyRingItems.TryGetValue(gachaKey.Type, out value))
			{
				GameObject gameObject = NGUITools.AddChild(Grid.gameObject, TemplateItem);
				KeyItemController component = gameObject.GetComponent<KeyItemController>();
				component.SetData(value);
				component.OnSelectEvent += OnSelectEvent;
			}
		}
		Grid.repositionNow = true;
		ShowTween.Play(true);
		if (BackCollider != null)
		{
			BackCollider.SetActive(true);
		}
	}

	public void Hide()
	{
		KeyRingActive = false;
		HideTween.Play(true);
		if (currentController != null)
		{
			currentController.EnableHighlight(false);
			currentController = null;
		}
		CWGachaController instance = CWGachaController.GetInstance();
		NGUITools.SetActive(instance.keyWindow, false);
		instance.OpenPremiumChestWithKey = false;
	}

	public void OnAcceptKey()
	{
		if (KeyRingActive)
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			instance.GachaKeys.ConsumeKey(currentController.keyRingItem.Type);
			CWGachaController instance2 = CWGachaController.GetInstance();
			Hide();
			instance2.OpenPremiumChestWithKey = true;
		}
	}

	private void ClearList(UIGrid list)
	{
		Transform transform = list.transform;
		for (int num = transform.childCount - 1; num >= 0; num--)
		{
			GameObject gameObject = transform.GetChild(num).gameObject;
			gameObject.SetActive(false);
			Object.Destroy(gameObject);
		}
	}

	private void Update()
	{
		if (!IsEmpty() && KeyRingActive)
		{
			CWGachaController instance = CWGachaController.GetInstance();
			if (currentController == null)
			{
				NGUITools.SetActive(instance.yesButton, false);
				NGUITools.SetActive(instance.noButton, true);
				NGUITools.SetActive(instance.storeButton, false);
			}
			else
			{
				NGUITools.SetActive(instance.yesButton, true);
				NGUITools.SetActive(instance.noButton, true);
			}
			if (BackCollider != null && !BackCollider.activeInHierarchy)
			{
				BackCollider.SetActive(true);
			}
		}
	}

	private void PickCard(string columns)
	{
		CWGachaController instance = CWGachaController.GetInstance();
		GachaManager instance2 = GachaManager.Instance;
		instance.cardID = null;
		if (currentController.keyRingItem.SpecialCards != null)
		{
			string[] specialCards = currentController.keyRingItem.SpecialCards;
			foreach (string text in specialCards)
			{
				if (!string.IsNullOrEmpty(text) && !LeaderManager.Instance.AlreadyOwned(text))
				{
					instance.cardID = text;
					break;
				}
			}
		}
		if (instance.cardID == null)
		{
			instance.cardID = instance2.PickColumn(columns);
		}
		if (instance.cardID == null)
		{
			instance.cardID = instance2.PickPremium();
		}
	}

	private void OnSelectEvent(KeyItemController itemController)
	{
		if (currentController != null)
		{
			currentController.EnableHighlight(false);
		}
		itemController.EnableHighlight(true);
		currentController = itemController;
		PickCard(itemController.keyRingItem.GachaColumn);
		ItemIcon.spriteName = itemController.keyRingItem.Icon;
		ItemType.text = KFFLocalization.Get(itemController.keyRingItem.Name);
		ItemInfo.text = KFFLocalization.Get(itemController.keyRingItem.Info);
		CWGachaController instance = CWGachaController.GetInstance();
		NGUITools.SetActive(instance.keyWindow, true);
		NGUITools.SetActive(instance.keyInfoBar, false);
		instance.OpenPremiumChestWithKey = true;
	}

	public bool IsEmpty()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		return instance.GachaKeys._GachaKeys == null || instance.GachaKeys._GachaKeys.Count == 0;
	}
}
