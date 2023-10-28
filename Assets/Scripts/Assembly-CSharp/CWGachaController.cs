using UnityEngine;

public class CWGachaController : MonoBehaviour
{
	public enum ChestType
	{
		Normal,
		Premium
	}

	public ChestType activeChest;

	public bool canChooseChest = true;

	public UILabel gatchaCostLabel;

	public GameObject yesButton;

	public UITweener yesButtonTweener;

	public GameObject noButton;

	public UIButtonPlayAnimation openAnimScript;

	public Animation normalChestAnim;

	public Animation premiumChestAnim;

	public UIButtonPlayAnimation closeAnimScript;

	public GameObject cardReveal;

	public UILabel gatchaResult;

	public int costCoins;

	public int costGems;

	public GameObject storeButton;

	public GameObject normalWindow;

	public GameObject keyWindow;

	public GameObject keyInfoBar;

	public UIButtonPlayAnimation openCameraScript;

	public UIButtonPlayAnimation closeCameraScript;

	public TriggerVFX vfx;

	public AudioClip[] SFX_Rarity;

	public CWGachaFlyCard flyCardScript;

	public string cardID;

	public string carnivalString;

	private bool openPremiumChestForFree;

	private float bonusGemStartTime;

	private int bonusGemCount;

	private static CWGachaController g_gathaManager;

	[HideInInspector]
	public bool OpenPremiumChestWithKey;

	public bool OpenPremiumChestForFree
	{
		get
		{
			return openPremiumChestForFree;
		}
		set
		{
			openPremiumChestForFree = value;
			if (value)
			{
				bonusGemStartTime = Time.realtimeSinceStartup;
				GachaManager instance = GachaManager.Instance;
				int coins = 0;
				int gems = 0;
				instance.GetChestCost(true, ref coins, ref gems);
				bonusGemCount = gems;
			}
		}
	}

	private void Awake()
	{
		g_gathaManager = this;
	}

	public static CWGachaController GetInstance()
	{
		return g_gathaManager;
	}

	public void ActivateLoopedIdleAnims()
	{
		if (normalChestAnim != null)
		{
			normalChestAnim.enabled = true;
			normalChestAnim.Play();
		}
		if (premiumChestAnim != null)
		{
			premiumChestAnim.enabled = true;
			premiumChestAnim.Play();
		}
		SetWorldChestCollider(true);
	}

	public void SetWorldChestCollider(bool enable)
	{
		BoxCollider[] componentsInChildren = normalChestAnim.transform.parent.parent.parent.GetComponentsInChildren<BoxCollider>(true);
		BoxCollider[] array = componentsInChildren;
		foreach (BoxCollider boxCollider in array)
		{
			boxCollider.enabled = enable;
		}
	}

	public int GetBonusGemCount()
	{
		if (openPremiumChestForFree)
		{
			float num = 0.5f;
			float num2 = Time.realtimeSinceStartup - bonusGemStartTime;
			int a = (int)(num2 / num);
			return Mathf.Min(a, bonusGemCount);
		}
		return 0;
	}
}
