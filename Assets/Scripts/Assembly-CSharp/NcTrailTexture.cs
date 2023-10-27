using UnityEngine;

public class NcTrailTexture : NcEffectBehaviour
{
	public enum AXIS_TYPE
	{
		AXIS_FORWARD = 0,
		AXIS_BACK = 1,
		AXIS_RIGHT = 2,
		AXIS_LEFT = 3,
		AXIS_UP = 4,
		AXIS_DOWN = 5,
	}

	public float m_fDelayTime;
	public float m_fEmitTime;
	public bool m_bSmoothHide;
	public float m_fLifeTime;
	public AXIS_TYPE m_TipAxis;
	public float m_fTipSize;
	public bool m_bCenterAlign;
	public bool m_UvFlipHorizontal;
	public bool m_UvFlipVirtical;
	public int m_nFadeHeadCount;
	public int m_nFadeTailCount;
	public Color[] m_Colors;
	public float[] m_SizeRates;
	public bool m_bInterpolation;
	public int m_nMaxSmoothCount;
	public int m_nSubdivisions;
	public float m_fMinVertexDistance;
	public float m_fMaxVertexDistance;
	public float m_fMaxAngle;
	public bool m_bAutoDestruct;
}
