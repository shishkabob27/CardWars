using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWFuseShowRecipe : MonoBehaviour
{
	public GameObject targetCard;

	public string currentRecipeName;

	public UILabel targetCountLabel;

	public UILabel costLabel;

	public GameObject craftButton;

	public CWFuseCraft craftScript;

	public Collider craftButtonCollider;

	public ButtonReactionScript buttonReactionScript;

	public UITweener enabledTweener;

	public List<GameObject> ingredientCards;

	public List<GameObject> ingredientChecks;

	public List<UILabel> ingredientCountLabels;

	public AudioClip armSoundFX;

	private bool saveRequired;

	private PlayerInfoScript pInfo;

	private string recipename;

	private void OnEnable()
	{
		pInfo = PlayerInfoScript.GetInstance();
		saveRequired = false;
	}

	private void OnDisable()
	{
		if (saveRequired)
		{
			PlayerInfoScript.GetInstance().Save();
		}
	}

	public void ShowRecipe(string recipeName, bool markAsViewed)
	{
		ShowRecipe(recipeName, markAsViewed, false, false);
	}

	public void ShowRecipe(string recipeName, bool markAsViewed, bool armSound, bool ignoreInputEnabler)
	{
		StartCoroutine(ShowRecipeAfterArmTween(recipeName, markAsViewed, armSound, ignoreInputEnabler));
	}

	private IEnumerator ShowRecipeAfterArmTween(string recipeName, bool markAsViewed, bool armSound, bool ignoreInputEnabler)
	{
		recipename = recipeName;
		if (!ignoreInputEnabler)
		{
			UICamera.useInputEnabler = true;
		}
		if (armSound)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(armSoundFX);
		}
		TweenPosition tw = targetCard.GetComponent<TweenPosition>();
		tw.Play(false);
		yield return new WaitForSeconds(0.5f);
		PlayerDeckManager deckMgr = pInfo.DeckManager;
		PanelManagerDeck pMgrDeck = PanelManagerDeck.GetInstance();
		CardForm form = CardDataManager.Instance.GetCard(recipeName);
		CardItem target = new CardItem(form, 1, false);
		CWDeckCard script = targetCard.GetComponent<CWDeckCard>();
		if ((bool)script)
		{
			script.card = target;
			script.Responsive = false;
		}
		pMgrDeck.FillCardInfo(targetCard, target);
		targetCountLabel.text = string.Format(KFFLocalization.Get("!!DM_C_0_INVENTORY"), deckMgr.CardCount(form));
		if (markAsViewed)
		{
			saveRequired |= FusionManager.Instance.MarkAsViewed(form);
		}
		UpdateButtonState();
		tw.Play(true);
		yield return new WaitForSeconds(0.5f);
		bool tutorialTriggered = false;
		int CRAFTING_UNLOCK_LEVEL = 4;
		QuestData qd = QuestManager.Instance.GetQuestByID("main", CRAFTING_UNLOCK_LEVEL);
		int progress = pInfo.GetQuestProgress(qd);
		if (progress > 0)
		{
			if (TutorialMonitor.Instance != null)
			{
				tutorialTriggered = TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.CardCraftingUnlocked);
			}
		}
		else if (TutorialMonitor.Instance != null)
		{
			tutorialTriggered = TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.CardCraftingLocked);
		}
		if (!ignoreInputEnabler && !CWFuseCraft.isCrafting && !tutorialTriggered)
		{
			UICamera.useInputEnabler = false;
		}
	}

	public void UpdateButtonState()
	{
		CardForm card = CardDataManager.Instance.GetCard(recipename);
		RecipeData recipe = FusionManager.Instance.GetRecipe(card);
		costLabel.text = recipe.cost.ToString();
		bool flag = pInfo.Coins > recipe.cost;
		bool flag2 = false;
		bool flag3 = true;
		PlayerDeckManager deckManager = pInfo.DeckManager;
		PanelManagerDeck instance = PanelManagerDeck.GetInstance();
		for (int i = 0; i < 3; i++)
		{
			bool flag4 = i < recipe.ingredients.Count;
			NGUITools.SetActive(ingredientCards[i].transform.parent.gameObject, flag4);
			if (flag4)
			{
				RecipeIngredientData recipeIngredientData = recipe.ingredients[i];
				CardItem card2 = new CardItem(recipeIngredientData.Form, 1, false);
				CWDeckCard component = ingredientCards[i].GetComponent<CWDeckCard>();
				if ((bool)component)
				{
					component.card = card2;
					component.Responsive = false;
				}
				instance.FillCardInfo(ingredientCards[i], card2);
				int count = recipeIngredientData.Count;
				int num = deckManager.CardCount(recipeIngredientData.Form);
				bool flag5 = num >= count;
				flag = flag && flag5;
				if (flag3)
				{
					flag3 = false;
					flag2 = flag5;
				}
				else
				{
					flag2 = flag2 && flag5;
				}
				NGUITools.SetActive(ingredientChecks[i], flag5);
				SQUtils.SetActive(ingredientCards[i], "Panel/Panel/Card_Dim", !flag5);
				ingredientCountLabels[i].text = string.Format("{0}/{1}", num, count);
			}
		}
		SQUtils.SetGray(craftButton, (!flag) ? 0.4f : 1f);
		SQUtils.SetActive(targetCard, "Panel/Panel/Card_Dim", !flag);
		craftButtonCollider.enabled = true;
		if (buttonReactionScript != null)
		{
			buttonReactionScript.enabled = flag;
		}
		if (enabledTweener != null)
		{
			enabledTweener.Play(true);
			enabledTweener.Reset();
			enabledTweener.enabled = flag;
		}
		craftScript.recipeName = recipename;
		craftScript.canCraft = flag;
		craftScript.sufficient = flag2;
		craftScript.showRecipe = this;
		targetCountLabel.text = string.Format(KFFLocalization.Get("!!DM_C_0_INVENTORY"), deckManager.CardCount(card));
	}

	private void DebugGainRecipeIngredient(string recipeName)
	{
		PlayerDeckManager deckManager = pInfo.DeckManager;
		CardForm card = CardDataManager.Instance.GetCard(recipeName);
		RecipeData recipe = FusionManager.Instance.GetRecipe(card);
		for (int i = 0; i < 3; i++)
		{
			if (i < recipe.ingredients.Count)
			{
				RecipeIngredientData recipeIngredientData = recipe.ingredients[i];
				CardItem card2 = new CardItem(recipeIngredientData.Form, 1, false);
				deckManager.AddCard(card2);
			}
		}
		pInfo.Save();
	}

	private void Update()
	{
		if (Input.GetKeyDown("d"))
		{
			DebugGainRecipeIngredient(currentRecipeName);
			ShowRecipe(currentRecipeName, false);
		}
	}
}
