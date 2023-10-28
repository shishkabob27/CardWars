using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using Multiplayer;
using UnityEngine;

public class PlayQuestButton : AsyncData<string>
{
	public GameObject heartTweenDoneEvents;

	public Collider[] collidersToDisable;

	public GameObject heart;

	public string heartTweenBeginTarget;

	public float heartTweenBeginTargetXOffset;

	public float heartTweenBeginTargetYOffset;

	public Vector3 heartTweenEndPosOffset = new Vector3(52f, 9f, -100f);

	public AudioClip heartBeginSFX;

	public AudioClip heartEndSFX;

	public GameObject heartVFXPrefab;

	public float heartSpawnDelay = 0.25f;

	public UITweener blinkTween;

	public UIButtonTween FailedToGetDeck;

	public UIButtonTween LoadingActivityShow;

	public UIButtonTween LoadingActivityHide;

	public RefreshMatch RefreshMatchScript;

	private CWQuestLoad cwQuestLoad;

	private GameObject heartTweenBeginTargetObj;

	private Camera heartTweenBeginTargetCam;

	private Camera thisCam;

	private bool wasUsingInputEnabler;

	private Vector3 heartPos;

	private GameObject[] heartInstances;

	private TweenPosition[] heartTweeners;

	private int doneIndex;

	private void Awake()
	{
		if (heartTweenDoneEvents != null)
		{
			cwQuestLoad = heartTweenDoneEvents.GetComponent(typeof(CWQuestLoad)) as CWQuestLoad;
		}
		if (!string.IsNullOrEmpty(heartTweenBeginTarget))
		{
			heartTweenBeginTargetObj = GameObject.Find(heartTweenBeginTarget);
			if (heartTweenBeginTargetObj != null)
			{
				heartTweenBeginTargetCam = NGUITools.FindCameraForLayer(heartTweenBeginTargetObj.layer);
			}
		}
		thisCam = NGUITools.FindCameraForLayer(base.gameObject.layer);
		if (heart != null)
		{
			heart.SetActive(false);
		}
	}

	public void StringCallback(string data, ResponseFlag flag)
	{
		Asyncdata.Set(flag, data);
	}

	private void Update()
	{
		if (Asyncdata.processed)
		{
			return;
		}
		Asyncdata.processed = true;
		if ((bool)LoadingActivityHide)
		{
			LoadingActivityHide.Play(true);
		}
		if (Asyncdata.MP_Data != null)
		{
			CWMPMapController.MPData mLastMPData = CWMPMapController.GetInstance().mLastMPData;
			Dictionary<string, object> dictionary = JsonReader.Deserialize<Dictionary<string, object>>(Asyncdata.MP_Data);
			if (dictionary != null)
			{
				mLastMPData.mCards = (string[])dictionary["cards"];
				HeartAnimation();
			}
		}
		else if ((bool)FailedToGetDeck)
		{
			FailedToGetDeck.Play(true);
		}
	}

	private void OnClick()
	{
		UIButtonSound component = GetComponent<UIButtonSound>();
		if (cwQuestLoad != null && !cwQuestLoad.CanStartQuest())
		{
			component.audioClip = cwQuestLoad.errorSound;
			return;
		}
		if (GlobalFlags.Instance.InMPMode && Asyncdata.processed)
		{
			CWMPMapController.MPData mLastMPData = CWMPMapController.GetInstance().mLastMPData;
			int num = 0;
			string leader = "Leader_Jake";
			if (CWDeckController.GetInstance() != null)
			{
				int currentMPDeck = CWDeckController.GetInstance().currentMPDeck;
				Deck deck = PlayerInfoScript.GetInstance().DeckManager.Decks[currentMPDeck];
				leader = deck.Leader.Form.ID;
				num = deck.Leader.Rank;
			}
			if ((bool)LoadingActivityShow)
			{
				LoadingActivityShow.Play(true);
			}
			if ((bool)RefreshMatchScript)
			{
				RefreshMatchScript.StopAllCoroutines();
			}
			global::Multiplayer.Multiplayer.MatchGetDeck(SessionManager.GetInstance().theSession, mLastMPData.mMatchID, num, leader, num, StringCallback);
		}
		if (!GlobalFlags.Instance.InMPMode)
		{
			HeartAnimation();
		}
		component.audioClip = cwQuestLoad.okSound;
		GlobalFlags.Instance.enableMapDrag = true;
	}

	private void HeartAnimation()
	{
		Vector3 position = Vector3.zero;
		if (heartTweenBeginTargetObj != null && heartTweenBeginTargetCam != null && thisCam != null)
		{
			position = heartTweenBeginTargetObj.transform.position;
			position = heartTweenBeginTargetCam.WorldToScreenPoint(position);
			position = thisCam.ScreenToWorldPoint(position);
		}
		if (heart != null)
		{
			doneIndex = 0;
			TweenPosition tweenPosition = heart.GetComponent(typeof(TweenPosition)) as TweenPosition;
			if (tweenPosition != null)
			{
				Vector3 position2 = (heartPos = base.transform.position);
				position2 = tweenPosition.transform.parent.InverseTransformPoint(position2);
				position2 += heartTweenEndPosOffset;
				tweenPosition.to = position2;
				position = tweenPosition.transform.parent.InverseTransformPoint(position);
				position.z = position2.z;
				tweenPosition.from = position;
			}
			heart.SetActive(true);
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			QuestData currentQuest = instance.GetCurrentQuest();
			int num = currentQuest.StaminaCost;
			if (GlobalFlags.Instance.InMPMode)
			{
				num = RankManager.Instance.FindRank(instance.DeckManager.GetHighestLeaderRank()).PVPParticipationCost;
			}
			if (num > 0)
			{
				heartInstances = new GameObject[num];
				heartTweeners = new TweenPosition[num];
				for (int i = 0; i < num; i++)
				{
					GameObject gameObject = SLOTGame.InstantiateFX(heart) as GameObject;
					if (!(gameObject != null))
					{
						continue;
					}
					gameObject.transform.parent = heart.transform.parent;
					gameObject.transform.localPosition = heart.transform.localPosition;
					gameObject.transform.localRotation = heart.transform.localRotation;
					gameObject.transform.localScale = heart.transform.localScale;
					gameObject.layer = heart.layer;
					gameObject.SetActive(false);
					heartInstances[i] = gameObject;
					TweenPosition tweenPosition2 = gameObject.GetComponent(typeof(TweenPosition)) as TweenPosition;
					if (tweenPosition2 != null)
					{
						tweenPosition2.from.z -= (float)i * 0.1f;
						tweenPosition2.to.z -= (float)i * 0.1f;
						tweenPosition2.Reset();
						tweenPosition2.enabled = false;
						if (i != num - 1)
						{
							tweenPosition2.callWhenFinished = "OnHeartTweenDone2";
						}
						heartTweeners[i] = tweenPosition2;
					}
				}
			}
			heart.SetActive(false);
			if (heartTweeners != null && heartTweeners.Length > 0 && heartTweeners[0] != null)
			{
				StartCoroutine(EnableHeartTweener(0));
			}
		}
		wasUsingInputEnabler = UICamera.useInputEnabler;
		if (!wasUsingInputEnabler)
		{
			UICamera.useInputEnabler = true;
		}
		Collider[] array = collidersToDisable;
		foreach (Collider collider in array)
		{
			if (collider != null)
			{
				collider.enabled = false;
			}
		}
		if (GetComponent<Collider>() != null)
		{
			GetComponent<Collider>().enabled = false;
		}
		if (blinkTween != null)
		{
			blinkTween.enabled = false;
		}
	}

	private IEnumerator EnableHeartTweener(int tweenIndex)
	{
		while (tweenIndex >= 0 && tweenIndex < heartTweeners.Length)
		{
			if (heartTweeners[tweenIndex] != null)
			{
				heartTweeners[tweenIndex].gameObject.SetActive(true);
				heartTweeners[tweenIndex].enabled = true;
				if (heartBeginSFX != null)
				{
					SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayGUISound(heartBeginSFX);
				}
				yield return new WaitForSeconds(heartSpawnDelay);
			}
			tweenIndex++;
		}
	}

	private void OnHeartTweenDone()
	{
		OnHeartTweenDone2();
		if (heartTweenDoneEvents != null)
		{
			heartTweenDoneEvents.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		if (!wasUsingInputEnabler)
		{
			UICamera.useInputEnabler = false;
		}
	}

	private void OnHeartTweenDone2()
	{
		if (doneIndex > 0 && doneIndex < heartInstances.Length && heartInstances[doneIndex] != null)
		{
			Object.Destroy(heartInstances[doneIndex]);
			doneIndex++;
		}
		if (heartEndSFX != null)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayGUISound(heartEndSFX);
		}
		if (heartVFXPrefab != null)
		{
			GameObject gameObject = SLOTGame.InstantiateFX(heartVFXPrefab) as GameObject;
			if (gameObject != null)
			{
				gameObject.transform.position = heartPos;
			}
		}
	}
}
