using UnityEngine;

public class DungeonScreenController : MonoBehaviour
{
	public GameObject TemplateWorldItem;
	public GameObject TemplateStageItem;
	public UITweener StageActivationTween;
	public UILabel InfoLabel;
	public GameObject StartButton;
	public UIButtonTween NotEnoughStaminaTween;
	public UIButtonTween TooManyCardsTween;
	public UIGrid WidgetWorlds;
	public UIGrid WidgetStages;
	public GameObject heartEarnedPanel;
	public AudioClip heartEarnedSound;
	public UISprite leaderSprite;
	public UILabel leaderLabel;
	public UILabel leaderAbility;
	public GameObject leaderCardPanel;
	public AudioClip leaderAquiredSound;
	public UISprite recipeSprite;
	public UILabel recipeLabel;
	public UILabel recipeTypeLabel;
	public GameObject recipeAquiredPanel;
	public AudioClip recipeAquiredSound;
	public UILabel levelLimitLabel;
	public UILabel invalidHeroLabel;
}
