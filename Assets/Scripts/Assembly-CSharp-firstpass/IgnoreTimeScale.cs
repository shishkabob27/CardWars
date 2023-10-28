using UnityEngine;

[AddComponentMenu("NGUI/Internal/Ignore TimeScale Behaviour")]
public class IgnoreTimeScale : MonoBehaviour
{
	private float mRt;

	private float mTimeStart;

	private float mTimeDelta;

	private float mActual;

	private bool mTimeStarted;

	public float realTime
	{
		get
		{
			return mRt;
		}
	}

	public float realTimeDelta
	{
		get
		{
			return mTimeDelta;
		}
	}

	protected virtual void OnEnable()
	{
		mTimeStarted = true;
		mTimeDelta = 0f;
		mTimeStart = Time.realtimeSinceStartup;
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
}
