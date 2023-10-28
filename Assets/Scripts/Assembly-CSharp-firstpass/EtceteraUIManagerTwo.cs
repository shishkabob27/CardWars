using Prime31;
using UnityEngine;

public class EtceteraUIManagerTwo : MonoBehaviourGUI
{
	private void OnGUI()
	{
		beginColumn();
		if (GUILayout.Button("Show Inline Web View"))
		{
			EtceteraAndroid.inlineWebViewShow("http://prime31.com/", 160, 430, Screen.width - 160, Screen.height - 100);
		}
		if (GUILayout.Button("Close Inline Web View"))
		{
			EtceteraAndroid.inlineWebViewClose();
		}
		if (GUILayout.Button("Set Url of Inline Web View"))
		{
			EtceteraAndroid.inlineWebViewSetUrl("http://google.com");
		}
		if (GUILayout.Button("Set Frame of Inline Web View"))
		{
			EtceteraAndroid.inlineWebViewSetFrame(80, 50, 300, 400);
		}
		endColumn(true);
		if (GUILayout.Button("Schedule Notification in 5 Seconds"))
		{
			EtceteraAndroid.scheduleNotification(5L, "Notiifcation Title", "The subtitle of the notification", "Ticker text gets ticked", "my-special-data");
		}
		if (GUILayout.Button("Schedule Notification in 10 Seconds"))
		{
			EtceteraAndroid.scheduleNotification(10L, "Notiifcation Title", "The subtitle of the notification", "Ticker text gets ticked", "my-special-data");
		}
		if (GUILayout.Button("Check for Noitifications"))
		{
			EtceteraAndroid.checkForNotifications();
		}
		if (GUILayout.Button("Quit App"))
		{
			Application.Quit();
		}
		endColumn();
		if (bottomRightButton("Previous Scene"))
		{
			Application.LoadLevel("EtceteraTestScene");
		}
	}
}
