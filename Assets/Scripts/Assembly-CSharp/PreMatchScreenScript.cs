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
}
