using UnityEngine;

public class CampaignScreenScript : MonoBehaviour
{
	public PlayerInfoScript PlayerInfo;

	public OpponentScript Opponent1Script;

	public OpponentScript Opponent2Script;

	public OpponentScript Opponent3Script;

	public OpponentScript Opponent4Script;

	public OpponentScript Opponent5Script;

	public OpponentScript Opponent6Script;

	public OpponentScript Opponent7Script;

	public OpponentScript Opponent8Script;

	public OpponentScript Opponent9Script;

	public OpponentScript Opponent10Script;

	public GameObject Opponent_2_Lock;

	public Collider Opponent_2_Collider;

	public GameObject Opponent_3_Lock;

	public Collider Opponent_3_Collider;

	public GameObject Opponent_4_Lock;

	public Collider Opponent_4_Collider;

	public GameObject Opponent_5_Lock;

	public Collider Opponent_5_Collider;

	public GameObject Opponent_6_Lock;

	public Collider Opponent_6_Collider;

	public GameObject Opponent_7_Lock;

	public Collider Opponent_7_Collider;

	public GameObject Opponent_8_Lock;

	public Collider Opponent_8_Collider;

	public GameObject Opponent_9_Lock;

	public Collider Opponent_9_Collider;

	public GameObject Opponent_10_Lock;

	public Collider Opponent_10_Collider;

	public int Selected;

	private void Start()
	{
		PlayerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfoScript>();
	}

	public void UpdateOpponents()
	{
		Opponent1Script.UpdateSprite();
		Opponent2Script.UpdateSprite();
		Opponent3Script.UpdateSprite();
		Opponent4Script.UpdateSprite();
		Opponent5Script.UpdateSprite();
		Opponent6Script.UpdateSprite();
		Opponent7Script.UpdateSprite();
		Opponent8Script.UpdateSprite();
		Opponent9Script.UpdateSprite();
		Opponent10Script.UpdateSprite();
	}
}
