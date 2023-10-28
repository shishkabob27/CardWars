using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NIS/Async Player")]
public class NisAsyncPlayer : NisSequence
{
	private enum LoadState
	{
		Unloaded,
		Loading,
		Loaded
	}

	public GameObject nisRoot;

	public GameObject skipPromptGO;

	public string[] sequencesPath;

	public bool keepLastSequenceOnComplete;

	public bool noCache;

	public bool overrideLayer = true;

	public CanvasRenderer scrimPrefab;

	public float fadeSecs = 1f;

	public UIButtonTween busyTweenShow;

	public UIButtonTween busyTweenHide;

	private bool playIsActive;

	private bool queueing;

	private LoadState loadState;

	private List<NisComponent> nisPrefabs = new List<NisComponent>();

	private bool playCompleting;

	public override bool isPlaying
	{
		get
		{
			return playIsActive;
		}
	}

	public bool isLoading
	{
		get
		{
			return loadState == LoadState.Loading;
		}
	}

	private GameObject nisRootApplied
	{
		get
		{
			return (!(nisRoot != null)) ? base.gameObject : nisRoot;
		}
	}

	private void Start()
	{
		if (skipPromptGO != null)
		{
			skipPromptGO.SetActive(false);
		}
	}

	public void Preload()
	{
		StartCoroutine(CoroutinePreload());
	}

	public override void Play()
	{
		if (base.isActiveAndEnabled)
		{
			StartCoroutine(CoroutinePlay());
		}
	}

	public bool ClearCache()
	{
		if (queueing || loadState == LoadState.Loading)
		{
			return false;
		}
		nisPrefabs.Clear();
		return true;
	}

	protected override void PlayNextSegment()
	{
		StartCoroutine(CoroutineWaitToPlayNextSegment());
	}

	protected override void SetComplete()
	{
		TeardownSkipPromptHandling();
		StartCoroutine(CoroutineSetComplete());
	}

	private IEnumerator CoroutineSetComplete()
	{
		if (!playCompleting)
		{
			playCompleting = true;
			if (scrimPrefab != null && fadeSecs > 0f)
			{
				GameObject goScrim = Object.Instantiate(scrimPrefab).gameObject;
				goScrim.transform.SetParent(nisRootApplied.transform, false);
				NisTweenAlpha.PlayFrom(goScrim, fadeSecs, 0f);
				yield return new WaitForSeconds(fadeSecs);
			}
			base.SetComplete();
			playIsActive = false;
			if (!keepLastSequenceOnComplete)
			{
				ClearSegmentsAll();
			}
			if (noCache)
			{
				ClearCache();
			}
			playCompleting = false;
		}
	}

	private IEnumerator CoroutinePreload()
	{
		if (loadState != 0)
		{
			yield break;
		}
		loadState = LoadState.Loading;
		string[] array = sequencesPath;
		foreach (string sequencePath in array)
		{
			SLOTResoureRequest asyncReq = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResourceAsync(sequencePath, typeof(NisComponent));
			yield return asyncReq.asyncOp;
			NisComponent nisPrefab = asyncReq.asset as NisComponent;
			if (nisPrefab != null)
			{
				nisPrefabs.Add(nisPrefab);
			}
		}
		loadState = LoadState.Loaded;
	}

	private IEnumerator CoroutinePlay()
	{
		if (playIsActive)
		{
			yield break;
		}
		playIsActive = true;
		queueing = false;
		ClearSegmentsAll();
		SetupSkipPromptHandling();
		if (loadState == LoadState.Loaded)
		{
			foreach (NisComponent nisPrefab in nisPrefabs)
			{
				AddSegment(nisPrefab);
			}
			DoPlay();
			yield break;
		}
		Preload();
		queueing = true;
		if (busyTweenShow != null)
		{
			PlayTween(busyTweenShow);
		}
		do
		{
			yield return null;
			if (segments.Count != nisPrefabs.Count)
			{
				int endIdx = nisPrefabs.Count - 1;
				for (int currIdx = segments.Count; currIdx <= endIdx; currIdx++)
				{
					AddSegment(nisPrefabs[currIdx]);
				}
				DoPlay();
			}
		}
		while (loadState != LoadState.Loaded);
		if (busyTweenHide != null)
		{
			PlayTween(busyTweenHide);
		}
		queueing = false;
		if (!base.isPlaying)
		{
			SetComplete();
		}
	}

	private void DoPlay()
	{
		if (!base.isPlaying)
		{
			base.Play();
		}
	}

	private IEnumerator CoroutineWaitToPlayNextSegment()
	{
		while (queueing && playingSegmentIndex >= segments.Count - 1)
		{
			yield return null;
		}
		int lastSegmentIdx = playingSegmentIndex;
		base.PlayNextSegment();
		if (isPlaying && lastSegmentIdx >= 0 && lastSegmentIdx < segments.Count && (!keepLastSequenceOnComplete || lastSegmentIdx < segments.Count - 1))
		{
			Object.Destroy(segments[lastSegmentIdx].gameObject);
		}
	}

	private void PlayTween(UIButtonTween target)
	{
		if (!(target == null))
		{
			GameObject gameObject = target.gameObject;
			UIButtonTween[] componentsInChildren = gameObject.GetComponentsInChildren<UIButtonTween>();
			UIButtonTween[] array = componentsInChildren;
			foreach (UIButtonTween uIButtonTween in array)
			{
				uIButtonTween.Play(true);
			}
		}
	}

	private void AddSegment(NisComponent segmentPrefab)
	{
		GameObject gameObject = nisRootApplied;
		NisComponent nisComponent = UnityUtils.InstantiatePrefab(segmentPrefab, gameObject);
		segments.Add(nisComponent);
		if (overrideLayer)
		{
			nisComponent.gameObject.SetLayerRecursively(gameObject.layer);
		}
	}

	private void ClearSegmentsAll()
	{
		foreach (NisComponent segment in segments)
		{
			if (segment != null)
			{
				Object.Destroy(segment.gameObject);
			}
		}
		segments.Clear();
	}

	private void SetupSkipPromptHandling()
	{
		if (!(skipPromptGO == null))
		{
			skipPromptGO.SetActive(base.ShowSkipPrompt);
			base.onShowSkipPrompt += OnSkipPromptStatusChange;
		}
	}

	private void TeardownSkipPromptHandling()
	{
		if (!(skipPromptGO == null))
		{
			base.onShowSkipPrompt -= OnSkipPromptStatusChange;
			skipPromptGO.SetActive(false);
		}
	}

	private void OnSkipPromptStatusChange(NisComponent target, bool showPrompt)
	{
		skipPromptGO.SetActive(showPrompt);
	}
}
