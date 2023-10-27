using UnityEngine;

public class MapControllerBase : MonoBehaviour
{
	public string dbMapJsonFileName;
	public GameObject QuestMapRoot;
	public string QuestMapPrefabPath;
	public MapInfo QuestMapInfo;
	public GameObject playerIcon;
	public Camera uiCameraMap;
	public GameObject vfx;
	public GameObject vfxPrefab;
	public GameObject vfxStar;
	public GameObject vfxStarPrefab;
	public GameObject bonusQuestPrefab;
	public AudioClip bonusQuestAlertSound;
	public UISprite regionSprite;
	public GameObject regionPanel;
	public AudioClip regionUnlockSound;
	public AudioClip regionUnlockQuestSound;
	public UISprite recipeSprite;
	public UILabel recipeLabel;
	public UILabel recipeTypeLabel;
	public UISprite leaderSprite;
	public UILabel leaderLabel;
	public UILabel leaderAbility;
	public GameObject heartEarnedPanel;
	public AudioClip heartEarnedSound;
	public GameObject leaderCardPanel;
	public AudioClip leaderAquiredSound;
	public GameObject recipeAquiredPanel;
	public AudioClip recipeAquiredSound;
	public GameObject gemEarnedPanel;
	public AudioClip gemEarnedSound;
	public GameObject endOfRoadPanel;
	public AudioClip endOfRoadSound;
	public AudioClip[] playerMoveSounds;
	public AudioClip starSound;
	public GameObject questPanel;
	public bool resumeFlag;
	public string LoadingScreenTextureName;
}
