using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickerManager : MonoBehaviour
{
	public class TickerMessage
	{
		public string Name;

		public string Message;

		public float TimeToNextMessage;

		public TickerMessage(string aMessage, float aTimeToNextMessage)
		{
			Message = aMessage;
			TimeToNextMessage = aTimeToNextMessage;
		}
	}

	private const float SPEED_TWEAK = 1.5f;

	public UILabel TickerLabel;

	private Vector3 TickerOriginalPos;

	public float Speed;

	public Camera MainCamera;

	private float TickerBoxLength;

	private List<TickerMessage> MessageList = new List<TickerMessage>();

	private TickerMessage CurrentMessage;

	private void AddMessage(string aMessage)
	{
		aMessage = "                          " + aMessage;
		float aTimeToNextMessage = CalculateTimeToNextMessage(aMessage);
		MessageList.Add(new TickerMessage(aMessage, aTimeToNextMessage));
	}

	private void RemoveMessage(TickerMessage aTickerMessage)
	{
		MessageList.Remove(aTickerMessage);
	}

	private void ClearMessages()
	{
		MessageList.Clear();
	}

	private float CalculateTimeToNextMessage(string aMessage)
	{
		float num = TickerBoxLength + (float)(aMessage.Length * TickerLabel.font.size);
		Vector3 position = new Vector3(Speed, 0f, 0f);
		return num / (((Vector2)MainCamera.WorldToScreenPoint(position)).x * 1.5f);
	}

	private void Start()
	{
		TickerBoxLength = base.gameObject.GetComponent<UIGrid>().cellWidth;
		TickerOriginalPos = base.gameObject.transform.position;
		AddMessage("this is a test");
		AddMessage("yupeeeee Ma I fly fly away, yahoo, hhahhahhahaahhaha!");
		AddMessage("3 little pigs..........");
		StartCoroutine(NextMessageRoutine());
	}

	private IEnumerator NextMessageRoutine()
	{
		while (true)
		{
			foreach (TickerMessage Ticker in MessageList)
			{
				TickerLabel.text = Ticker.Message;
				TickerLabel.transform.position = TickerOriginalPos;
				yield return new WaitForSeconds(Ticker.TimeToNextMessage);
			}
		}
	}

	private void Update()
	{
		TickerLabel.transform.position -= new Vector3(Speed * Time.deltaTime, 0f, 0f);
	}
}
