using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class LoadingScreenScript : MonoBehaviour
{
	private AsyncOperation async;

	private void Start()
	{
		if (GameState.Instance == null)
		{
			return;
		}
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		if (activeQuest == null)
		{
			return;
		}
		UITexture componentInChildren = GetComponentInChildren<UITexture>();
		if (!string.IsNullOrEmpty(activeQuest.LoadingScreenTextureName))
		{
			Texture texture = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(activeQuest.LoadingScreenTextureName) as Texture;
			if (texture != null)
			{
				componentInChildren.mainTexture = texture;
			}
		}
	}
}
