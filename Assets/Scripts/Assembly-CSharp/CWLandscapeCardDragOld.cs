using System.Collections;
using UnityEngine;

public class CWLandscapeCardDragOld : MonoBehaviour
{
	private LandscapeManagerScript landscapeMgr;

	public int Index;

	public Camera uiCamera;

	public AudioClip SummonSound;

	public AudioClip CardMove;

	public AudioClip[] PlaceCardSounds;

	public bool FollowFinger;

	public bool Return;

	public string currentTypeName;

	private GameState GameInstance;

	private Vector3 StartPosition;

	private AudioManager audioMgr;

	private Transform cardRoot;

	private Vector3 cardRootPos;

	private Vector3 cardRootScale;

	private Transform cardRootParent;

	private int cardRootLayer;

	private Plane tablePlane = new Plane(Vector3.up, Vector3.zero);

	private Camera GameCamera;

	public LandscapeType CurrentType { get; set; }

	private void Start()
	{
		GameCamera = Camera.main;
		cardRoot = base.transform.Find("card");
		if (cardRoot != null)
		{
			cardRootPos = cardRoot.localPosition;
			cardRootScale = cardRoot.localScale;
			cardRootParent = cardRoot.parent;
			cardRootLayer = cardRoot.gameObject.layer;
		}
		StartPosition = base.transform.localPosition;
		landscapeMgr = LandscapeManagerScript.GetInstance();
		GameInstance = GameState.Instance;
		audioMgr = AudioManager.GetInstance();
	}

	private void OnPress(bool pressed)
	{
		if (Time.timeScale != 0f && !UICamera.useInputEnabler)
		{
			if (pressed && landscapeMgr.enableDragLandscapeCard && !FollowFinger)
			{
				FollowFinger = true;
				SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), CardMove);
				landscapeMgr.enableDragLandscapeCard = false;
			}
			if (!pressed && FollowFinger)
			{
				FollowFinger = false;
				PlaceLandscape();
				landscapeMgr.enableDragLandscapeCard = true;
			}
		}
	}

	private void ReturnCard()
	{
		base.transform.localPosition = StartPosition;
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(audioMgr.GetComponent<AudioSource>(), CardMove, true, false, SLOTAudioManager.AudioType.SFX);
		RestoreCardRoot();
	}

	private void PlaceLandscape()
	{
		FollowFinger = false;
		if ((double)Input.mousePosition.y < (double)Screen.height * 0.5)
		{
			ReturnCard();
			return;
		}
		float[] array = new float[4] { 0.25f, 0.5f, 0.75f, 1f };
		base.transform.localPosition = new Vector3(StartPosition.x, StartPosition.y - 525f, StartPosition.z);
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(audioMgr.GetComponent<AudioSource>(), SummonSound, true, false, SLOTAudioManager.AudioType.SFX);
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(audioMgr.GetComponent<AudioSource>(), PlaceCardSounds[KFFRandom.GetRandomIndex(PlaceCardSounds.Length)], true, false, SLOTAudioManager.AudioType.SFX);
		bool flag = false;
		for (int i = 0; i < 4; i++)
		{
			if (!(Input.mousePosition.x < (float)Screen.width * array[i]))
			{
				continue;
			}
			if (GameInstance.GetLandscapeType(PlayerType.User, i) == LandscapeType.None)
			{
				GameInstance.SetLandscape(PlayerType.User, i, CurrentType);
				landscapeMgr.UpdateLandscapes();
				landscapeMgr.populatedLandscapeCount++;
				if (landscapeMgr.populatedLandscapeCount == 4)
				{
					landscapeMgr.ReadyButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
					TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.StartBottleSpin);
				}
				flag = true;
			}
			else
			{
				base.transform.localPosition = new Vector3(StartPosition.x, StartPosition.y, StartPosition.z);
			}
			break;
		}
		if (flag)
		{
			StartCoroutine(PlayFadingTweens());
		}
		else
		{
			ReturnCard();
		}
	}

	private IEnumerator PlayFadingTweens()
	{
		TweenAlpha tweenAlpha = cardRoot.GetComponent<TweenAlpha>();
		tweenAlpha.Play(true);
		TweenScale tweenScale = cardRoot.GetComponent<TweenScale>();
		tweenScale.Play(true);
		yield return new WaitForSeconds(tweenAlpha.duration);
		RestoreCardRoot();
	}

	private IEnumerator AutoClickReadyButton()
	{
		yield return new WaitForSeconds(0.5f);
		landscapeMgr.ReadyButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
	}

	private void Update()
	{
		if (!FollowFinger)
		{
			return;
		}
		if (UICamera.useInputEnabler)
		{
			ReturnCard();
			FollowFinger = false;
		}
		else
		{
			if (!(cardRoot != null))
			{
				return;
			}
			bool flag = true;
			Vector3 worldPos;
			if (raycast(Input.mousePosition, out worldPos))
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
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				RestoreCardRoot();
				if (ProjectScreenPointOnDragPlane(base.transform.position, Input.mousePosition, out worldPos))
				{
					cardRoot.position = worldPos;
				}
			}
		}
	}

	private void RestoreCardRoot()
	{
		cardRoot.parent = cardRootParent;
		NGUITools.SetLayer(cardRoot.gameObject, cardRootLayer);
		cardRoot.gameObject.BroadcastMessage("ParentHasChanged", SendMessageOptions.DontRequireReceiver);
		cardRoot.localPosition = cardRootPos;
		cardRoot.localScale = cardRootScale;
		cardRoot.localRotation = cardRootParent.localRotation;
		UIPanel uIPanel = cardRoot.gameObject.GetComponent(typeof(UIPanel)) as UIPanel;
		if (uIPanel != null)
		{
			Object.Destroy(uIPanel);
		}
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

	private bool ProjectScreenPointOnDragPlane(Vector3 refPos, Vector2 screenPos, out Vector3 worldPos)
	{
		worldPos = refPos;
		Camera camera = uiCamera;
		Transform transform = camera.transform;
		Plane plane = new Plane(-transform.forward, refPos);
		Ray ray = camera.ScreenPointToRay(screenPos);
		float enter = 0f;
		if (!plane.Raycast(ray, out enter))
		{
			return false;
		}
		worldPos = ray.GetPoint(enter);
		return true;
	}
}
