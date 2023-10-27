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
}
