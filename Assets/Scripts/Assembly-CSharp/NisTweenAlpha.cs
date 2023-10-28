using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("NIS/Tween/Alpha")]
[RequireComponent(typeof(Graphic))]
public class NisTweenAlpha : UITweener
{
	public float from = 1f;

	public float to = 1f;

	private Transform mTrans;

	private Graphic mWidget;

	public float alpha
	{
		get
		{
			if (mWidget != null)
			{
				return mWidget.color.a;
			}
			return 0f;
		}
		set
		{
			if (mWidget != null)
			{
				mWidget.color = new Color(mWidget.color.r, mWidget.color.g, mWidget.color.b, value);
			}
		}
	}

	private void Awake()
	{
		mWidget = GetComponent<Graphic>();
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		alpha = Mathf.Lerp(from, to, factor);
	}

	public static NisTweenAlpha PlayTo(GameObject go, float duration, float alpha)
	{
		return Play(go, duration, null, alpha);
	}

	public static NisTweenAlpha PlayFrom(GameObject go, float duration, float alpha)
	{
		return Play(go, duration, alpha, null);
	}

	public static NisTweenAlpha Play(GameObject go, float duration, float? alphaFrom, float? alphaTo)
	{
		NisTweenAlpha nisTweenAlpha = UITweener.Begin<NisTweenAlpha>(go, duration);
		nisTweenAlpha.from = ((!alphaFrom.HasValue) ? nisTweenAlpha.alpha : alphaFrom.Value);
		nisTweenAlpha.to = ((!alphaTo.HasValue) ? nisTweenAlpha.alpha : alphaTo.Value);
		if (duration <= 0f)
		{
			nisTweenAlpha.Sample(1f, true);
			nisTweenAlpha.enabled = false;
		}
		return nisTweenAlpha;
	}
}
