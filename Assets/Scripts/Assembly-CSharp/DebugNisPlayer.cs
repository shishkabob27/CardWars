using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DebugNisPlayer : NisSequence
{
	public GameObject nisRoot;

	public GameObject skipPromptGO;

	public Transform nisDialog;

	public Button nisListItemPrefab;

	public string nisResourcePath = string.Empty;

	private GameObject nisListWidget;

	private List<GameObject> nisPrefabsAll = new List<GameObject>();

	private void Start()
	{
		GameObject gameObject = ((!(nisRoot != null)) ? base.gameObject : nisRoot);
		if (nisDialog != null)
		{
			Button button = UnityUtils.FindChildRecursive<Button>(nisDialog.gameObject, "PlayAllButton");
			if (button != null)
			{
				button.onClick.AddListener(PlayAll);
			}
			Transform transform = UnityUtils.FindChildRecursive(nisDialog.gameObject, "ScrollPanel/Panel");
			nisListWidget = ((!(transform != null)) ? null : transform.gameObject);
			nisDialog.gameObject.SetActive(true);
		}
		Transform transform2 = gameObject.transform;
		int childCount = transform2.childCount;
		for (int i = 0; i < childCount; i++)
		{
			NisComponent componentInChildren = transform2.GetChild(i).GetComponentInChildren<NisComponent>();
			if (componentInChildren != null)
			{
				AddSeqToGlobalList(componentInChildren, "Local");
			}
		}
		Object[] array = Resources.LoadAll(nisResourcePath, typeof(NisComponent));
		Object[] array2 = array;
		for (int j = 0; j < array2.Length; j++)
		{
			NisComponent nisComponent = (NisComponent)array2[j];
			if (!nisComponent.name.StartsWith("low_"))
			{
				AddSeqToGlobalList(nisComponent, "Res");
			}
		}
		Resources.UnloadUnusedAssets();
		if (skipPromptGO != null)
		{
			skipPromptGO.SetActive(false);
		}
	}

	private void AddSeqToGlobalList(NisComponent seq, string category)
	{
		nisPrefabsAll.Add(seq.gameObject);
		if (nisListWidget != null)
		{
			Button button = UnityUtils.InstantiatePrefab(nisListItemPrefab, nisListWidget);
			button.onClick.AddListener(ConstructCallbackPlayOne(seq.gameObject));
			button.transform.Find("Text").GetComponent<Text>().text = category + ": " + seq.name;
		}
	}

	private UnityAction ConstructCallbackPlayOne(GameObject nisPrefab)
	{
		return delegate
		{
			if (base.isActiveAndEnabled && !isPlaying)
			{
				GameObject parent = ((!(nisRoot != null)) ? base.gameObject : nisRoot);
				segments.Clear();
				segments.Add(UnityUtils.InstantiatePrefab(nisPrefab, parent).GetComponent<NisComponent>());
				Play();
			}
		};
	}

	private void PlayAll()
	{
		if (!base.isActiveAndEnabled || isPlaying)
		{
			return;
		}
		GameObject parent = ((!(nisRoot != null)) ? base.gameObject : nisRoot);
		segments.Clear();
		foreach (GameObject item in nisPrefabsAll)
		{
			segments.Add(UnityUtils.InstantiatePrefab(item, parent).GetComponent<NisComponent>());
		}
		Play();
	}

	public override void Play()
	{
		if (base.isActiveAndEnabled && !isPlaying)
		{
			SetupSkipPromptHandling();
			base.Play();
			if (isPlaying && nisDialog != null)
			{
				nisDialog.gameObject.SetActive(false);
			}
		}
	}

	protected override void SetComplete()
	{
		if (nisDialog != null)
		{
			nisDialog.gameObject.SetActive(true);
		}
		base.SetComplete();
		TeardownSkipPromptHandling();
		foreach (NisComponent segment in segments)
		{
			if (segment != null)
			{
				Object.Destroy(segment.gameObject);
			}
		}
		segments.Clear();
	}

	protected override void PlayNextSegment()
	{
		int num = playingSegmentIndex;
		base.PlayNextSegment();
		if (num >= 0 && num < segments.Count)
		{
			Object.Destroy(segments[num].gameObject);
		}
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
