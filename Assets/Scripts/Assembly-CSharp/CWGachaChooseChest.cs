using UnityEngine;

public class CWGachaChooseChest : MonoBehaviour
{
	public CWGachaController.ChestType activeChest;

	public KeyListController keyListController;

	private void OnClick()
	{
		CWGachaController instance = CWGachaController.GetInstance();
		if (!instance.canChooseChest)
		{
			return;
		}
		instance.canChooseChest = false;
		instance.activeChest = activeChest;
		GachaManager instance2 = GachaManager.Instance;
		PlayerInfoScript instance3 = PlayerInfoScript.GetInstance();
		if (keyListController != null && !keyListController.IsEmpty() && PlayerInfoScript.GetInstance().IsKeyGatchaUnlocked())
		{
			keyListController.Show();
			NGUITools.SetActive(instance.keyWindow, false);
			NGUITools.SetActive(instance.keyInfoBar, false);
		}
		else
		{
			instance.normalWindow.SetActive(true);
			instance.activeChest = activeChest;
			instance.cardID = ((instance.activeChest != CWGachaController.ChestType.Premium) ? instance2.PickNormal() : instance2.PickPremium());
			instance.costCoins = 0;
			instance.costGems = 0;
			instance2.GetChestCost(activeChest == CWGachaController.ChestType.Premium, ref instance.costCoins, ref instance.costGems);
			if (instance3.DeckManager.CardCount() >= instance3.MaxInventory)
			{
				NGUITools.SetActive(instance.yesButton, false);
				NGUITools.SetActive(instance.storeButton, false);
				return;
			}
			if (instance.costGems > 0)
			{
				if (!instance.OpenPremiumChestForFree && instance.costGems > instance3.Gems)
				{
					instance.gatchaCostLabel.text = string.Format(KFFLocalization.Get("!!ERROR_GATCHACHOOSECHEST_GEMS"), instance.costGems);
					NGUITools.SetActive(instance.yesButton, false);
					NGUITools.SetActive(instance.storeButton, true);
				}
				else
				{
					string text = string.Format(KFFLocalization.Get((activeChest != CWGachaController.ChestType.Premium) ? "!!CONFIRM_GATCHACHOOSECHEST_GEMS_NORMAL" : "!!CONFIRM_GATCHACHOOSECHEST_GEMS_PREMIUM"), instance.costGems, activeChest.ToString());
					instance.gatchaCostLabel.text = ((instance.carnivalString != null && !(instance.carnivalString == string.Empty) && activeChest != 0) ? string.Format("[FFEA00]{0}[FFFFFF]: {1}", KFFLocalization.Get(instance.carnivalString), text) : text);
					NGUITools.SetActive(instance.storeButton, false);
					NGUITools.SetActive(instance.yesButton, true);
				}
			}
			else if (instance.costCoins > instance3.Coins)
			{
				instance.gatchaCostLabel.text = string.Format(KFFLocalization.Get("!!ERROR_GATCHACHOOSECHEST_COINS"), instance.costCoins);
				NGUITools.SetActive(instance.yesButton, false);
				NGUITools.SetActive(instance.storeButton, false);
			}
			else
			{
				instance.gatchaCostLabel.text = string.Format(KFFLocalization.Get((activeChest != CWGachaController.ChestType.Premium) ? "!!CONFIRM_GATCHACHOOSECHEST_COINS_NORMAL" : "!!CONFIRM_GATCHACHOOSECHEST_COINS_PREMIUM"), instance.costCoins, activeChest.ToString());
				NGUITools.SetActive(instance.yesButton, true);
				NGUITools.SetActive(instance.storeButton, false);
			}
			if (instance.yesButtonTweener != null)
			{
				instance.yesButtonTweener.enabled = instance.OpenPremiumChestForFree;
				if (!instance.yesButtonTweener.enabled)
				{
					instance.yesButtonTweener.Reset();
					instance.yesButtonTweener.Play(true);
					instance.yesButtonTweener.Reset();
					instance.yesButtonTweener.enabled = false;
				}
			}
		}
		instance.flyCardScript.startTr = base.transform;
		switch (activeChest)
		{
		case CWGachaController.ChestType.Normal:
		{
			instance.openAnimScript.target = instance.normalChestAnim;
			instance.openAnimScript.clipName = "NChest_OpenLvl5_0";
			instance.closeAnimScript.target = instance.normalChestAnim;
			instance.closeAnimScript.clipName = "NChest_End";
			UIButtonPlayAnimation[] components2 = instance.openCameraScript.gameObject.GetComponents<UIButtonPlayAnimation>();
			UIButtonPlayAnimation[] array3 = components2;
			foreach (UIButtonPlayAnimation uIButtonPlayAnimation3 in array3)
			{
				uIButtonPlayAnimation3.clipName = "NChest_OpenLvl5_0";
			}
			components2 = instance.closeCameraScript.gameObject.GetComponents<UIButtonPlayAnimation>();
			UIButtonPlayAnimation[] array4 = components2;
			foreach (UIButtonPlayAnimation uIButtonPlayAnimation4 in array4)
			{
				uIButtonPlayAnimation4.clipName = "NChest_End";
			}
			break;
		}
		case CWGachaController.ChestType.Premium:
		{
			instance.openAnimScript.target = instance.premiumChestAnim;
			instance.openAnimScript.clipName = "PChest_OpenLvl5_0";
			instance.closeAnimScript.target = instance.premiumChestAnim;
			instance.closeAnimScript.clipName = "PChest_End";
			UIButtonPlayAnimation[] components = instance.openCameraScript.gameObject.GetComponents<UIButtonPlayAnimation>();
			UIButtonPlayAnimation[] array = components;
			foreach (UIButtonPlayAnimation uIButtonPlayAnimation in array)
			{
				uIButtonPlayAnimation.clipName = "PChest_OpenLvl5_0";
			}
			components = instance.closeCameraScript.gameObject.GetComponents<UIButtonPlayAnimation>();
			UIButtonPlayAnimation[] array2 = components;
			foreach (UIButtonPlayAnimation uIButtonPlayAnimation2 in array2)
			{
				uIButtonPlayAnimation2.clipName = "PChest_End";
			}
			break;
		}
		}
	}
}
