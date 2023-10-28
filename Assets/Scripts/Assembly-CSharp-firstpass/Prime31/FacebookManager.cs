using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Prime31
{
	public class FacebookManager : AbstractManager
	{
		[method: MethodImpl(32)]
		public static event Action sessionOpenedEvent;

		[method: MethodImpl(32)]
		public static event Action preLoginSucceededEvent;

		[method: MethodImpl(32)]
		public static event Action<P31Error> loginFailedEvent;

		[method: MethodImpl(32)]
		public static event Action<string> dialogCompletedWithUrlEvent;

		[method: MethodImpl(32)]
		public static event Action<P31Error> dialogFailedEvent;

		[method: MethodImpl(32)]
		public static event Action<object> graphRequestCompletedEvent;

		[method: MethodImpl(32)]
		public static event Action<P31Error> graphRequestFailedEvent;

		[method: MethodImpl(32)]
		public static event Action<bool> facebookComposerCompletedEvent;

		[method: MethodImpl(32)]
		public static event Action reauthorizationSucceededEvent;

		[method: MethodImpl(32)]
		public static event Action<P31Error> reauthorizationFailedEvent;

		[method: MethodImpl(32)]
		public static event Action<Dictionary<string, object>> shareDialogSucceededEvent;

		[method: MethodImpl(32)]
		public static event Action<P31Error> shareDialogFailedEvent;

		static FacebookManager()
		{
			AbstractManager.initialize(typeof(FacebookManager));
		}

		public void sessionOpened(string accessToken)
		{
			FacebookManager.preLoginSucceededEvent.fire();
			Facebook.instance.accessToken = accessToken;
			FacebookManager.sessionOpenedEvent.fire();
		}

		public void loginFailed(string json)
		{
			FacebookManager.loginFailedEvent.fire(P31Error.errorFromJson(json));
		}

		public void dialogCompletedWithUrl(string url)
		{
			FacebookManager.dialogCompletedWithUrlEvent.fire(url);
		}

		public void dialogFailedWithError(string json)
		{
			FacebookManager.dialogFailedEvent.fire(P31Error.errorFromJson(json));
		}

		public void graphRequestCompleted(string json)
		{
			if (FacebookManager.graphRequestCompletedEvent != null)
			{
				object param = Json.decode(json);
				FacebookManager.graphRequestCompletedEvent.fire(param);
			}
		}

		public void graphRequestFailed(string json)
		{
			FacebookManager.graphRequestFailedEvent.fire(P31Error.errorFromJson(json));
		}

		public void facebookComposerCompleted(string result)
		{
			FacebookManager.facebookComposerCompletedEvent.fire(result == "1");
		}

		public void reauthorizationSucceeded(string empty)
		{
			FacebookManager.reauthorizationSucceededEvent.fire();
		}

		public void reauthorizationFailed(string json)
		{
			FacebookManager.reauthorizationFailedEvent.fire(P31Error.errorFromJson(json));
		}

		public void shareDialogFailed(string json)
		{
			FacebookManager.shareDialogFailedEvent.fire(P31Error.errorFromJson(json));
		}

		public void shareDialogSucceeded(string json)
		{
			FacebookManager.shareDialogSucceededEvent.fire(json.dictionaryFromJson());
		}
	}
}
