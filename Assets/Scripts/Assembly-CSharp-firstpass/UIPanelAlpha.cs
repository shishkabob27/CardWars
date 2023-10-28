using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Panel Alpha")]
public class UIPanelAlpha : MonoBehaviour
{
	public float alpha = 1f;

	private Collider[] mColliders;

	private UIWidget[] mWidgets;

	private float[] mAlpha;

	private float mLastAlpha = 1f;

	private int mLevel = 2;

	private void Start()
	{
		mColliders = GetComponentsInChildren<Collider>(true);
		mWidgets = GetComponentsInChildren<UIWidget>(true);
		if (mWidgets.Length == 0)
		{
			base.enabled = false;
			return;
		}
		mAlpha = new float[mWidgets.Length];
		int i = 0;
		for (int num = mWidgets.Length; i < num; i++)
		{
			mAlpha[i] = mWidgets[i].alpha;
		}
		mLastAlpha = Mathf.Clamp01(alpha);
		mLevel = ((mLastAlpha > 0.99f) ? 2 : ((!(mLastAlpha < 0.01f)) ? 1 : 0));
		UpdateAlpha();
	}

	private void Update()
	{
		alpha = Mathf.Clamp01(alpha);
		if (mLastAlpha != alpha)
		{
			mLastAlpha = alpha;
			UpdateAlpha();
		}
	}

	private void UpdateAlpha()
	{
		int i = 0;
		for (int num = mWidgets.Length; i < num; i++)
		{
			UIWidget uIWidget = mWidgets[i];
			if (uIWidget != null)
			{
				uIWidget.alpha = mAlpha[i] * alpha;
			}
		}
		if (mLevel == 0)
		{
			Transform transform = base.transform;
			int j = 0;
			for (int childCount = transform.childCount; j < childCount; j++)
			{
				NGUITools.SetActive(transform.GetChild(j).gameObject, true);
			}
			int k = 0;
			for (int num2 = mColliders.Length; k < num2; k++)
			{
				mColliders[k].enabled = false;
			}
			mLevel = 1;
		}
		else if (mLevel == 2 && alpha < 0.99f)
		{
			TweenColor[] componentsInChildren = GetComponentsInChildren<TweenColor>();
			int l = 0;
			for (int num3 = componentsInChildren.Length; l < num3; l++)
			{
				componentsInChildren[l].enabled = false;
			}
			int m = 0;
			for (int num4 = mColliders.Length; m < num4; m++)
			{
				mColliders[m].enabled = false;
			}
			mLevel = 1;
		}
		if (mLevel != 1)
		{
			return;
		}
		if (alpha < 0.01f)
		{
			Transform transform2 = base.transform;
			int n = 0;
			for (int childCount2 = transform2.childCount; n < childCount2; n++)
			{
				NGUITools.SetActive(transform2.GetChild(n).gameObject, false);
			}
			mLevel = 0;
		}
		else if (alpha > 0.99f)
		{
			int num5 = 0;
			for (int num6 = mColliders.Length; num5 < num6; num5++)
			{
				mColliders[num5].enabled = true;
			}
			mLevel = 2;
		}
	}
}
