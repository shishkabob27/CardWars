using UnityEngine;

public class CWMapQuestInfoSet : MonoBehaviour
{
	public QuestData questData;

	public UISprite[] stars;

	public UISprite charIcon;

	public UISprite unlockedFrameIcon;

	public UISprite lockedFrameIcon;

	private void Start()
	{
	}

	private void OnClick()
	{
		MapControllerBase instance = MapControllerBase.GetInstance();
		PlayerInfoScript instance2 = PlayerInfoScript.GetInstance();
		GlobalFlags instance3 = GlobalFlags.Instance;
		instance3.lastQuestConditionStatus = instance2.GetQuestProgress(questData);
		instance3.lastQuestMapCameraIdealPos = instance.mainCamera.transform.position;
		instance.SaveCameraPos(instance.mainCamera.transform.position, instance.mainCamera.GetComponent<Camera>().orthographicSize);
		instance3.lastQuestMapCameraFOV = instance.mainCamera.GetComponent<Camera>().orthographicSize;
		instance3.lastGem = instance2.Gems;
		instance3.lastStaminaMax = instance2.Stamina_Max;
	}
}
