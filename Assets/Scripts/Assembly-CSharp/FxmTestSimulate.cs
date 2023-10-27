using UnityEngine;

public class FxmTestSimulate : MonoBehaviour
{
	public enum MODE_TYPE
	{
		NONE = 0,
		MOVE = 1,
		ARC = 2,
		ROTATE = 3,
		TORNADO = 4,
		SCALE = 5,
	}

	public MODE_TYPE m_Mode;
	public FxmTestControls.AXIS m_nAxis;
	public float m_fStartTime;
	public Vector3 m_StartPos;
	public Vector3 m_EndPos;
	public float m_fSpeed;
	public bool m_bRotFront;
	public float m_fDist;
	public float m_fRadius;
	public float m_fArcLenRate;
	public AnimationCurve m_Curve;
	public Component m_FXMakerControls;
	public int m_nMultiShotIndex;
	public int m_nMultiShotCount;
	public int m_nCircleCount;
	public Vector3 m_PrevPosition;
}
