using UnityEngine;

public class NowLoadingScript : MonoBehaviour
{
	public UILabel Label;

	public bool SpawnDeckManager;

	private int Timer;

	private void Start()
	{
		Label = GetComponent<UILabel>();
		Label.enabled = false;
	}

	private void Update()
	{
		if (SpawnDeckManager)
		{
			if (Timer > 1)
			{
				SpawnDeckManager = false;
				Label.enabled = false;
				Timer = 0;
			}
			Timer++;
		}
	}
}
