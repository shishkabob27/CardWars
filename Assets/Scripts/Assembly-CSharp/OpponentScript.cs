using UnityEngine;

public class OpponentScript : MonoBehaviour
{
	public PlayerInfoScript PlayerInfo;

	public CampaignScreenScript CampaignScreen;

	public GameObject Character;

	public GameObject ContinueButton;

	public GameObject NextScreen;

	public bool Locked;

	public bool Grow;

	public int ID;

	private void Start()
	{
		PlayerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfoScript>();
		Character.GetComponent<Animation>().Stop();
	}

	private void OnClick()
	{
		CampaignScreen.Selected = ID;
		CampaignScreen.UpdateOpponents();
		PlayerInfo.OpponentID = ID;
		if (!Locked)
		{
			ContinueButton.transform.localPosition = new Vector3(550f, -450f, 0f);
		}
		else
		{
			ContinueButton.transform.localPosition = new Vector3(550f, -1000f, 0f);
		}
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>());
	}

	public void UpdateSprite()
	{
		if (CampaignScreen.Selected == ID)
		{
			Character.GetComponent<Animation>().Play();
		}
		else
		{
			Character.GetComponent<Animation>().Stop();
		}
	}
}
