using System;
using System.IO;
using System.Runtime.CompilerServices;
using Prime31;

public class EtceteraAndroidManager : AbstractManager
{
	[method: MethodImpl(32)]
	public static event Action<string> alertButtonClickedEvent;

	[method: MethodImpl(32)]
	public static event Action alertCancelledEvent;

	[method: MethodImpl(32)]
	public static event Action<string> promptFinishedWithTextEvent;

	[method: MethodImpl(32)]
	public static event Action promptCancelledEvent;

	[method: MethodImpl(32)]
	public static event Action<string, string> twoFieldPromptFinishedWithTextEvent;

	[method: MethodImpl(32)]
	public static event Action twoFieldPromptCancelledEvent;

	[method: MethodImpl(32)]
	public static event Action webViewCancelledEvent;

	[method: MethodImpl(32)]
	public static event Action albumChooserCancelledEvent;

	[method: MethodImpl(32)]
	public static event Action<string> albumChooserSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action photoChooserCancelledEvent;

	[method: MethodImpl(32)]
	public static event Action<string> photoChooserSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> videoRecordingSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action videoRecordingCancelledEvent;

	[method: MethodImpl(32)]
	public static event Action ttsInitializedEvent;

	[method: MethodImpl(32)]
	public static event Action ttsFailedToInitializeEvent;

	[method: MethodImpl(32)]
	public static event Action askForReviewWillOpenMarketEvent;

	[method: MethodImpl(32)]
	public static event Action askForReviewRemindMeLaterEvent;

	[method: MethodImpl(32)]
	public static event Action askForReviewDontAskAgainEvent;

	[method: MethodImpl(32)]
	public static event Action<string> inlineWebViewJSCallbackEvent;

	[method: MethodImpl(32)]
	public static event Action<string> notificationReceivedEvent;

	static EtceteraAndroidManager()
	{
		AbstractManager.initialize(typeof(EtceteraAndroidManager));
	}

	public void alertButtonClicked(string positiveButton)
	{
		if (EtceteraAndroidManager.alertButtonClickedEvent != null)
		{
			EtceteraAndroidManager.alertButtonClickedEvent(positiveButton);
		}
	}

	public void alertCancelled(string empty)
	{
		if (EtceteraAndroidManager.alertCancelledEvent != null)
		{
			EtceteraAndroidManager.alertCancelledEvent();
		}
	}

	public void promptFinishedWithText(string text)
	{
		string[] array = text.Split(new string[1] { "|||" }, StringSplitOptions.None);
		if (array.Length == 1 && EtceteraAndroidManager.promptFinishedWithTextEvent != null)
		{
			EtceteraAndroidManager.promptFinishedWithTextEvent(array[0]);
		}
		if (array.Length == 2 && EtceteraAndroidManager.twoFieldPromptFinishedWithTextEvent != null)
		{
			EtceteraAndroidManager.twoFieldPromptFinishedWithTextEvent(array[0], array[1]);
		}
	}

	public void promptCancelled(string empty)
	{
		if (EtceteraAndroidManager.promptCancelledEvent != null)
		{
			EtceteraAndroidManager.promptCancelledEvent();
		}
	}

	public void twoFieldPromptCancelled(string empty)
	{
		if (EtceteraAndroidManager.twoFieldPromptCancelledEvent != null)
		{
			EtceteraAndroidManager.twoFieldPromptCancelledEvent();
		}
	}

	public void webViewCancelled(string empty)
	{
		if (EtceteraAndroidManager.webViewCancelledEvent != null)
		{
			EtceteraAndroidManager.webViewCancelledEvent();
		}
	}

	public void albumChooserCancelled(string empty)
	{
		if (EtceteraAndroidManager.albumChooserCancelledEvent != null)
		{
			EtceteraAndroidManager.albumChooserCancelledEvent();
		}
	}

	public void albumChooserSucceeded(string path)
	{
		if (EtceteraAndroidManager.albumChooserSucceededEvent != null)
		{
			if (File.Exists(path))
			{
				EtceteraAndroidManager.albumChooserSucceededEvent(path);
			}
			else if (EtceteraAndroidManager.albumChooserCancelledEvent != null)
			{
				EtceteraAndroidManager.albumChooserCancelledEvent();
			}
		}
	}

	public void photoChooserCancelled(string empty)
	{
		if (EtceteraAndroidManager.photoChooserCancelledEvent != null)
		{
			EtceteraAndroidManager.photoChooserCancelledEvent();
		}
	}

	public void photoChooserSucceeded(string path)
	{
		if (EtceteraAndroidManager.photoChooserSucceededEvent != null)
		{
			if (File.Exists(path))
			{
				EtceteraAndroidManager.photoChooserSucceededEvent(path);
			}
			else if (EtceteraAndroidManager.photoChooserCancelledEvent != null)
			{
				EtceteraAndroidManager.photoChooserCancelledEvent();
			}
		}
	}

	public void videoRecordingSucceeded(string path)
	{
		if (EtceteraAndroidManager.videoRecordingSucceededEvent != null)
		{
			EtceteraAndroidManager.videoRecordingSucceededEvent(path);
		}
	}

	public void videoRecordingCancelled(string empty)
	{
		if (EtceteraAndroidManager.videoRecordingCancelledEvent != null)
		{
			EtceteraAndroidManager.videoRecordingCancelledEvent();
		}
	}

	public void ttsInitialized(string result)
	{
		bool flag = result == "1";
		if (flag && EtceteraAndroidManager.ttsInitializedEvent != null)
		{
			EtceteraAndroidManager.ttsInitializedEvent();
		}
		if (!flag && EtceteraAndroidManager.ttsFailedToInitializeEvent != null)
		{
			EtceteraAndroidManager.ttsFailedToInitializeEvent();
		}
	}

	public void ttsUtteranceCompleted(string utteranceId)
	{
	}

	public void askForReviewWillOpenMarket(string empty)
	{
		if (EtceteraAndroidManager.askForReviewWillOpenMarketEvent != null)
		{
			EtceteraAndroidManager.askForReviewWillOpenMarketEvent();
		}
	}

	public void askForReviewRemindMeLater(string empty)
	{
		if (EtceteraAndroidManager.askForReviewRemindMeLaterEvent != null)
		{
			EtceteraAndroidManager.askForReviewRemindMeLaterEvent();
		}
	}

	public void askForReviewDontAskAgain(string empty)
	{
		if (EtceteraAndroidManager.askForReviewDontAskAgainEvent != null)
		{
			EtceteraAndroidManager.askForReviewDontAskAgainEvent();
		}
	}

	public void inlineWebViewJSCallback(string message)
	{
		EtceteraAndroidManager.inlineWebViewJSCallbackEvent.fire(message);
	}

	public void notificationReceived(string extraData)
	{
		EtceteraAndroidManager.notificationReceivedEvent.fire(extraData);
	}
}
