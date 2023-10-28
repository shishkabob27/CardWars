using UnityEngine;

public class CWGachaOpenChest : MonoBehaviour
{
	public CWMenuCameraTarget cameraTarget;

	public GameObject NormalCard;

	public GameObject PremiumCard;

	public GameObject VFX_RarityLow;

	public GameObject VFX_RarityMed;

	public GameObject VFX_RarityHigh;

	public GameObject VFX_Premium_Open;

	public GameObject VFX_Normal_Open;

	private void OnClick()
	{
		CWGachaController instance = CWGachaController.GetInstance();
		PlayerInfoScript instance2 = PlayerInfoScript.GetInstance();
		instance.SetWorldChestCollider(false);
		if ((instance2.Gems >= instance.costGems && instance2.Coins >= instance.costCoins) || instance.OpenPremiumChestForFree || instance.OpenPremiumChestWithKey)
		{
			if (!instance.OpenPremiumChestForFree && !instance.OpenPremiumChestWithKey)
			{
				instance2.Coins -= instance.costCoins;
				instance2.Gems -= instance.costGems;
			}
			GachaManager instance3 = GachaManager.Instance;
			PartyInfo currentPartyInfo = instance3.GetCurrentPartyInfo();
			if (currentPartyInfo != null && instance.activeChest == CWGachaController.ChestType.Premium && !instance.OpenPremiumChestWithKey)
			{
				instance.cardID = instance3.PickPremium(currentPartyInfo.gachaId);
			}
			if (cameraTarget != null)
			{
				cameraTarget.followFlag = false;
			}
			if (instance.activeChest == CWGachaController.ChestType.Normal)
			{
				Singleton<AnalyticsManager>.Instance.LogNormalChestPurchase();
			}
			else
			{
				Singleton<AnalyticsManager>.Instance.LogPremiumChestPurchase();
			}
			if (instance.activeChest == CWGachaController.ChestType.Premium && instance.vfx != null)
			{
				instance.vfx.useCustomCoords = false;
				instance.vfx.VFX = VFX_Premium_Open;
				instance.vfx.SpawnVFX();
			}
			if (instance.activeChest == CWGachaController.ChestType.Normal && instance.vfx != null)
			{
				instance.vfx.useCustomCoords = true;
				instance.vfx.VFX = VFX_Normal_Open;
				instance.vfx.SpawnVFX();
			}
			if (instance.activeChest == CWGachaController.ChestType.Premium && PremiumCard != null)
			{
				GameObject gameObject = (GameObject)SLOTGame.InstantiateFX(VFX_RarityHigh);
				gameObject.transform.parent = PremiumCard.transform;
				gameObject.transform.position = PremiumCard.transform.position;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			if (instance.activeChest == CWGachaController.ChestType.Normal && NormalCard != null)
			{
				GameObject gameObject2 = (GameObject)SLOTGame.InstantiateFX(VFX_RarityLow);
				gameObject2.transform.parent = NormalCard.transform;
				gameObject2.transform.position = NormalCard.transform.position;
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}
}
