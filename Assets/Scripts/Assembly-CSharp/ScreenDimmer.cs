using System.Collections;
using UnityEngine;

public class ScreenDimmer : MonoBehaviour
{
	public enum StartupType
	{
		FadeIn,
		FadeOut,
		FadedIn,
		FadedOut
	}

	public float fadeDuration = 0.5f;

	public float alpha = 0.5f;

	public StartupType startupType;

	private TweenAlpha tweenAlpha;

	private void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		TweenAlpha tweenAlpha = base.gameObject.GetComponent(typeof(TweenAlpha)) as TweenAlpha;
		if (tweenAlpha == null)
		{
			this.tweenAlpha = base.gameObject.AddComponent(typeof(TweenAlpha)) as TweenAlpha;
			if (this.tweenAlpha != null)
			{
				this.tweenAlpha.duration = 0.5f;
				this.tweenAlpha.from = 0f;
				this.tweenAlpha.to = alpha;
			}
		}
	}

	private void OnDestroy()
	{
		if (Application.isPlaying && tweenAlpha != null)
		{
			Object.Destroy(tweenAlpha);
			tweenAlpha = null;
		}
	}

	private void OnEnable()
	{
		if (tweenAlpha != null)
		{
			switch (startupType)
			{
			case StartupType.FadeIn:
				tweenAlpha.Play(true);
				tweenAlpha.Reset();
				tweenAlpha.enabled = true;
				break;
			case StartupType.FadeOut:
				tweenAlpha.Play(false);
				tweenAlpha.Reset();
				tweenAlpha.enabled = true;
				break;
			case StartupType.FadedIn:
				tweenAlpha.Play(false);
				tweenAlpha.Reset();
				tweenAlpha.enabled = false;
				break;
			case StartupType.FadedOut:
				tweenAlpha.Play(true);
				tweenAlpha.Reset();
				tweenAlpha.enabled = false;
				break;
			}
		}
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Vector3 localScale = Vector3.one;
			if (base.transform.parent != null)
			{
				localScale = SLOTGame.GetScale(base.transform.parent);
			}
			localScale.x = ((localScale.x != 0f) ? ((float)Screen.width / localScale.x) : 0f);
			localScale.y = ((localScale.y != 0f) ? ((float)Screen.height / localScale.y) : 0f);
			base.transform.localScale = localScale;
		}
	}

	private void DestroyOnClose()
	{
		StartCoroutine(DestroyOnCloseCoroutine());
	}

	private IEnumerator DestroyOnCloseCoroutine()
	{
		yield return null;
		Object.Destroy(base.gameObject);
	}

	public void FadeIn()
	{
		if (tweenAlpha != null)
		{
			tweenAlpha.Play(true);
		}
	}

	public void FadeOut()
	{
		if (tweenAlpha != null)
		{
			tweenAlpha.Play(false);
		}
	}
}
