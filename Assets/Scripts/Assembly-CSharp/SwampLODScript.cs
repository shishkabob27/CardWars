using UnityEngine;

public class SwampLODScript : MonoBehaviour
{
	public Texture HighResSwamp;

	public Texture LowResSwamp;

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
			GetComponent<Renderer>().materials[1].mainTexture = HighResSwamp;
		}
		else
		{
			GetComponent<Renderer>().materials[1].mainTexture = LowResSwamp;
		}
	}
}
