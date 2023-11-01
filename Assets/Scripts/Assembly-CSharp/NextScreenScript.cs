using UnityEngine;

public class NextScreenScript : MonoBehaviour
{
	public GameObject Jukebox;

	public AudioClip NewMusic;

	public GameObject NextScreen;

	public GameObject NextScreen_Facebook;

	public bool DoNotDestroy;

	public bool ChangeMusic;

	public bool LeaveAllowed = true;

	public bool LeaveScreen;

	public float GrowthSpeed = 0.05f;

	private void Start()
	{
		Jukebox = GameObject.Find("Jukebox");
	}

	private void OnClick()
	{
		if (LeaveAllowed)
		{
			LeaveScreen = true;
		}
		if (!LeaveScreen)
		{
			return;
		}
		if (ChangeMusic)
		{
			Jukebox.GetComponent<AudioSource>().clip = NewMusic;
		}
		if (base.transform.parent.transform.parent == null)
		{
			if (!DoNotDestroy)
			{
				Object.Destroy(base.transform.parent.gameObject);
			}
		}
		else if (!DoNotDestroy)
		{
			Object.Destroy(base.transform.parent.transform.parent.gameObject);
		}
	}
}
