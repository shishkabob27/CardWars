using UnityEngine;

public class MatchScript : MonoBehaviour
{
	public PlayerInfoScript PlayerInfo;

	public PreMatchScreenScript PreMatchScreen;

	public GameObject ContinueButton;

	public GameObject Background;

	public GameObject NextScreen;

	public bool Grow;

	public int ID;

	private void Start()
	{
		PlayerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfoScript>();
	}

	private void OnClick()
	{
		PreMatchScreen.Selected = ID;
		PreMatchScreen.UpdateMatches();
		PlayerInfo.MatchID = ID;
		ContinueButton.transform.localPosition = new Vector3(550f, -450f, 0f);
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>());
	}

	private void Update()
	{
		if (!Grow)
		{
			base.transform.localScale = new Vector3(SQUtils.Lerp(base.transform.localScale.x, 0.75f, Time.deltaTime * 10f), SQUtils.Lerp(base.transform.localScale.y, 0.75f, Time.deltaTime * 10f), SQUtils.Lerp(base.transform.localScale.z, 0.75f, Time.deltaTime * 10f));
		}
		else
		{
			base.transform.localScale = new Vector3(SQUtils.Lerp(base.transform.localScale.x, 1f, Time.deltaTime * 10f), SQUtils.Lerp(base.transform.localScale.y, 1f, Time.deltaTime * 10f), SQUtils.Lerp(base.transform.localScale.z, 1f, Time.deltaTime * 10f));
		}
	}
}
