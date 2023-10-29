using UnityEngine;

public class TutorialBattleRing : MonoBehaviour
{
	public GameObject bg;

	public GameObject[] bars;

	public GameObject[] labels;

	public UIButtonTween showTween;

	public UIButtonTween hideTween;

	private Animation animToPlayWhilePaused;

	private float animElapsed;

	private float lastUpdateTime;

	public void SetPhase(int phaseIndex)
	{
		//TODO: Actually make this tutorial work.
		//Right now we just skip it.
		Time.timeScale = 1;
		return;
		base.gameObject.SetActive(true);
		animToPlayWhilePaused = null;
		switch (phaseIndex)
		{
		case 0:
			HideAllLabels();
			ShowLabel(0);
			ShowBar(0);
			if (GetComponent<Animation>() != null)
			{
				GetComponent<Animation>().Rewind();
				GetComponent<Animation>().Play();
				if (Time.timeScale == 0f)
				{
					animToPlayWhilePaused = GetComponent<Animation>();
					animElapsed = 0f;
				}
			}
			break;
		case 1:
			break;
		case 2:
		case 3:
		case 4:
			ShowLabel(phaseIndex - 1);
			ShowBar(phaseIndex - 1);
			break;
		case 5:
			if (hideTween != null)
			{
				hideTween.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
	}

	private void HideAllLabels()
	{
		GameObject[] array = labels;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
	}

	private void ShowLabel(int i)
	{
		if (i >= 0 && i < labels.Length && labels[i] != null)
		{
			labels[i].SetActive(true);
		}
		for (int j = 0; j < labels.Length; j++)
		{
			if (labels[j] != null && labels[j].activeInHierarchy)
			{
				EnableTweens(labels[j], j == i);
			}
		}
	}

	private void ShowBar(int i)
	{
		for (int j = 0; j < bars.Length; j++)
		{
			if (bars[j] != null)
			{
				bars[j].SetActive(j == i);
				EnableTweens(bars[j], i > 0 && j == i);
			}
		}
	}

	private void EnableTweens(GameObject obj, bool enable)
	{
		if (!(obj != null))
		{
			return;
		}
		Component[] componentsInChildren = obj.GetComponentsInChildren(typeof(UITweener));
		Component[] array = componentsInChildren;
		foreach (Component component in array)
		{
			UITweener uITweener = component as UITweener;
			if (uITweener != null)
			{
				uITweener.Play(true);
				uITweener.Reset();
				uITweener.enabled = enable;
			}
		}
	}

	private void Update()
	{
		if (!(animToPlayWhilePaused != null))
		{
			return;
		}
		float num = 0f;
		if (lastUpdateTime > 0f)
		{
			num = Time.realtimeSinceStartup - lastUpdateTime;
		}
		lastUpdateTime = Time.realtimeSinceStartup;
		foreach (AnimationState item in animToPlayWhilePaused)
		{
			if (item != null)
			{
				item.time = animElapsed;
			}
		}
		animToPlayWhilePaused.Sample();
		animElapsed += num;
	}
}
