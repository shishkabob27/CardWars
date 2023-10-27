using UnityEngine;

public class CWUpdatePlayerData : MonoBehaviour
{
	public UILabel healthLabel;
	public UIFilledSprite healthBar;
	public UIFilledSprite healthBarDamage;
	public UILabel deckLabel;
	public UILabel discardLabel;
	public UILabel magicLabel;
	public UILabel lootLabel;
	public UILabel coinsLabel;
	public UILabel gemLabel;
	public UIFilledSprite magicBar;
	public UISprite portrait;
	public UILabel playerName;
	public int player;
	public float maxHP;
	public float currentHP;
	public float prevHP;
	public UILabel magicLabelAux;
	public UIFilledSprite magicBarAux;
	public UILabel turnCounter;
	public UISprite turnBG;
	public UILabel turnLbl;
	public GameObject leaderButton;
	public TriggerVFX magicPointUp;
	public TriggerVFX magicPointDown;
}
