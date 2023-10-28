using UnityEngine;

public class DebugCraftRecipe : MonoBehaviour
{
	public string recipeName;

	private void OnClick()
	{
		if (!(recipeName == string.Empty))
		{
			CardForm card = CardDataManager.Instance.GetCard(recipeName);
			RecipeData recipe = FusionManager.Instance.GetRecipe(card);
			CWFuseCraftSequencer instance = CWFuseCraftSequencer.GetInstance();
			instance.TriggerCraftSequence(card, recipe);
		}
	}
}
