using UnityEngine;

public class FxmTestSimulate : MonoBehaviour
{
	public enum MODE_TYPE
	{
		NONE,
		MOVE,
		ARC,
		ROTATE,
		TORNADO,
		SCALE
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

	public Vector3 m_PrevPosition = Vector3.zero;

	protected static int m_nMultiShotCreate;

	public void Init(Component fxmEffectControls, int nMultiShotCount)
	{
		m_FXMakerControls = fxmEffectControls;
		m_nMultiShotCount = nMultiShotCount;
	}

    public void SimulateMove(FxmTestControls.AXIS nTransAxis, float fHalfDist, float fSpeed, bool bRotFront)
    {
        Vector3 position = base.transform.position;
        m_nAxis = nTransAxis;
        m_StartPos = position;
        m_EndPos = position;

        int nAxis;
        int index = (nAxis = (int)m_nAxis);
        float num = m_StartPos[nAxis];
        m_StartPos[index] = num - fHalfDist;

        int index2 = (nAxis = (int)m_nAxis);
        num = m_EndPos[nAxis];
        m_EndPos[index2] = num + fHalfDist;

        m_fDist = Vector3.Distance(m_StartPos, m_EndPos);
        m_Mode = MODE_TYPE.MOVE;
        SimulateStart(m_StartPos, fSpeed, bRotFront);
    }

    public void SimulateArc(float fHalfDist, float fSpeed, bool bRotFront)
	{
		m_Curve = FxmTestMain.inst.m_SimulateArcCurve;
		if (m_Curve != null)
		{
			Vector3 position = base.transform.position;
			m_StartPos = new Vector3(position.x - fHalfDist, position.y, position.z);
			m_EndPos = new Vector3(position.x + fHalfDist, position.y, position.z);
			m_fDist = Vector3.Distance(m_StartPos, m_EndPos);
			m_Mode = MODE_TYPE.ARC;
			SimulateStart(m_StartPos, fSpeed, bRotFront);
		}
	}

	public void SimulateFall(float fHeight, float fSpeed, bool bRotFront)
	{
		Vector3 position = base.transform.position;
		m_StartPos = new Vector3(position.x, position.y + fHeight, position.z);
		m_EndPos = new Vector3(position.x, position.y, position.z);
		m_fDist = Vector3.Distance(m_StartPos, m_EndPos);
		m_Mode = MODE_TYPE.MOVE;
		SimulateStart(m_StartPos, fSpeed, bRotFront);
	}

	public void SimulateRaise(float fHeight, float fSpeed, bool bRotFront)
	{
		Vector3 position = base.transform.position;
		m_StartPos = new Vector3(position.x, position.y, position.z);
		m_EndPos = new Vector3(position.x, position.y + fHeight, position.z);
		m_fDist = Vector3.Distance(m_StartPos, m_EndPos);
		m_Mode = MODE_TYPE.MOVE;
		SimulateStart(m_StartPos, fSpeed, bRotFront);
	}

	public void SimulateCircle(float fRadius, float fSpeed, bool bRotFront)
	{
		Vector3 position = base.transform.position;
		m_fRadius = fRadius;
		m_Mode = MODE_TYPE.ROTATE;
		m_fDist = 1f;
		SimulateStart(new Vector3(position.x - fRadius, position.y, position.z), fSpeed, bRotFront);
	}

	public void SimulateTornado(float fRadius, float fHeight, float fSpeed, bool bRotFront)
	{
		Vector3 position = base.transform.position;
		m_fRadius = fRadius;
		m_Mode = MODE_TYPE.TORNADO;
		m_StartPos = new Vector3(position.x - fRadius, position.y, position.z);
		m_EndPos = new Vector3(position.x - fRadius, position.y + fHeight, position.z);
		m_fDist = Vector3.Distance(m_StartPos, m_EndPos);
		SimulateStart(m_StartPos, fSpeed, bRotFront);
	}

    public void SimulateScale(FxmTestControls.AXIS nTransAxis, float fHalfDist, float fStartPosition, float fSpeed, bool bRotFront)
    {
        Vector3 position = base.transform.position;
        m_nAxis = nTransAxis;
        m_StartPos = position;
        m_EndPos = position;
        int nAxis;
        int index = (nAxis = (int)m_nAxis);
        float num = m_StartPos[nAxis];
        m_StartPos[index] = num + fHalfDist * fStartPosition;
        int index2 = (nAxis = (int)m_nAxis);
        num = m_EndPos[nAxis];
        m_EndPos[index2] = num + (fHalfDist * 2f + fHalfDist * fStartPosition);
        m_fDist = Vector3.Distance(m_StartPos, m_EndPos);
        m_Mode = MODE_TYPE.SCALE;
        SimulateStart(m_StartPos, fSpeed, bRotFront);
    }

    public void Stop()
	{
		m_fSpeed = 0f;
	}

	private void SimulateStart(Vector3 startPos, float fSpeed, bool bRotFront)
	{
		base.transform.position = startPos;
		m_fSpeed = fSpeed;
		m_bRotFront = bRotFront;
		m_nCircleCount = 0;
		m_PrevPosition = Vector3.zero;
		if (bRotFront && m_Mode == MODE_TYPE.MOVE)
		{
			base.transform.LookAt(m_EndPos);
		}
		if (m_Mode != MODE_TYPE.SCALE && 1 < m_nMultiShotCount)
		{
			NcDuplicator ncDuplicator = base.gameObject.AddComponent<NcDuplicator>();
			ncDuplicator.m_fDuplicateTime = 0.2f;
			ncDuplicator.m_nDuplicateCount = m_nMultiShotCount;
			ncDuplicator.m_fDuplicateLifeTime = 0f;
			m_nMultiShotCreate = 0;
			m_nMultiShotIndex = 0;
		}
		m_fStartTime = Time.time;
		Update();
	}

	private Vector3 GetArcPos(float fTimeRate)
	{
		Vector3 vector = Vector3.Lerp(m_StartPos, m_EndPos, fTimeRate);
		return new Vector3(vector.x, m_Curve.Evaluate(fTimeRate) * m_fDist, vector.z);
	}

	private void Awake()
	{
		m_nMultiShotIndex = m_nMultiShotCreate;
		m_nMultiShotCreate++;
	}

	private void Start()
	{
		m_fStartTime = Time.time;
	}

	private void Update()
	{
		if (0f < m_fDist && 0f < m_fSpeed)
		{
			switch (m_Mode)
			{
			case MODE_TYPE.MOVE:
			{
				float num3 = m_fDist / m_fSpeed;
				float num4 = Time.time - m_fStartTime;
				base.transform.position = Vector3.Lerp(m_StartPos, m_EndPos, num4 / num3);
				if (1f < num4 / num3)
				{
					OnMoveEnd();
				}
				break;
			}
			case MODE_TYPE.ARC:
			{
				float num6 = m_fDist / m_fSpeed;
				float num7 = Time.time - m_fStartTime;
				Vector3 arcPos = GetArcPos(num7 / num6 + num7 / num6 * 0.01f);
				base.transform.position = GetArcPos(num7 / num6);
				if (m_bRotFront)
				{
					base.transform.LookAt(arcPos);
				}
				if (1f < num7 / num6)
				{
					OnMoveEnd();
				}
				break;
			}
			case MODE_TYPE.ROTATE:
			{
				float num5 = m_fSpeed / 3.14f * 360f;
				base.transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * ((m_fRadius != 0f) ? (num5 / (m_fRadius * 2f)) : 0f));
				if (m_PrevPosition.z < 0f && 0f < base.transform.position.z)
				{
					if (1 <= m_nCircleCount)
					{
						OnMoveEnd();
					}
					m_nCircleCount++;
				}
				break;
			}
			case MODE_TYPE.TORNADO:
			{
				float num8 = m_fDist / (m_fSpeed / 20f);
				float num9 = Time.time - m_fStartTime;
				Vector3 vector = Vector3.Lerp(m_StartPos, m_EndPos, num9 / num8);
				base.transform.position = new Vector3(base.transform.position.x, vector.y, base.transform.position.z);
				float num10 = m_fSpeed / 3.14f * 360f;
				base.transform.RotateAround(new Vector3(0f, vector.y, 0f), Vector3.up, Time.deltaTime * ((m_fRadius != 0f) ? (num10 / (m_fRadius * 2f)) : 0f));
				if (1f < num9 / num8)
				{
					OnMoveEnd();
				}
				break;
			}
			case MODE_TYPE.SCALE:
			{
				float num = m_fDist / m_fSpeed;
				float num2 = Time.time - m_fStartTime;
				Vector3 localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
				localScale[(int)m_nAxis] = m_fDist * num2 / num;
				if (localScale[(int)m_nAxis] == 0f)
				{
					localScale[(int)m_nAxis] = 0.001f;
				}
				base.transform.localScale = localScale;
				if (1f < num2 / num)
				{
					OnMoveEnd();
				}
				break;
			}
			}
		}
		m_PrevPosition = base.transform.position;
	}

	private void FixedUpdate()
	{
	}

	public void LateUpdate()
	{
	}

	private void OnMoveEnd()
	{
		m_fSpeed = 0f;
		NgObject.SetActiveRecursively(base.gameObject, false);
		if ((1 >= m_nMultiShotCreate || m_nMultiShotIndex >= m_nMultiShotCreate - 1) && m_FXMakerControls != null)
		{
			m_FXMakerControls.SendMessage("OnActionTransEnd");
		}
	}
}
