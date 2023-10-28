using System;
using UnityEngine;

namespace Prime31
{
	public class LifecycleHelper : MonoBehaviour
	{
		public event Action<bool> onApplicationPausedEvent;

		private void OnApplicationPause(bool paused)
		{
			if (this.onApplicationPausedEvent != null)
			{
				this.onApplicationPausedEvent(paused);
			}
		}
	}
}
