using UnityEngine;

public class UpsightContentRequest : MonoBehaviour
{
	public string placementID;

	public bool showsOverlayImmediately = true;

	public bool shouldAnimate = true;

	private void Start()
	{
		Upsight.sendContentRequest(placementID, showsOverlayImmediately, shouldAnimate);
	}

	private void OnEnable()
	{
		UpsightManager.contentRequestLoadedEvent += contentRequestLoaded;
		UpsightManager.contentRequestFailedEvent += contentRequestFailed;
		UpsightManager.contentWillDisplayEvent += contentWillDisplay;
		UpsightManager.contentDismissedEvent += contentDismissed;
	}

	private void OnDisable()
	{
		UpsightManager.contentRequestLoadedEvent -= contentRequestLoaded;
		UpsightManager.contentRequestFailedEvent -= contentRequestFailed;
		UpsightManager.contentWillDisplayEvent -= contentWillDisplay;
		UpsightManager.contentDismissedEvent -= contentDismissed;
	}

	private void contentRequestLoaded(string placementID)
	{
	}

	private void contentRequestFailed(string placementID, string error)
	{
	}

	private void contentWillDisplay(string placementID)
	{
	}

	private void contentDismissed(string placementID, string dismissType)
	{
	}
}
