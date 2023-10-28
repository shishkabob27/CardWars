using System.Collections;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Message Delayed")]
public class UIButtonMessageDelayed : UIButtonMessage
{
	public float delaySecs;

	protected override void Send()
	{
		StartCoroutine(DelayedSendCoroutine(delaySecs));
	}

	private IEnumerator DelayedSendCoroutine(float secs)
	{
		if (secs > 0f)
		{
			yield return new WaitForSeconds(secs);
		}
		base.Send();
	}
}
