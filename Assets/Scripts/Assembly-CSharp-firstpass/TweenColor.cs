using UnityEngine;

[AddComponentMenu("NGUI/Tween/Color")]
public class TweenColor : UITweener
{
	public Color from = Color.white;

	public Color to = Color.white;

	private Transform mTrans;

	private UIWidget mWidget;

	private Material mMat;

	private Light mLight;

	public Color color
	{
		get
		{
			if (mWidget != null)
			{
				return mWidget.color;
			}
			if (mLight != null)
			{
				return mLight.color;
			}
			if (mMat != null)
			{
				return mMat.color;
			}
			return Color.black;
		}
		set
		{
			if (mWidget != null)
			{
				mWidget.color = value;
			}
			if (mMat != null)
			{
				mMat.color = value;
			}
			if (mLight != null)
			{
				mLight.color = value;
				mLight.enabled = value.r + value.g + value.b > 0.01f;
			}
		}
	}

	private void Awake()
	{
		mWidget = GetComponentInChildren<UIWidget>();
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			mMat = component.material;
		}
		mLight = GetComponent<Light>();
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		color = Color.Lerp(from, to, factor);
	}

	public static TweenColor Begin(GameObject go, float duration, Color color)
	{
		TweenColor tweenColor = UITweener.Begin<TweenColor>(go, duration);
		tweenColor.from = tweenColor.color;
		tweenColor.to = color;
		if (duration <= 0f)
		{
			tweenColor.Sample(1f, true);
			tweenColor.enabled = false;
		}
		return tweenColor;
	}
}
