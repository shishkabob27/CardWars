using UnityEngine;

public class FCDemoMarketing : MonoBehaviour
{
	public UITweener vfxDemoUnplayed;

	private bool complete;

	private void Update()
	{
		if (complete)
		{
			return;
		}
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (!instance.IsReady())
		{
			return;
		}
		if (vfxDemoUnplayed != null && instance.HasCompletedFCDemo())
		{
			UITweener[] components = vfxDemoUnplayed.gameObject.GetComponents<UITweener>();
			UITweener[] array = components;
			foreach (UITweener uITweener in array)
			{
				vfxDemoUnplayed.Reset();
				vfxDemoUnplayed.enabled = false;
			}
			vfxDemoUnplayed = null;
		}
		if (instance.HasPurchasedFC())
		{
			base.gameObject.SetActive(false);
			complete = true;
		}
	}
}
