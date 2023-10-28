using System.Collections;
using UnityEngine;

public class KFFRequestorController : MonoBehaviour
{
	public enum PlacementStates
	{
		PlacementPending,
		PlacementStarted,
		PlacementFinished,
		PlacementFailed
	}

	public delegate void PurchaseEvent(UpsightPurchase purchase);

	private const float SHOW_PLACEMENT_TIMEOUT_SECS = 30f;

	public bool enablePlacements;

	public PurchaseEvent onPurchaseEvent;

	private float placementStartSecs;

	public static KFFRequestorController controller;

	private PlacementStates _PlacementState;

	private PlacementStates PlacementState
	{
		get
		{
			return _PlacementState;
		}
		set
		{
			if (_PlacementState != value)
			{
				_PlacementState = value;
			}
		}
	}

	public bool IsPlacementInProgress
	{
		get
		{
			return PlacementState == PlacementStates.PlacementStarted || PlacementState == PlacementStates.PlacementPending;
		}
	}

	public static KFFRequestorController GetInstance()
	{
		return controller;
	}

	private void PlacementPending()
	{
		placementStartSecs = Time.realtimeSinceStartup;
		PlacementState = PlacementStates.PlacementPending;
	}

	private void PlacementStarted()
	{
		PlacementState = PlacementStates.PlacementStarted;
	}

	private void PlacementFinished()
	{
		PlacementState = PlacementStates.PlacementFinished;
	}

	private void PlacementFailed()
	{
		PlacementState = PlacementStates.PlacementFailed;
	}

	private void Awake()
	{
		if (controller != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		controller = this;
		PlacementFinished();
		RegisterUpsightCallbacks();
	}

	private void OnDestroy()
	{
		DeregisterUpsightCallbacks();
	}

	public Coroutine ShowContentCoroutine(string placement, float waitSecs = 0f)
	{
		return StartCoroutine(ShowPlacement(placement, waitSecs));
	}

	public bool RequestContent(string name)
	{
		if (PlacementState == PlacementStates.PlacementStarted)
		{
			return false;
		}
		if (PlacementState == PlacementStates.PlacementPending)
		{
			if (placementStartSecs + 30f <= Time.realtimeSinceStartup)
			{
				return false;
			}
			PlacementFinished();
		}
		if (name != null && name != string.Empty)
		{
			PlacementPending();
			Upsight.sendContentRequest(name, true);
			return true;
		}
		return false;
	}

	private void RegisterUpsightCallbacks()
	{
		UpsightManager.makePurchaseEvent += makePurchase;
		UpsightManager.contentRequestLoadedEvent += contentRequestLoaded;
		UpsightManager.contentRequestFailedEvent += contentRequestFailed;
		UpsightManager.contentWillDisplayEvent += contentWillDisplay;
		UpsightManager.contentDismissedEvent += contentDismissed;
		UpsightManager.openRequestFailedEvent += openRequestFailed;
	}

	private void DeregisterUpsightCallbacks()
	{
		UpsightManager.makePurchaseEvent -= makePurchase;
		UpsightManager.contentRequestLoadedEvent -= contentRequestLoaded;
		UpsightManager.contentRequestFailedEvent -= contentRequestFailed;
		UpsightManager.contentWillDisplayEvent -= contentWillDisplay;
		UpsightManager.contentDismissedEvent -= contentDismissed;
		UpsightManager.openRequestFailedEvent -= openRequestFailed;
	}

	private IEnumerator ShowPlacement(string placement, float waitSecs)
	{
		RequestContent(placement);
		for (float currWaitSecs = 0f; currWaitSecs <= waitSecs; currWaitSecs += Time.unscaledDeltaTime)
		{
			if (!IsPlacementInProgress)
			{
				break;
			}
			yield return null;
		}
	}

	private void openRequestFailed(string error)
	{
		PlacementFailed();
	}

	private void contentRequestLoaded(string placementID)
	{
	}

	private void contentRequestFailed(string placementID, string error)
	{
		PlacementFailed();
	}

	private void contentWillDisplay(string placementID)
	{
		PlacementStarted();
	}

	private void contentDismissed(string placementID, string dismissType)
	{
		PlacementFinished();
	}

	private void makePurchase(UpsightPurchase purchase)
	{
		if (onPurchaseEvent != null)
		{
			onPurchaseEvent(purchase);
		}
	}
}
