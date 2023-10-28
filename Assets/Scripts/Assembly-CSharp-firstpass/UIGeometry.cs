using UnityEngine;

public class UIGeometry
{
	public BetterList<Vector3> verts = new BetterList<Vector3>();

	public BetterList<Vector2> uvs = new BetterList<Vector2>();

	public BetterList<Color32> cols = new BetterList<Color32>();

	private BetterList<Vector3> mRtpVerts = new BetterList<Vector3>();

	private Vector3 mRtpNormal;

	private Vector4 mRtpTan;

	public bool hasVertices
	{
		get
		{
			return verts.size > 0;
		}
	}

	public bool hasTransformed
	{
		get
		{
			return mRtpVerts != null && mRtpVerts.size > 0 && mRtpVerts.size == verts.size;
		}
	}

	public void Clear()
	{
		verts.Clear();
		uvs.Clear();
		cols.Clear();
		mRtpVerts.Clear();
	}

	public void ApplyOffset(Vector3 pivotOffset)
	{
		for (int i = 0; i < verts.size; i++)
		{
			verts.buffer[i] += pivotOffset;
		}
	}

	public void ApplyTransform(Matrix4x4 widgetToPanel)
	{
		if (verts.size > 0)
		{
			mRtpVerts.Clear();
			int i = 0;
			for (int size = verts.size; i < size; i++)
			{
				mRtpVerts.Add(widgetToPanel.MultiplyPoint3x4(verts[i]));
			}
			mRtpNormal = widgetToPanel.MultiplyVector(Vector3.back).normalized;
			Vector3 normalized = widgetToPanel.MultiplyVector(Vector3.right).normalized;
			mRtpTan = new Vector4(normalized.x, normalized.y, normalized.z, -1f);
		}
		else
		{
			mRtpVerts.Clear();
		}
	}

	public void WriteToBuffers(BetterList<Vector3> v, BetterList<Vector2> u, BetterList<Color32> c, BetterList<Vector3> n, BetterList<Vector4> t)
	{
		if (mRtpVerts == null || mRtpVerts.size <= 0)
		{
			return;
		}
		if (n == null)
		{
			for (int i = 0; i < mRtpVerts.size; i++)
			{
				v.Add(mRtpVerts.buffer[i]);
				u.Add(uvs.buffer[i]);
				c.Add(cols.buffer[i]);
			}
			return;
		}
		for (int j = 0; j < mRtpVerts.size; j++)
		{
			v.Add(mRtpVerts.buffer[j]);
			u.Add(uvs.buffer[j]);
			c.Add(cols.buffer[j]);
			n.Add(mRtpNormal);
			t.Add(mRtpTan);
		}
	}
}
