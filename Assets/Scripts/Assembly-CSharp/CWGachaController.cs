using UnityEngine;

public class CWGachaController : MonoBehaviour
{
	public enum ChestType
	{
		Normal = 0,
		Premium = 1,
	}

	public ChestType activeChest;
	public bool canChooseChest;
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
	public bool OpenPremiumChestWithKey;
}
