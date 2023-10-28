using UnityEngine;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NGUI/Examples/Typewriter Effect (ignoreTimeScale)")]
public class TypewriterEffectIgnoreTime : MonoBehaviour
{
	public string CWtutorialText;

	public int CWtextLength;

	public CWTutorialTapDelegate tapDelegateScript;

	public int charsPerSecond = 40;

	private UILabel mLabel;

	private string mText;

	private int mOffset;

	private float mNextChar;

	public bool ignoreTimeScale = true;

	private float mRt;

	private float mTimeStart;

	private float mTimeDelta;

	private float mActual;

	private bool mTimeStarted;

	private bool mStarted;

	private float mStartTime;

	private float mDuration;

	private float mAmountPerDelta = 1f;

	private float mFactor;

	public float duration = 1f;

	private bool fastForward;

	private float debugTotalDelay;

	public float realTime
	{
		get
		{
			return mRt;
		}
	}

	public float amountPerDelta
	{
		get
		{
			if (mDuration != duration)
			{
				mDuration = duration;
				mAmountPerDelta = Mathf.Abs((!(duration > 0f)) ? 1000f : (1f / duration));
			}
			return mAmountPerDelta;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (CWtextLength == 0)
		{
			CWtextLength = CWtutorialText.Length;
		}
		float num = ((!ignoreTimeScale) ? Time.deltaTime : UpdateRealTimeDelta());
		float num2 = ((!ignoreTimeScale) ? Time.time : realTime);
		if (!mStarted)
		{
			mStarted = true;
			mStartTime = num2;
			debugTotalDelay = 0f;
		}
		if (num2 < mStartTime)
		{
			return;
		}
		mFactor += amountPerDelta * num;
		if (mFactor > 1f || mFactor < 0f)
		{
			mFactor = Mathf.Clamp01(mFactor);
			if ((mFactor == 1f && mAmountPerDelta > 0f) || (mFactor == 0f && mAmountPerDelta < 0f))
			{
				SetSkippable();
			}
		}
		PlayTypewriterEffect();
	}

	private void SetSkippable()
	{
		tapDelegateScript = base.transform.parent.GetComponent<CWTutorialTapDelegate>();
		if (tapDelegateScript != null)
		{
			tapDelegateScript.skipFlag = true;
		}
	}

	private void PlayTypewriterEffect()
	{
		if (mLabel == null)
		{
			mLabel = GetComponent<UILabel>();
			mLabel.supportEncoding = false;
			mLabel.symbolStyle = UIFont.SymbolStyle.None;
			Vector2 vector = mLabel.cachedTransform.localScale;
			mLabel.font.WrapText(mLabel.text, out mText, (float)mLabel.lineWidth / vector.x, (float)mLabel.lineHeight / vector.y, mLabel.maxLineCount, false, UIFont.SymbolStyle.None);
		}
		if (mOffset < mText.Length)
		{
			if (!(mNextChar <= realTime))
			{
				return;
			}
			float num = realTime - mNextChar;
			if (mOffset == 0)
			{
				num = 0f;
			}
			float num2 = Mathf.Max(1, charsPerSecond);
			num2 = charsPerSecond;
			num2 *= (float)((!fastForward) ? 1 : 100);
			float num3 = 1f / num2;
			char c = mText[mOffset];
			if (c == '.' || c == '\n' || c == '!' || c == '?' || c == ',')
			{
				num3 *= 6f;
			}
			mLabel.text = mText.Substring(0, ++mOffset);
			int num4 = (int)(num / num3);
			for (int i = 0; i < num4; i++)
			{
				if (mOffset >= mText.Length)
				{
					break;
				}
				mLabel.text = mText.Substring(0, ++mOffset);
			}
			mNextChar = realTime + num3;
			debugTotalDelay += num3;
		}
		else
		{
			SetSkippable();
			base.enabled = false;
		}
	}

	protected float UpdateRealTimeDelta()
	{
		mRt = Time.realtimeSinceStartup;
		if (mTimeStarted)
		{
			float b = mRt - mTimeStart;
			mActual += Mathf.Max(0f, b);
			mTimeDelta = 0.001f * Mathf.Round(mActual * 1000f);
			mActual -= mTimeDelta;
			if (mTimeDelta > 1f)
			{
				mTimeDelta = 1f;
			}
			mTimeStart = mRt;
		}
		else
		{
			mTimeStarted = true;
			mTimeStart = mRt;
			mTimeDelta = 0f;
		}
		return mTimeDelta;
	}

	public void FastForward()
	{
		fastForward = true;
	}

	public bool IsDone()
	{
		return mText == null || mOffset >= mText.Length;
	}
}
