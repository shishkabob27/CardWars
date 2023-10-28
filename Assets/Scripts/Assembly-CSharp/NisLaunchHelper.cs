using System;
using System.Collections;
using UnityEngine;

public class NisLaunchHelper : MonoBehaviour
{
	private const string NIS_ROOT = "Nis/";

	public GameObject nisRoot;

	public NisAsyncPlayer playerPrefab;

	public bool overrideLayer = true;

	public UIButtonTween busyTweenShow;

	public UIButtonTween busyTweenHide;

	public float busyTweenWaitSeconds = 0.3f;

	public bool cleanupPlayerOnFinish;

	public float endFadeSecsOverride = -1f;

	public Action<NisLaunchHelper> onLoaded;

	public Action<NisLaunchHelper> onComplete;

	private NisAsyncPlayer player;

	public bool isPlaying { get; private set; }

	public void LaunchNis(string nisName)
	{
		StartCoroutine(CoroutineCreateAndPlayNis(nisName));
	}

	public void Reset()
	{
		DestroyNisPlayer();
	}

	public void OnceComplete(Action<NisLaunchHelper> onCompleteCb)
	{
		if (onCompleteCb != null)
		{
			Action<NisLaunchHelper> onceCb = null;
			onceCb = delegate(NisLaunchHelper helper)
			{
				helper.onComplete = (Action<NisLaunchHelper>)Delegate.Remove(helper.onComplete, onceCb);
				onCompleteCb(helper);
			};
			onComplete = (Action<NisLaunchHelper>)Delegate.Combine(onComplete, onceCb);
		}
	}

	public void OnceLoaded(Action<NisLaunchHelper> onLoadedCb)
	{
		if (onLoadedCb != null)
		{
			Action<NisLaunchHelper> onceCb = null;
			onceCb = delegate(NisLaunchHelper helper)
			{
				helper.onLoaded = (Action<NisLaunchHelper>)Delegate.Remove(helper.onLoaded, onceCb);
				onLoadedCb(helper);
			};
			onLoaded = (Action<NisLaunchHelper>)Delegate.Combine(onLoaded, onceCb);
		}
	}

	private IEnumerator CoroutineCreateAndPlayNis(string nisName)
	{
		if (isPlaying)
		{
			yield break;
		}
		NisAsyncPlayer player = CreateNisPlayer();
		if (player == null)
		{
			yield return null;
			SetLoaded();
			SetComplete();
			yield break;
		}
		isPlaying = true;
		UICamera.useInputEnabler = true;
		player.sequencesPath = new string[1] { "Nis/" + nisName };
		player.Preload();
		if (busyTweenShow != null)
		{
			busyTweenShow.Play(true);
			if (busyTweenWaitSeconds > 0f)
			{
				yield return new WaitForSeconds(busyTweenWaitSeconds);
			}
		}
		while (player.isLoading)
		{
			yield return null;
		}
		if (busyTweenHide != null)
		{
			busyTweenHide.Play(true);
			if (busyTweenWaitSeconds > 0f)
			{
				yield return new WaitForSeconds(busyTweenWaitSeconds);
			}
		}
		SetLoaded();
		UICamera.useInputEnabler = false;
		player.Play();
		while (player.isPlaying)
		{
			yield return null;
		}
		if (cleanupPlayerOnFinish)
		{
			DestroyNisPlayer();
		}
		isPlaying = false;
		SetComplete();
	}

	private NisAsyncPlayer CreateNisPlayer()
	{
		if (player != null)
		{
			return player;
		}
		if (playerPrefab == null)
		{
			return null;
		}
		GameObject gameObject = ((!(nisRoot != null)) ? base.gameObject : nisRoot);
		player = UnityUtils.InstantiatePrefab(playerPrefab, gameObject);
		if (endFadeSecsOverride >= 0f)
		{
			player.fadeSecs = endFadeSecsOverride;
		}
		if (overrideLayer)
		{
			player.gameObject.SetLayerRecursively(gameObject.layer);
		}
		return player;
	}

	private void DestroyNisPlayer()
	{
		if (!(player == null))
		{
			UnityEngine.Object.Destroy(player.gameObject);
			player = null;
		}
	}

	private void SetComplete()
	{
		if (onComplete != null)
		{
			onComplete(this);
		}
	}

	private void SetLoaded()
	{
		if (onLoaded != null)
		{
			onLoaded(this);
		}
	}
}
