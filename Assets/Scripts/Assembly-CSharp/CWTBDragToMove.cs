using System.Collections.Generic;
using UnityEngine;

public class CWTBDragToMove : MonoBehaviour
{
	public enum DragPlaneType
	{
		Camera,
		UseCollider
	}

	public Collider DragPlaneCollider;

	public float DragPlaneOffset;

	public Camera RaycastCamera;

	public Camera GameCamera;

	public GameObject cardTray;

	private CWTBTapToBringForward tapScript;

	private GameState GameInstance;

	private PanelManagerBattle panelMgrBattle;

	private Collider currentCollider;

	private DragRecognizer dragRecognizer;

	private bool dragging;

	private FingerGestures.Finger draggingFinger;

	private GestureRecognizer gestureRecognizer;

	private bool oldUseGravity;

	private bool oldIsKinematic;

	public Transform originalPos;

	private Transform cardRoot;

	private Vector3 cardRootPos;

	private Vector3 cardRootScale;

	private Quaternion cardRootRot;

	private Transform cardRootParent;

	private int cardRootLayer;

	private bool cardRootDragging;

	private Plane tablePlane = new Plane(Vector3.up, Vector3.zero);

	private Vector3 dragOffset;

	private CWPlayerHandsController handCtlr;

	private bool dragMode;

	private bool selectMode;

	private int prevLane;

	public bool Dragging
	{
		get
		{
			return dragging;
		}
		private set
		{
			if (dragging == value)
			{
				return;
			}
			dragging = value;
			if ((bool)GetComponent<Rigidbody>())
			{
				if (dragging)
				{
					oldUseGravity = GetComponent<Rigidbody>().useGravity;
					oldIsKinematic = GetComponent<Rigidbody>().isKinematic;
					GetComponent<Rigidbody>().useGravity = false;
					GetComponent<Rigidbody>().isKinematic = true;
				}
				else
				{
					GetComponent<Rigidbody>().isKinematic = oldIsKinematic;
					GetComponent<Rigidbody>().useGravity = oldUseGravity;
					GetComponent<Rigidbody>().velocity = Vector3.zero;
				}
			}
		}
	}

	private void Start()
	{
		cardRoot = base.transform.Find("card");
		if (cardRoot != null)
		{
			cardRootPos = cardRoot.localPosition;
			cardRootScale = cardRoot.localScale;
			cardRootRot = cardRoot.localRotation;
			cardRootParent = cardRoot.parent;
			cardRootLayer = cardRoot.gameObject.layer;
		}
		if (!RaycastCamera)
		{
			RaycastCamera = Camera.main;
		}
		handCtlr = CWPlayerHandsController.GetInstance();
		tapScript = GetComponent<CWTBTapToBringForward>();
		GameInstance = GameState.Instance;
		panelMgrBattle = PanelManagerBattle.GetInstance();
		dragRecognizer = handCtlr.gameObject.GetComponent(typeof(DragRecognizer)) as DragRecognizer;
	}

	public bool ProjectScreenPointOnDragPlane(Vector3 refPos, Vector2 screenPos, out Vector3 worldPos)
	{
		worldPos = refPos;
		if ((bool)DragPlaneCollider)
		{
			Ray ray = RaycastCamera.ScreenPointToRay(screenPos);
			RaycastHit hitInfo;
			if (!DragPlaneCollider.Raycast(ray, out hitInfo, float.MaxValue))
			{
				return false;
			}
			worldPos = hitInfo.point + DragPlaneOffset * hitInfo.normal;
		}
		else
		{
			Transform transform = RaycastCamera.transform;
			Plane plane = new Plane(-transform.forward, refPos);
			Ray ray2 = RaycastCamera.ScreenPointToRay(screenPos);
			float enter = 0f;
			if (!plane.Raycast(ray2, out enter))
			{
				return false;
			}
			worldPos = ray2.GetPoint(enter);
		}
		return true;
	}

	private void HandleDrag(DragGesture gesture)
	{
		if (!base.enabled)
		{
			return;
		}
		if (gesture.Phase == ContinuousGesturePhase.Started)
		{
			Dragging = true;
			draggingFinger = gesture.Fingers[0];
		}
		else
		{
			if (!Dragging || gesture.Fingers[0] != draggingFinger)
			{
				return;
			}
			if (gesture.Phase == ContinuousGesturePhase.Updated)
			{
				if (!dragMode && !selectMode)
				{
					if (Mathf.Abs(gesture.DeltaMove.y) < Mathf.Abs(gesture.DeltaMove.x))
					{
						dragMode = false;
						selectMode = true;
					}
					else if (Mathf.Abs(gesture.DeltaMove.y) > Mathf.Abs(gesture.DeltaMove.x))
					{
						BeginDrag(gesture.Position);
					}
				}
				if (dragMode)
				{
					DragMode(gesture);
				}
				if (!selectMode)
				{
				}
			}
			else
			{
				Dragging = false;
				dragMode = false;
				selectMode = false;
				ReleaseCard(gesture.Position);
			}
		}
	}

	public void BeginDrag(Vector2 fingerpos)
	{
		float num = 1f;
		if (base.transform.localScale.x != 0f)
		{
			num = originalPos.localScale.x / base.transform.localScale.x;
		}
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), handCtlr.cardDragSound);
		panelMgrBattle.currentCardObj = base.gameObject;
		handCtlr.card = tapScript.card;
		handCtlr.cardName = tapScript.card.Form.Name;
		tapScript.ApplySorting();
		PanelManagerBattle.GetInstance().battleLaneColController.P1FlippedCollidersOn();
		dragMode = true;
		selectMode = false;
		dragOffset = Vector3.zero;
		Vector3 position = new Vector3(fingerpos.x, fingerpos.y, RaycastCamera.nearClipPlane);
		position = RaycastCamera.ScreenToWorldPoint(position);
		dragOffset += (base.transform.position - position) * num;
		dragOffset.z = 0f;
	}

	private void RestoreCardRoot()
	{
		cardRoot.parent = cardRootParent;
		NGUITools.SetLayer(cardRoot.gameObject, cardRootLayer);
		cardRoot.gameObject.BroadcastMessage("ParentHasChanged", SendMessageOptions.DontRequireReceiver);
		cardRoot.localPosition = cardRootPos;
		cardRoot.localScale = cardRootScale;
		cardRoot.localRotation = cardRootRot;
		UIPanel uIPanel = cardRoot.gameObject.GetComponent(typeof(UIPanel)) as UIPanel;
		if (uIPanel != null)
		{
			Object.Destroy(uIPanel);
		}
	}

	public void ReleaseCard(Vector2 gesturePos)
	{
		cardRootDragging = false;
		if (cardRoot != null)
		{
			RestoreCardRoot();
		}
		BoxCollider[] laneColliders = handCtlr.laneColliders;
		foreach (BoxCollider boxCollider in laneColliders)
		{
			iTween.Stop(boxCollider.gameObject);
		}
		Transform transform = base.transform;
		int num = GetActiveLane(gesturePos);
		if (num < 0 && tapScript.card.Form.Type == CardType.Spell && IsAboveCardTray(gesturePos.y))
		{
			num = 0;
		}
		if (num == -1)
		{
			transform.position = originalPos.position;
			transform.localScale = originalPos.localScale;
			tapScript.Select();
			tapScript.ApplySorting();
		}
		else if (tapScript.card.Form.CanPlay(PlayerType.User, num) && handCtlr.CanPlay())
		{
			handCtlr.lane = num;
			handCtlr.PlayCard(num, tapScript.card);
			if (tapScript.card.Form.Type != CardType.Spell)
			{
				UITexture[] componentsInChildren = base.gameObject.GetComponentsInChildren<UITexture>();
				UITexture[] array = componentsInChildren;
				foreach (UITexture uITexture in array)
				{
					switch (uITexture.name)
					{
					case "Card_Art":
						CWPlayerHandsController.GetInstance().RegisterDynamicTexture(uITexture);
						break;
					}
				}
			}
		}
		else
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), handCtlr.errorSound);
			transform.position = originalPos.position;
			tapScript.ApplySorting();
			if (tapScript.card.Form.Faction == Faction.Universal || GameInstance.IsLaneOfLandscapeType(PlayerType.User, num, (LandscapeType)tapScript.card.Form.Faction))
			{
				int num2 = tapScript.card.Form.DetermineCost(PlayerType.User);
				int magicPoints = GameInstance.GetMagicPoints(PlayerType.User);
				if (magicPoints < num2)
				{
					TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.NotEnoughActionPoints);
				}
			}
			else
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.IncorrectLandscape);
			}
		}
		handCtlr.ResetLaneColor();
		PanelManagerBattle.GetInstance().battleLaneColController.P1CollidersOn();
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), handCtlr.cardPlaceSounds[KFFRandom.GetRandomIndex(handCtlr.cardPlaceSounds.Length)]);
	}

	public void DragMode(Vector2 gesturePos, FingerGestures.Finger finger)
	{
		DragMode(GetActiveLane(gesturePos), finger);
	}

	private void DragMode(DragGesture gesture)
	{
		DragMode(GetActiveLane(gesture.Position), draggingFinger);
	}

	private bool IsAboveCardTray(float y)
	{
		if (cardTray == null)
		{
			return true;
		}
		Bounds uIBoundsRecursive = SLOTGame.GetUIBoundsRecursive(cardTray.transform);
		Vector3 center = uIBoundsRecursive.center;
		center.y += uIBoundsRecursive.extents.y;
		return y > RaycastCamera.WorldToScreenPoint(center).y;
	}

	private void DragMode(int lane, FingerGestures.Finger finger)
	{
		Transform transform = base.transform;
		transform.localScale = Vector3.one;
		if (cardRoot != null)
		{
			bool flag = true;
			Vector3 worldPos;
			if ((lane >= 0 || cardRootDragging) && raycast(finger.Position, out worldPos))
			{
				flag = false;
				if (IsAboveCardTray(finger.Position.y))
				{
					cardRootDragging = true;
					if (tapScript.card.Form.Type != CardType.Spell)
					{
						cardRoot.parent = null;
						if (cardRoot.gameObject.layer != GameCamera.gameObject.layer)
						{
							NGUITools.SetLayer(cardRoot.gameObject, GameCamera.gameObject.layer);
						}
						cardRoot.gameObject.BroadcastMessage("ParentHasChanged", SendMessageOptions.DontRequireReceiver);
						cardRoot.position = worldPos;
						cardRoot.localScale = new Vector3(0.02f, 0.02f, 0.02f);
						cardRoot.localEulerAngles = new Vector3(90f, 90f, 0f);
						UIPanel uIPanel = cardRoot.gameObject.GetComponent(typeof(UIPanel)) as UIPanel;
						if (uIPanel != null)
						{
							uIPanel.sortByDepth = false;
						}
						base.transform.position = originalPos.position;
						base.transform.localScale = originalPos.localScale;
						tapScript.ApplySorting();
						flag = true;
					}
				}
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				RestoreCardRoot();
				if (ProjectScreenPointOnDragPlane(transform.position, finger.Position, out worldPos))
				{
					transform.position = worldPos + dragOffset;
				}
			}
		}
		SetActiveLaneColor(lane);
	}

	private bool raycast(Vector3 fingerPos, out Vector3 worldPos)
	{
		worldPos = Vector3.zero;
		Ray ray = GameCamera.ScreenPointToRay(fingerPos);
		float enter = 0f;
		if (!tablePlane.Raycast(ray, out enter))
		{
			return false;
		}
		worldPos = ray.GetPoint(enter);
		return true;
	}

	private void SetActiveLaneColor(Vector2 gesturePos)
	{
		SetActiveLaneColor(GetActiveLane(gesturePos));
	}

	private void SetActiveLaneColor(int lane)
	{
		if (prevLane == lane)
		{
			return;
		}
		for (int i = 0; i < handCtlr.laneColliders.Length; i++)
		{
			iTween.Stop(handCtlr.laneColliders[i].gameObject);
			bool flag = tapScript.card.Form.CanPlay(PlayerType.User, i) && handCtlr.CanPlay();
			if (tapScript.card.Form.Type == CardType.Spell)
			{
				if (flag)
				{
					SetActiveLaneColorSingle(true, i);
				}
				else
				{
					SetActiveLaneColorSingle(false, i);
				}
			}
			else if (lane == i)
			{
				if (flag)
				{
					SetActiveLaneColorSingle(true, i);
				}
				else
				{
					SetActiveLaneColorSingle(false, i);
				}
			}
			else
			{
				Material material = handCtlr.laneColliders[i].gameObject.GetComponent<Renderer>().material;
				material.color = new Color(0f, 0f, 0f, 0f);
			}
		}
		prevLane = lane;
	}

	private void SetActiveLaneColorSingle(bool enable, int lane)
	{
		Material material = handCtlr.laneColliders[lane].gameObject.GetComponent<Renderer>().material;
		Color color = new Color(1f, 1f, 1f);
		color = (material.color = ((tapScript.card.Form.Faction == Faction.Universal) ? ((!enable) ? Color.red : FactionManager.Instance.GetFactionData(GameInstance.GetLandscapeType(PlayerType.User, lane)).FactionColor) : ((!enable) ? Color.red : FactionManager.Instance.GetFactionData(tapScript.card.Form.Faction).FactionColor)));
		material.SetColor("_Emission", color);
		if (enable)
		{
			SpawnHighlightFX(lane);
			TriggerMatFadePingpong(true, "ToLandscapeHighlight", lane);
		}
		else
		{
			TriggerMatFadePingpong(true, "ToLandscapeHighlight", lane);
		}
	}

	private void TriggerMatFadePingpong(bool enable, string tweenName, int lane)
	{
		if (tweenName == string.Empty)
		{
			return;
		}
		iTweenEvent @event = iTweenEvent.GetEvent(handCtlr.laneColliders[lane].gameObject, tweenName);
		if (@event != null)
		{
			if (enable)
			{
				@event.Play();
			}
			else
			{
				@event.Stop();
			}
		}
	}

	private void SpawnHighlightFX(int lane)
	{
		string highlightFxName = GetHighlightFxName(lane);
		GameObject target = handCtlr.laneColliders[lane].gameObject;
		handCtlr.SpawnHighlightFX(highlightFxName, target);
	}

	private void SpawnInvalidFX(int lane)
	{
		string invalidFxName = GetInvalidFxName(lane);
		GameObject target = handCtlr.laneColliders[lane].gameObject;
		handCtlr.SpawnHighlightFX(invalidFxName, target);
	}

	private int GetActiveLane(Vector2 gesturePos)
	{
		int result = -1;
		if (IsAboveCardTray(gesturePos.y))
		{
			for (int i = 0; i < handCtlr.laneColliders.Length; i++)
			{
				BoxCollider col = handCtlr.laneColliders[i];
				if (checkReleasePoint(gesturePos, col))
				{
					result = i;
				}
			}
		}
		return result;
	}

	private bool checkReleasePoint(Vector2 gesturePos, BoxCollider col)
	{
		bool flag = col.enabled;
		col.enabled = true;
		Vector3 position = new Vector3(col.bounds.max.x, 0f, col.bounds.max.z);
		Vector3 position2 = new Vector3(col.bounds.max.x, 0f, col.bounds.min.z);
		Vector3 position3 = new Vector3(col.bounds.min.x, 0f, col.bounds.max.z);
		Vector3 position4 = new Vector3(col.bounds.min.x, 0f, col.bounds.min.z);
		Vector3 vector = Vector3.Lerp(GameCamera.WorldToScreenPoint(position2), GameCamera.WorldToScreenPoint(position4), 0.5f);
		Vector3 vector2 = Vector3.Lerp(GameCamera.WorldToScreenPoint(position), GameCamera.WorldToScreenPoint(position3), 0.5f);
		Vector3 vector3 = GameCamera.WorldToScreenPoint(col.bounds.min);
		col.enabled = flag;
		if (gesturePos.x > vector.x || gesturePos.x < vector2.x)
		{
			return false;
		}
		if (gesturePos.y < vector3.y)
		{
			return false;
		}
		return true;
	}

	private void OnDrag(DragGesture gesture)
	{
		if (gesture != null && gesture.Hit.collider != currentCollider)
		{
			Collider collider = currentCollider;
			currentCollider = gesture.Hit.collider;
			if (currentCollider != null)
			{
				if (dragRecognizer != null && collider != null && gesture.State != GestureRecognitionState.Started)
				{
					gesture.State = GestureRecognitionState.Ended;
					dragRecognizer.Update();
				}
				CWTBTapToBringForward cWTBTapToBringForward = currentCollider.gameObject.GetComponent(typeof(CWTBTapToBringForward)) as CWTBTapToBringForward;
				if (cWTBTapToBringForward != null)
				{
					cWTBTapToBringForward.Select();
				}
			}
		}
		HandleDrag(gesture);
	}

	private void OnDisable()
	{
		if (Dragging)
		{
			Dragging = false;
		}
	}

	private void ResizeWhileDragging(DragGesture gesture)
	{
	}

	private string GetHighlightFxName(int lane)
	{
		LandscapeType landscapeType = GameInstance.GetLandscapeType(PlayerType.User, lane);
		return FactionManager.Instance.GetFactionData(landscapeType).HighlightFX_Lane[lane];
	}

	private string GetInvalidFxName(int lane)
	{
		string result = string.Empty;
		Dictionary<string, object>[] array = SQUtils.ReadJSONData("db_Faction.json");
		LandscapeType landscapeType = GameInstance.GetLandscapeType(PlayerType.User, lane);
		Dictionary<string, object>[] array2 = array;
		foreach (Dictionary<string, object> dictionary in array2)
		{
			if ((string)dictionary["FactionID"] == landscapeType.ToString())
			{
				string key = "InvalidFX_Lane" + (lane + 1);
				result = (string)dictionary[key];
			}
		}
		return result;
	}

	private Faction GetFactionFromLandscape(LandscapeType land)
	{
		switch (land)
		{
		case LandscapeType.Corn:
			return Faction.Corn;
		case LandscapeType.Cotton:
			return Faction.Cotton;
		case LandscapeType.Sand:
			return Faction.Sand;
		case LandscapeType.Swamp:
			return Faction.Swamp;
		case LandscapeType.Plains:
			return Faction.Plains;
		default:
			return Faction.Universal;
		}
	}

	public void CancelDrag()
	{
		cardRootDragging = false;
		if (cardRoot != null)
		{
			RestoreCardRoot();
		}
		BoxCollider[] laneColliders = handCtlr.laneColliders;
		foreach (BoxCollider boxCollider in laneColliders)
		{
			iTween.Stop(boxCollider.gameObject);
		}
		Transform transform = base.transform;
		transform.position = originalPos.position;
		tapScript.ApplySorting();
	}
}
