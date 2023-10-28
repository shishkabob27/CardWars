using UnityEngine;

public class ResultsDweebCardScript : MonoBehaviour
{
	public Vector3 Destination;

	public UISprite DweebSprite;

	public UILabel NameLabel;

	public UILabel DescLabel;

	public float Scale;

	public bool Unlock;

	public bool Grow;

	public AudioClip CardMove;

	public CardItem CardData;

	public void UpdateCard()
	{
		if (CardData != null)
		{
			DweebSprite.spriteName = CardData.Form.SpriteName;
			NameLabel.text = CardData.Form.Name;
			DescLabel.text = CardData.Description;
		}
	}

	private void Update()
	{
		if (Unlock)
		{
			if (Grow)
			{
				Scale = SQUtils.Lerp(Scale, 1.25f, Time.deltaTime * 10f);
				base.transform.localScale = new Vector3(Scale, Scale, Scale);
			}
			else
			{
				Scale = SQUtils.Lerp(Scale, 0f, Time.deltaTime * 10f);
				base.transform.localScale = new Vector3(Scale, Scale, Scale);
			}
		}
		else
		{
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, Destination, Time.deltaTime * 5f);
		}
	}
}
