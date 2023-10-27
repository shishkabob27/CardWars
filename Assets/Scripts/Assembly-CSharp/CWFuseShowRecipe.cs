using UnityEngine;
using System.Collections.Generic;

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
}
