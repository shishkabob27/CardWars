using UnityEngine;

public class GameDataScript : MonoBehaviour
{
	public GameObject BackgroundBMO;
	public GameObject[] characterObjects;
	public GameObject[] characterPositions;
	public UIAtlas[] characterUiAtlas;
	public CWUpdatePlayerData[] playerData;
	public int FirstPlayer;
	public int P1_CoinsEarned;
	public int P2_CoinsEarned;
	public int ActivePlayer;
	public int Turn;
	public bool ShowAnims;
	public bool GameOver;
	public float Timer;
	public float Spin;
	public bool stopUpdateFlag;
}
