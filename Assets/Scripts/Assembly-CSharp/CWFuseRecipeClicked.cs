using UnityEngine;

public class CWFuseRecipeClicked : MonoBehaviour
{
	public string recipeName;

	public CWFuseShowRecipe showScript;

	public void OnClick()
	{
		if (recipeName != null && !(showScript == null))
		{
			GameObject go = base.transform.Find("Panel/Panel/New").gameObject;
			NGUITools.SetActive(go, false);
			ShowRecipe(showScript, recipeName, false);
		}
	}

	public static void ShowRecipe(CWFuseShowRecipe showscript, string recipename, bool ignoreInputEnabler)
	{
		if (showscript != null)
		{
			showscript.ShowRecipe(recipename, true, true, ignoreInputEnabler);
			showscript.currentRecipeName = recipename;
		}
	}

	public void ShowRecipe()
	{
		if (recipeName != null && !(showScript == null))
		{
			showScript.ShowRecipe(recipeName, false);
		}
	}
}
