using System.Collections;
using UnityEngine;

public class CWFuseCraft : MonoBehaviour
{
	public string recipeName;

	public CWFuseFuseCards panelScript;

	public bool canCraft;

	public bool sufficient;

	public CWFuseShowRecipe showRecipe;

	public AudioClip errorSound;

	public AudioClip okSound;

	public CWFuseCraftSequencer craftSqcr;

	public static bool isCrafting;

	private void OnClick()
	{
		UIButtonSound component = GetComponent<UIButtonSound>();
		component.audioClip = errorSound;
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		PlayerDeckManager deckManager = instance.DeckManager;
		CardForm card = CardDataManager.Instance.GetCard(recipeName);
		RecipeData recipe = FusionManager.Instance.GetRecipe(card);
		craftSqcr = CWFuseCraftSequencer.GetInstance();
		if (!sufficient)
		{
			TutorialMonitor.Instance.QueueTutorial(TutorialTrigger.CardCraftingError);
			return;
		}
		if (!canCraft)
		{
			TutorialMonitor.Instance.QueueTutorial(TutorialTrigger.CardCraftingError);
			return;
		}
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			if (i < recipe.ingredients.Count && recipe.ingredients[i].Form.Type == CardType.Creature)
			{
				num += recipe.ingredients[i].Count;
			}
		}
		int num2 = deckManager.CreatureCount() - num;
		if (card.Type == CardType.Creature)
		{
			num2++;
		}
		if (num2 < ParametersManager.Instance.Min_Creatures_In_Inventory)
		{
			TutorialMonitor.Instance.QueueTutorial(TutorialTrigger.NotEnoughCreatures);
			return;
		}
		isCrafting = true;
		UICamera.useInputEnabler = true;
		component.audioClip = okSound;
		craftSqcr.TriggerCraftSequence(card, recipe);
		instance.Coins -= recipe.cost;
		for (int j = 0; j < 3; j++)
		{
			if (j < recipe.ingredients.Count)
			{
				RecipeIngredientData recipeIngredientData = recipe.ingredients[j];
				deckManager.RemoveCards(recipeIngredientData.Form, recipeIngredientData.Count);
			}
		}
		CardItem cardItem = new CardItem(card);
		deckManager.AddCard(cardItem);
		Singleton<AnalyticsManager>.Instance.LogCardCrafted(cardItem.Form.ID);
		PlayerInfoScript.GetInstance().Save();
		StartCoroutine(DelayUpdateDisplay());
	}

	private IEnumerator DelayUpdateDisplay()
	{
		yield return new WaitForSeconds(1f);
		panelScript.Populate(recipeName);
		if (showRecipe != null)
		{
			showRecipe.UpdateButtonState();
		}
	}
}
