using UnityEngine;

public class CornLODScript : MonoBehaviour
{
	public Texture HighResCorn;

	public Texture LowResCorn;

	private GameState GameInstance;

	private void Start()
	{
		if (Application.loadedLevelName == "BattleScene")
		{
			GameInstance = GameState.Instance;
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void Update()
	{
		if (GameInstance.IsSummoning(PlayerType.User) || GameInstance.IsSummoning(PlayerType.Opponent))
		{
			GetComponent<Renderer>().material.mainTexture = HighResCorn;
		}
		else
		{
			GetComponent<Renderer>().material.mainTexture = LowResCorn;
		}
	}
}
