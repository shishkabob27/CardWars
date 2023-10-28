using UnityEngine;

public class ResultsCardsScript : MonoBehaviour
{
	public bool Shrink;

	public float Scale;

	private void Update()
	{
		if (Shrink)
		{
			Scale = SQUtils.Lerp(Scale, 0f, Time.deltaTime * 10f);
			base.transform.localScale = new Vector3(Scale, Scale, Scale);
		}
		else
		{
			Scale = SQUtils.Lerp(Scale, 1f, Time.deltaTime * 10f);
			base.transform.localScale = new Vector3(Scale, Scale, Scale);
		}
	}
}
