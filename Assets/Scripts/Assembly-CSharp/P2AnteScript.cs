using UnityEngine;

public class P2AnteScript : MonoBehaviour
{
	public GameObject Splash;

	public Transform Cup;

	public float Float;

	public bool Fall;

	public bool Splashed;

	public bool Bob;

	public bool Up;

	private void Update()
	{
		if (!Bob)
		{
			if (Fall)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, Cup.position, Time.deltaTime * 10f);
				if (base.transform.position.y < 4f && !Splashed)
				{
					Object.Instantiate(Splash, Cup.position, Quaternion.identity);
					SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>());
					Splashed = true;
				}
				if ((double)base.transform.position.y < (double)Cup.position.y + 0.01)
				{
					base.transform.position = Cup.position;
					base.transform.parent = Cup.transform.parent;
					Fall = false;
					Bob = true;
				}
			}
			return;
		}
		if (Up)
		{
			Float += Time.deltaTime * 0.01f;
			if (Float > 0.01f)
			{
				Float = 0.01f;
				Up = false;
			}
		}
		else
		{
			Float -= Time.deltaTime * 0.01f;
			if (Float < -0.01f)
			{
				Float = -0.01f;
				Up = true;
			}
		}
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y + Float, base.transform.localPosition.z);
	}
}
