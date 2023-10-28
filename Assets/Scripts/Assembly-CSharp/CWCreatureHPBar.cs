using UnityEngine;

public class CWCreatureHPBar : MonoBehaviour
{
	private GameState GameInstance;

	public UILabel creatureHP;

	public UILabel creatureDEF;

	public UILabel creatureATK;

	public UIFilledSprite creatureHPBar;

	public UIFilledSprite creatureHPBarDamage;

	public UISprite ArrowATK;

	public UISprite ArrowDEF;

	private CWCreatureHPBarFaceCam parentScript;

	private CreatureScript creatureScript;

	public GameObject depletionFX;

	public GameObject refillFX;

	private Color hpBarOriginalColor;

	private Color atkLabelOriginalColor;

	private Color defLabelOriginalColor;

	private Color arrowATKOriginalColor;

	private Color arrowDEFOriginalColor;

	private int originalATK;

	private int originalDEF;

	public Color atkUpColor;

	public Color atkDownColor;

	public Color defUpColor;

	public Color defDownColor;

	public float maxHP;

	public float currentHP;

	public float prevHP;

	private bool stop;

	private GameObject particleObj;

	private void Start()
	{
		GameInstance = GameState.Instance;
		parentScript = base.transform.parent.GetComponent<CWCreatureHPBarFaceCam>();
		creatureScript = GameInstance.GetCreature(parentScript.playerType(parentScript.player), parentScript.lane);
		originalATK = creatureScript.ATK;
		originalDEF = creatureScript.DEF;
		maxHP = creatureScript.DEF;
		prevHP = maxHP;
		currentHP = maxHP;
		creatureHP.text = currentHP.ToString();
		creatureDEF.text = maxHP.ToString();
		hpBarOriginalColor = creatureHPBar.color;
		atkLabelOriginalColor = creatureATK.color;
		defLabelOriginalColor = creatureDEF.color;
		arrowATKOriginalColor = ArrowATK.color;
		arrowDEFOriginalColor = ArrowDEF.color;
	}

	public PlayerType playerType(int player)
	{
		return (player != 0) ? PlayerType.Opponent : PlayerType.User;
	}

	public int GetHealth(CreatureScript creatureScript)
	{
		if (creatureScript != null)
		{
			return creatureScript.Health;
		}
		return 0;
	}

	private void Update()
	{
		if (stop)
		{
			return;
		}
		creatureScript = GameInstance.GetCreature(parentScript.playerType(parentScript.player), parentScript.lane);
		if (creatureScript == null)
		{
			return;
		}
		currentHP = GetHealth(creatureScript);
		creatureHP.text = currentHP.ToString();
		creatureDEF.text = creatureScript.DEF.ToString();
		if (currentHP == 0f)
		{
			Invoke("DestoryHPBar", 1f);
		}
		maxHP = creatureScript.DEF;
		if (creatureATK != null)
		{
			creatureATK.text = creatureScript.ATK.ToString();
			NumberEffect(creatureScript.ATK, originalATK, atkUpColor, atkDownColor, creatureATK, ArrowATK, atkLabelOriginalColor, arrowATKOriginalColor);
		}
		if (creatureDEF != null)
		{
			NumberEffect(creatureScript.DEF, originalDEF, defUpColor, defDownColor, creatureDEF, ArrowDEF, defLabelOriginalColor, arrowDEFOriginalColor);
		}
		if (prevHP != currentHP)
		{
			prevHP = Mathf.Lerp(prevHP, currentHP, Time.deltaTime * 2f);
			creatureHPBarDamage.fillAmount = prevHP / maxHP;
			if (currentHP > prevHP)
			{
				creatureHPBar.fillAmount = prevHP / maxHP;
			}
			else
			{
				creatureHPBar.fillAmount = creatureScript.GetHealthPct();
			}
			if ((double)currentHP <= (double)maxHP * 0.25)
			{
				TriggerColorTween(true, Color.red, creatureHPBar);
			}
			else
			{
				TriggerColorTween(false, hpBarOriginalColor, creatureHPBar);
			}
		}
		if ((double)Mathf.Abs(currentHP - prevHP) < 0.01)
		{
			prevHP = currentHP;
		}
	}

	private void NumberEffect(int current, int original, Color upColor, Color downColor, UILabel num, UISprite arrow, Color numOriginal, Color arrowOriginal)
	{
		if (current > original)
		{
			TriggerColorTween(true, upColor, num);
			TriggerColorTween(true, upColor, arrow, true, true);
		}
		else if (current < original)
		{
			TriggerColorTween(true, downColor, num);
			TriggerColorTween(true, downColor, arrow, true, false);
		}
		else
		{
			TriggerColorTween(false, numOriginal, num);
			TriggerColorTween(false, arrowOriginal, arrow, true, false);
		}
	}

	private TweenPosition GetCurrentTween(bool up, UIWidget obj)
	{
		TweenPosition result = null;
		TweenPosition[] components = obj.GetComponents<TweenPosition>();
		int num = ((!up) ? 1 : 0);
		TweenPosition[] array = components;
		foreach (TweenPosition tweenPosition in array)
		{
			if (tweenPosition.tweenGroup == num)
			{
				result = tweenPosition;
				break;
			}
		}
		return result;
	}

	private void TriggerColorTween(bool enable, Color color, UIWidget obj, bool animatePos, bool up)
	{
		TweenPosition currentTween = GetCurrentTween(up, obj);
		if (currentTween != null)
		{
			if (enable && !currentTween.enabled)
			{
				currentTween.enabled = true;
				Quaternion localRotation = currentTween.transform.localRotation;
				float z = ((!up) ? 180f : 0f);
				currentTween.transform.localRotation = Quaternion.Euler(new Vector3(localRotation.x, localRotation.y, z));
				currentTween.Play(true);
			}
			if (!enable)
			{
				currentTween.enabled = false;
				obj.color = color;
			}
		}
		TriggerColorTween(enable, color, obj);
	}

	private void TriggerColorTween(bool enable, Color color, UIWidget obj)
	{
		TweenColor component = obj.GetComponent<TweenColor>();
		if (!(component == null))
		{
			if (enable && !component.enabled)
			{
				component.to = color;
				component.enabled = true;
				component.Play(enable);
			}
			if (!enable)
			{
				component.enabled = false;
				obj.color = color;
			}
		}
	}

	private void DestoryHPBar()
	{
		stop = true;
		Object.Destroy(base.transform.parent.gameObject);
	}
}
