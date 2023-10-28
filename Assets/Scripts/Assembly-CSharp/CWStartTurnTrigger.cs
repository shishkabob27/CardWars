using UnityEngine;

public class CWStartTurnTrigger : MonoBehaviour
{
	public int player;

	private GameState GameInstance;

	private void OnClick()
	{
		GameInstance = GameState.Instance;
		StartTurn();
	}

	private void StartTurn()
	{
		GameInstance.EndTurn(!(PlayerType)player);
		GameInstance.StartTurn(player);
	}

	private void Update()
	{
	}
}
