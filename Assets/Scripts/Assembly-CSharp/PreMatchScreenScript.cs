using UnityEngine;

public class PreMatchScreenScript : MonoBehaviour
{
	public PlayerInfoScript PlayerInfo;

	public UISprite OpponentPortrait;

	public UILabel OpponentName;

	public MatchScript Match1Script;

	public MatchScript Match2Script;

	public MatchScript Match3Script;

	public MatchScript Match4Script;

	public MatchScript Match5Script;

	public GameObject Match_2_Lock;

	public GameObject Match_2_Shadow;

	public Collider Match_2_Collider;

	public GameObject Match_3_Lock;

	public GameObject Match_3_Shadow;

	public Collider Match_3_Collider;

	public GameObject Match_4_Lock;

	public GameObject Match_4_Shadow;

	public Collider Match_4_Collider;

	public GameObject Match_5_Lock;

	public GameObject Match_5_Shadow;

	public Collider Match_5_Collider;

	public int Selected;

	private void Start()
	{
		PlayerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfoScript>();
		if (PlayerInfo.MatchProgress < 5)
		{
			NGUITools.SetActive(Match_5_Lock, true);
			NGUITools.SetActive(Match_5_Shadow, true);
			Match_5_Collider.enabled = false;
		}
		else
		{
			NGUITools.SetActive(Match_5_Lock, false);
			NGUITools.SetActive(Match_5_Shadow, false);
			Match_5_Collider.enabled = true;
		}
		if (PlayerInfo.MatchProgress < 4)
		{
			NGUITools.SetActive(Match_4_Lock, true);
			NGUITools.SetActive(Match_4_Shadow, true);
			Match_4_Collider.enabled = false;
		}
		else
		{
			NGUITools.SetActive(Match_4_Lock, false);
			NGUITools.SetActive(Match_4_Shadow, false);
			Match_4_Collider.enabled = true;
		}
		if (PlayerInfo.MatchProgress < 3)
		{
			NGUITools.SetActive(Match_3_Lock, true);
			NGUITools.SetActive(Match_3_Shadow, true);
			Match_3_Collider.enabled = false;
		}
		else
		{
			NGUITools.SetActive(Match_3_Lock, false);
			NGUITools.SetActive(Match_3_Shadow, false);
			Match_3_Collider.enabled = true;
		}
		if (PlayerInfo.MatchProgress < 2)
		{
			NGUITools.SetActive(Match_2_Lock, true);
			NGUITools.SetActive(Match_2_Shadow, true);
			Match_2_Collider.enabled = false;
		}
		else
		{
			NGUITools.SetActive(Match_2_Lock, false);
			NGUITools.SetActive(Match_2_Shadow, false);
			Match_2_Collider.enabled = true;
		}
		PlayerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfoScript>();
		string[] array = new string[15]
		{
			string.Empty,
			"Portrait_Finn_Frame",
			"Portrait_Jake_Frame",
			"Portrait_Princess_Frame",
			"Portrait_Marceline_Frame",
			"Portrait_Beemo_Frame",
			"Portrait_Lumpy_Frame",
			"Portrait_Earl_Frame",
			"Portrait_IceKing_Frame",
			"Portrait_LadyRainicorn_Frame",
			"Portrait_Ricardio_Frame",
			"Portrait_Hunson_Frame",
			"Portrait_Gunter_Frame",
			"Portrait_DrDonut_Frame",
			"Portrait_FlamePrincess_Frame"
		};
		if (PlayerInfo.OpponentID >= 1 && PlayerInfo.OpponentID <= 14)
		{
			OpponentPortrait.spriteName = array[PlayerInfo.OpponentID];
			OpponentName.text = KFFLocalization.Get("!!OPPONENT_NAME_" + PlayerInfo.OpponentID);
		}
	}

	public void UpdateMatches()
	{
		if (Selected == 1)
		{
			Match1Script.Grow = true;
			Match2Script.Grow = false;
			Match3Script.Grow = false;
			Match4Script.Grow = false;
			Match5Script.Grow = false;
		}
		if (Selected == 2)
		{
			Match1Script.Grow = false;
			Match2Script.Grow = true;
			Match3Script.Grow = false;
			Match4Script.Grow = false;
			Match5Script.Grow = false;
		}
		if (Selected == 3)
		{
			Match1Script.Grow = false;
			Match2Script.Grow = false;
			Match3Script.Grow = true;
			Match4Script.Grow = false;
			Match5Script.Grow = false;
		}
		if (Selected == 4)
		{
			Match1Script.Grow = false;
			Match2Script.Grow = false;
			Match3Script.Grow = false;
			Match4Script.Grow = true;
			Match5Script.Grow = false;
		}
		if (Selected == 5)
		{
			Match1Script.Grow = false;
			Match2Script.Grow = false;
			Match3Script.Grow = false;
			Match4Script.Grow = false;
			Match5Script.Grow = true;
		}
	}
}
