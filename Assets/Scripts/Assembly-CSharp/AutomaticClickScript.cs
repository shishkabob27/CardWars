using UnityEngine;

public class AutomaticClickScript : MonoBehaviour
{
	public float Delay = 1f;

	public string Message = string.Empty;

	private float timer;

	private void OnEnable()
	{
		timer = 0f;
	}

	private void Update()
	{
		if (timer < Delay)
		{
			timer += Time.deltaTime;
			if (timer >= Delay)
			{
				base.gameObject.SendMessage(Message);
			}
		}
	}
}
