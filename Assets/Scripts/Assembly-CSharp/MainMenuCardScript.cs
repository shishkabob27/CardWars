using UnityEngine;

public class MainMenuCardScript : MonoBehaviour
{
	public GameObject NextScreen;

	public CarouselScript CarouselScript;

	public GameObject FBX;

	public GameObject Jukebox;

	public AudioClip NewMusic;

	public bool DoNotProceed;

	public int ID;

	private void Start()
	{
		Jukebox = GameObject.Find("Jukebox");
		if (FBX != null)
		{
			FBX.GetComponent<Animation>()["Take 001"].speed = 0.5f;
		}
	}

	private void OnClick()
	{
		CarouselScript.IgnoreClick = true;
		CarouselScript.Target = ID;
		if ((double)base.transform.position.z > -0.5 && (double)base.transform.position.z < 0.5 && !DoNotProceed && NextScreen != null)
		{
			Object.Instantiate(NextScreen);
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayMusic(Jukebox.GetComponent<AudioSource>(), NewMusic, 0f, 1f);
			Object.Destroy(base.transform.parent.transform.parent.gameObject);
		}
		DoNotProceed = false;
	}

	private void Update()
	{
		if ((double)base.transform.position.z < -0.5 || (double)base.transform.position.z > 0.5)
		{
			if (FBX != null)
			{
				FBX.GetComponent<Animation>().Stop();
			}
			if (Input.touchCount > 0 || Input.GetMouseButton(0))
			{
				DoNotProceed = true;
			}
		}
		else if (FBX != null)
		{
			FBX.GetComponent<Animation>().Play();
		}
		base.transform.eulerAngles = new Vector3(0f, 0f, 0f);
	}
}
