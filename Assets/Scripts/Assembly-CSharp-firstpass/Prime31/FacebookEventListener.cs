using System.Collections.Generic;
using UnityEngine;

namespace Prime31
{
	public class FacebookEventListener : MonoBehaviour
	{
		private void OnEnable()
		{
			FacebookManager.sessionOpenedEvent += sessionOpenedEvent;
			FacebookManager.loginFailedEvent += loginFailedEvent;
			FacebookManager.dialogCompletedWithUrlEvent += dialogCompletedWithUrlEvent;
			FacebookManager.dialogFailedEvent += dialogFailedEvent;
			FacebookManager.graphRequestCompletedEvent += graphRequestCompletedEvent;
			FacebookManager.graphRequestFailedEvent += facebookCustomRequestFailed;
			FacebookManager.facebookComposerCompletedEvent += facebookComposerCompletedEvent;
			FacebookManager.reauthorizationFailedEvent += reauthorizationFailedEvent;
			FacebookManager.reauthorizationSucceededEvent += reauthorizationSucceededEvent;
			FacebookManager.shareDialogFailedEvent += shareDialogFailedEvent;
			FacebookManager.shareDialogSucceededEvent += shareDialogSucceededEvent;
		}

		private void OnDisable()
		{
			FacebookManager.sessionOpenedEvent -= sessionOpenedEvent;
			FacebookManager.loginFailedEvent -= loginFailedEvent;
			FacebookManager.dialogCompletedWithUrlEvent -= dialogCompletedWithUrlEvent;
			FacebookManager.dialogFailedEvent -= dialogFailedEvent;
			FacebookManager.graphRequestCompletedEvent -= graphRequestCompletedEvent;
			FacebookManager.graphRequestFailedEvent -= facebookCustomRequestFailed;
			FacebookManager.facebookComposerCompletedEvent -= facebookComposerCompletedEvent;
			FacebookManager.reauthorizationFailedEvent -= reauthorizationFailedEvent;
			FacebookManager.reauthorizationSucceededEvent -= reauthorizationSucceededEvent;
			FacebookManager.shareDialogFailedEvent -= shareDialogFailedEvent;
			FacebookManager.shareDialogSucceededEvent -= shareDialogSucceededEvent;
		}

		private void sessionOpenedEvent()
		{
		}

		private void loginFailedEvent(P31Error error)
		{
		}

		private void dialogCompletedWithUrlEvent(string url)
		{
		}

		private void dialogFailedEvent(P31Error error)
		{
		}

		private void facebokDialogCompleted()
		{
		}

		private void graphRequestCompletedEvent(object obj)
		{
			Utils.logObject(obj);
		}

		private void facebookCustomRequestFailed(P31Error error)
		{
		}

		private void facebookComposerCompletedEvent(bool didSucceed)
		{
		}

		private void reauthorizationSucceededEvent()
		{
		}

		private void reauthorizationFailedEvent(P31Error error)
		{
		}

		private void shareDialogFailedEvent(P31Error error)
		{
		}

		private void shareDialogSucceededEvent(Dictionary<string, object> dict)
		{
			Utils.logObject(dict);
		}
	}
}
