using UnityEngine;

public class CWCreatureHPBar : MonoBehaviour
{
	public UILabel creatureHP;
	public UILabel creatureDEF;
	public UILabel creatureATK;
	public UIFilledSprite creatureHPBar;
	public UIFilledSprite creatureHPBarDamage;
	public UISprite ArrowATK;
	public UISprite ArrowDEF;
	public GameObject depletionFX;
	public GameObject refillFX;
	public Color atkUpColor;
	public Color atkDownColor;
	public Color defUpColor;
	public Color defDownColor;
	public float maxHP;
	public float currentHP;
	public float prevHP;
}
