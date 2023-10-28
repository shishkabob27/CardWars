using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageController : MonoBehaviour
{
	public MenuController menu;

	public UILabel message;

	public GameObject gemInfo;

	public UILabel gems;

	public GameObject coinInfo;

	public UILabel coin;

	private Action rewardAction;

	private IEnumerator<string> messages;

	public void OnEnable()
	{
		messages = SessionManager.GetInstance().theSession.TheGame.MyMessages.GetEnumerator();
		ProcessMessage();
	}

	public void ProcessMessage()
	{
		if (rewardAction != null)
		{
			rewardAction();
		}
		if (!messages.MoveNext())
		{
			menu.SwitchToStart();
			return;
		}
		GiftMessage giftMessage = new GiftMessage(messages.Current);
		coin.text = giftMessage.Coins.ToString();
		coinInfo.SetActive(giftMessage.Coins > 0);
		gems.text = giftMessage.Gems.ToString();
		gemInfo.SetActive(giftMessage.Gems > 0);
		message.text = giftMessage.MessageText;
		rewardAction = giftMessage.ConfirmAction;
	}
}
