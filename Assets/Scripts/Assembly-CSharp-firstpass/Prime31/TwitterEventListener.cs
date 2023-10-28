using UnityEngine;

namespace Prime31
{
	public class TwitterEventListener : MonoBehaviour
	{
		private void OnEnable()
		{
			TwitterManager.twitterInitializedEvent += twitterInitializedEvent;
			TwitterManager.loginSucceededEvent += loginSucceeded;
			TwitterManager.loginFailedEvent += loginFailed;
			TwitterManager.requestDidFinishEvent += requestDidFinishEvent;
			TwitterManager.requestDidFailEvent += requestDidFailEvent;
			TwitterManager.tweetSheetCompletedEvent += tweetSheetCompletedEvent;
		}

		private void OnDisable()
		{
			TwitterManager.twitterInitializedEvent -= twitterInitializedEvent;
			TwitterManager.loginSucceededEvent -= loginSucceeded;
			TwitterManager.loginFailedEvent -= loginFailed;
			TwitterManager.requestDidFinishEvent -= requestDidFinishEvent;
			TwitterManager.requestDidFailEvent -= requestDidFailEvent;
			TwitterManager.tweetSheetCompletedEvent -= tweetSheetCompletedEvent;
		}

		private void twitterInitializedEvent()
		{
		}

		private void loginSucceeded(string username)
		{
		}

		private void loginFailed(string error)
		{
		}

		private void requestDidFailEvent(string error)
		{
		}

		private void requestDidFinishEvent(object result)
		{
			if (result != null)
			{
				Utils.logObject(result);
			}
		}

		private void tweetSheetCompletedEvent(bool didSucceed)
		{
		}
	}
}
