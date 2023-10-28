using System.Collections.Generic;
using UnityEngine;

public class NcTrailTexture : NcEffectBehaviour
{
	public enum AXIS_TYPE
	{
		AXIS_FORWARD,
		AXIS_BACK,
		AXIS_RIGHT,
		AXIS_LEFT,
		AXIS_UP,
		AXIS_DOWN
	}

	public class Point
	{
		public float timeCreated;

		public Vector3 basePosition;

		public Vector3 tipPosition;

		public bool lineBreak;
	}

	public float m_fDelayTime;

	public float m_fEmitTime;

	public bool m_bSmoothHide = true;

	protected bool m_bEmit = true;

	protected float m_fStartTime;

	protected float m_fStopTime;

	public float m_fLifeTime = 0.7f;

	public AXIS_TYPE m_TipAxis = AXIS_TYPE.AXIS_BACK;

	public float m_fTipSize = 1f;

	public bool m_bCenterAlign;

	public bool m_UvFlipHorizontal;

	public bool m_UvFlipVirtical;

	public int m_nFadeHeadCount = 2;

	public int m_nFadeTailCount = 2;

	public Color[] m_Colors;

	public float[] m_SizeRates;

	public bool m_bInterpolation;

	public int m_nMaxSmoothCount = 10;

	public int m_nSubdivisions = 4;

	protected List<Point> m_SmoothedPoints = new List<Point>();

	public float m_fMinVertexDistance = 0.2f;

	public float m_fMaxVertexDistance = 10f;

	public float m_fMaxAngle = 3f;

	public bool m_bAutoDestruct;

	protected List<Point> m_Points = new List<Point>();

	protected Transform m_base;

	protected GameObject m_TrialObject;

	protected Mesh m_TrailMesh;

	protected Vector3 m_LastPosition;

	protected Vector3 m_LastCameraPosition1;

	protected Vector3 m_LastCameraPosition2;

	protected bool m_bLastFrameEmit = true;

	public void SetEmit(bool bEmit)
	{
		m_bEmit = bEmit;
		m_fStartTime = NcEffectBehaviour.GetEngineTime();
		m_fStopTime = 0f;
	}

	public override int GetAnimationState()
	{
		if (base.enabled && NcEffectBehaviour.IsActive(base.gameObject))
		{
			if (NcEffectBehaviour.GetEngineTime() < m_fStartTime + m_fDelayTime + 0.1f)
			{
				return 1;
			}
			return -1;
		}
		return -1;
	}

	private void OnDisable()
	{
		if (m_TrialObject != null)
		{
			NcAutoDestruct.CreateAutoDestruct(m_TrialObject, 0f, m_fLifeTime / 2f, true, true);
		}
	}

	private void Start()
	{
		if (GetComponent<Renderer>() == null || GetComponent<Renderer>().sharedMaterial == null)
		{
			base.enabled = false;
		}
		else if (0f < m_fDelayTime)
		{
			m_fStartTime = NcEffectBehaviour.GetEngineTime();
		}
		else
		{
			InitTrailObject();
		}
	}

	private void InitTrailObject()
	{
		m_base = base.transform;
		m_fStartTime = NcEffectBehaviour.GetEngineTime();
		m_LastPosition = base.transform.position;
		m_TrialObject = new GameObject("Trail");
		m_TrialObject.transform.position = Vector3.zero;
		m_TrialObject.transform.rotation = Quaternion.identity;
		m_TrialObject.transform.localScale = base.transform.localScale;
		m_TrialObject.AddComponent(typeof(MeshFilter));
		m_TrialObject.AddComponent(typeof(MeshRenderer));
		m_TrialObject.GetComponent<Renderer>().sharedMaterial = GetComponent<Renderer>().sharedMaterial;
		m_TrailMesh = m_TrialObject.GetComponent<MeshFilter>().mesh;
		CreateEditorGameObject(m_TrialObject);
	}

	private Vector3 GetTipPoint()
	{
		switch (m_TipAxis)
		{
		case AXIS_TYPE.AXIS_FORWARD:
			return m_base.position + m_base.forward;
		case AXIS_TYPE.AXIS_BACK:
			return m_base.position + m_base.forward * -1f;
		case AXIS_TYPE.AXIS_RIGHT:
			return m_base.position + m_base.right;
		case AXIS_TYPE.AXIS_LEFT:
			return m_base.position + m_base.right * -1f;
		case AXIS_TYPE.AXIS_UP:
			return m_base.position + m_base.up;
		case AXIS_TYPE.AXIS_DOWN:
			return m_base.position + m_base.up * -1f;
		default:
			return m_base.position + m_base.forward;
		}
	}

	private void Update()
	{
		if (GetComponent<Renderer>() == null || GetComponent<Renderer>().sharedMaterial == null)
		{
			base.enabled = false;
			return;
		}
		if (0f < m_fDelayTime)
		{
			if (NcEffectBehaviour.GetEngineTime() < m_fStartTime + m_fDelayTime)
			{
				return;
			}
			m_fDelayTime = 0f;
			m_fStartTime = 0f;
			InitTrailObject();
		}
		if (m_bEmit && 0f < m_fEmitTime && m_fStopTime == 0f && m_fStartTime + m_fEmitTime < NcEffectBehaviour.GetEngineTime())
		{
			if (m_bSmoothHide)
			{
				m_fStopTime = NcEffectBehaviour.GetEngineTime();
			}
			else
			{
				m_bEmit = false;
			}
		}
		if (0f < m_fStopTime && m_fLifeTime < NcEffectBehaviour.GetEngineTime() - m_fStopTime)
		{
			m_bEmit = false;
		}
		if (!m_bEmit && m_Points.Count == 0 && m_bAutoDestruct)
		{
			Object.Destroy(m_TrialObject);
			Object.Destroy(base.gameObject);
		}
		float magnitude = (m_LastPosition - base.transform.position).magnitude;
		if (m_bEmit)
		{
			if (magnitude > m_fMinVertexDistance)
			{
				bool flag = false;
				if (m_Points.Count < 3)
				{
					flag = true;
				}
				else
				{
					Vector3 from = m_Points[m_Points.Count - 2].basePosition - m_Points[m_Points.Count - 3].basePosition;
					Vector3 to = m_Points[m_Points.Count - 1].basePosition - m_Points[m_Points.Count - 2].basePosition;
					if (Vector3.Angle(from, to) > m_fMaxAngle || magnitude > m_fMaxVertexDistance)
					{
						flag = true;
					}
				}
				if (flag)
				{
					Point point = new Point();
					point.basePosition = m_base.position;
					point.tipPosition = GetTipPoint();
					if (0f < m_fStopTime)
					{
						point.timeCreated = NcEffectBehaviour.GetEngineTime() - (NcEffectBehaviour.GetEngineTime() - m_fStopTime);
					}
					else
					{
						point.timeCreated = NcEffectBehaviour.GetEngineTime();
					}
					m_Points.Add(point);
					m_LastPosition = base.transform.position;
					if (m_bInterpolation)
					{
						if (m_Points.Count == 1)
						{
							m_SmoothedPoints.Add(point);
						}
						else if (1 < m_Points.Count)
						{
							for (int i = 0; i < 1 + m_nSubdivisions; i++)
							{
								m_SmoothedPoints.Add(point);
							}
						}
						int num = 2;
						if (num <= m_Points.Count)
						{
							int num2 = Mathf.Min(m_nMaxSmoothCount, m_Points.Count);
							Vector3[] array = new Vector3[num2];
							for (int j = 0; j < num2; j++)
							{
								array[j] = m_Points[m_Points.Count - (num2 - j)].basePosition;
							}
							IEnumerable<Vector3> collection = NgInterpolate.NewCatmullRom(array, m_nSubdivisions, false);
							Vector3[] array2 = new Vector3[num2];
							for (int k = 0; k < num2; k++)
							{
								array2[k] = m_Points[m_Points.Count - (num2 - k)].tipPosition;
							}
							IEnumerable<Vector3> collection2 = NgInterpolate.NewCatmullRom(array2, m_nSubdivisions, false);
							List<Vector3> list = new List<Vector3>(collection);
							List<Vector3> list2 = new List<Vector3>(collection2);
							float timeCreated = m_Points[m_Points.Count - num2].timeCreated;
							float timeCreated2 = m_Points[m_Points.Count - 1].timeCreated;
							for (int l = 0; l < list.Count; l++)
							{
								int num3 = m_SmoothedPoints.Count - (list.Count - l);
								if (-1 < num3 && num3 < m_SmoothedPoints.Count)
								{
									Point point2 = new Point();
									point2.tipPosition = list2[l];
									point2.basePosition = list[l];
									point2.timeCreated = Mathf.Lerp(timeCreated, timeCreated2, (float)l / (float)list.Count);
									m_SmoothedPoints[num3] = point2;
								}
							}
						}
					}
				}
				else
				{
					m_Points[m_Points.Count - 1].tipPosition = GetTipPoint();
					m_Points[m_Points.Count - 1].basePosition = m_base.position;
					if (m_bInterpolation)
					{
						m_SmoothedPoints[m_SmoothedPoints.Count - 1].tipPosition = GetTipPoint();
						m_SmoothedPoints[m_SmoothedPoints.Count - 1].basePosition = m_base.position;
					}
				}
			}
			else
			{
				if (m_Points.Count > 0)
				{
					m_Points[m_Points.Count - 1].tipPosition = GetTipPoint();
					m_Points[m_Points.Count - 1].basePosition = m_base.position;
				}
				if (m_bInterpolation && m_SmoothedPoints.Count > 0)
				{
					m_SmoothedPoints[m_SmoothedPoints.Count - 1].tipPosition = GetTipPoint();
					m_SmoothedPoints[m_SmoothedPoints.Count - 1].basePosition = m_base.position;
				}
			}
		}
		if (!m_bEmit && m_bLastFrameEmit && m_Points.Count > 0)
		{
			m_Points[m_Points.Count - 1].lineBreak = true;
		}
		m_bLastFrameEmit = m_bEmit;
		List<Point> list3 = new List<Point>();
		foreach (Point point4 in m_Points)
		{
			if (NcEffectBehaviour.GetEngineTime() - point4.timeCreated > m_fLifeTime)
			{
				list3.Add(point4);
			}
		}
		foreach (Point item in list3)
		{
			m_Points.Remove(item);
		}
		if (m_bInterpolation)
		{
			list3 = new List<Point>();
			foreach (Point smoothedPoint in m_SmoothedPoints)
			{
				if (NcEffectBehaviour.GetEngineTime() - smoothedPoint.timeCreated > m_fLifeTime)
				{
					list3.Add(smoothedPoint);
				}
			}
			foreach (Point item2 in list3)
			{
				m_SmoothedPoints.Remove(item2);
			}
		}
		List<Point> list4 = ((!m_bInterpolation) ? m_Points : m_SmoothedPoints);
		if (list4.Count > 1)
		{
			Vector3[] array3 = new Vector3[list4.Count * 2];
			Vector2[] array4 = new Vector2[list4.Count * 2];
			int[] array5 = new int[(list4.Count - 1) * 6];
			Color[] array6 = new Color[list4.Count * 2];
			for (int m = 0; m < list4.Count; m++)
			{
				Point point3 = list4[m];
				float num4 = (NcEffectBehaviour.GetEngineTime() - point3.timeCreated) / m_fLifeTime;
				Color color = Color.Lerp(Color.white, Color.clear, num4);
				if (m_Colors != null && m_Colors.Length > 0)
				{
					float num5 = num4 * (float)(m_Colors.Length - 1);
					float num6 = Mathf.Floor(num5);
					float num7 = Mathf.Clamp(Mathf.Ceil(num5), 1f, m_Colors.Length - 1);
					float t = Mathf.InverseLerp(num6, num7, num5);
					if (num6 >= (float)m_Colors.Length)
					{
						num6 = m_Colors.Length - 1;
					}
					if (num6 < 0f)
					{
						num6 = 0f;
					}
					if (num7 >= (float)m_Colors.Length)
					{
						num7 = m_Colors.Length - 1;
					}
					if (num7 < 0f)
					{
						num7 = 0f;
					}
					color = Color.Lerp(m_Colors[(int)num6], m_Colors[(int)num7], t);
				}
				Vector3 vector = point3.basePosition - point3.tipPosition;
				float num8 = m_fTipSize;
				if (m_SizeRates != null && m_SizeRates.Length > 0)
				{
					float num9 = num4 * (float)(m_SizeRates.Length - 1);
					float num10 = Mathf.Floor(num9);
					float num11 = Mathf.Clamp(Mathf.Ceil(num9), 1f, m_SizeRates.Length - 1);
					float t2 = Mathf.InverseLerp(num10, num11, num9);
					if (num10 >= (float)m_SizeRates.Length)
					{
						num10 = m_SizeRates.Length - 1;
					}
					if (num10 < 0f)
					{
						num10 = 0f;
					}
					if (num11 >= (float)m_SizeRates.Length)
					{
						num11 = m_SizeRates.Length - 1;
					}
					if (num11 < 0f)
					{
						num11 = 0f;
					}
					num8 *= Mathf.Lerp(m_SizeRates[(int)num10], m_SizeRates[(int)num11], t2);
				}
				if (m_bCenterAlign)
				{
					array3[m * 2] = point3.basePosition - vector * (num8 * 0.5f);
					array3[m * 2 + 1] = point3.basePosition + vector * (num8 * 0.5f);
				}
				else
				{
					array3[m * 2] = point3.basePosition - vector * num8;
					array3[m * 2 + 1] = point3.basePosition;
				}
				int num12 = ((!m_bInterpolation) ? m_nFadeTailCount : (m_nFadeTailCount * m_nSubdivisions));
				int num13 = ((!m_bInterpolation) ? m_nFadeHeadCount : (m_nFadeHeadCount * m_nSubdivisions));
				if (0 < num12 && m <= num12)
				{
					color.a = color.a * (float)m / (float)num12;
				}
				if (0 < num13 && list4.Count - (m + 1) <= num13)
				{
					color.a = color.a * (float)(list4.Count - (m + 1)) / (float)num13;
				}
				array6[m * 2] = (array6[m * 2 + 1] = color);
				float num14 = (float)m / (float)list4.Count;
				array4[m * 2] = new Vector2((!m_UvFlipHorizontal) ? num14 : (1f - num14), m_UvFlipVirtical ? 1 : 0);
				array4[m * 2 + 1] = new Vector2((!m_UvFlipHorizontal) ? num14 : (1f - num14), (!m_UvFlipVirtical) ? 1 : 0);
				if (m > 0)
				{
					array5[(m - 1) * 6] = m * 2 - 2;
					array5[(m - 1) * 6 + 1] = m * 2 - 1;
					array5[(m - 1) * 6 + 2] = m * 2;
					array5[(m - 1) * 6 + 3] = m * 2 + 1;
					array5[(m - 1) * 6 + 4] = m * 2;
					array5[(m - 1) * 6 + 5] = m * 2 - 1;
				}
			}
			m_TrailMesh.Clear();
			m_TrailMesh.vertices = array3;
			m_TrailMesh.colors = array6;
			m_TrailMesh.uv = array4;
			m_TrailMesh.triangles = array5;
		}
		else
		{
			m_TrailMesh.Clear();
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime /= fSpeedRate;
		m_fEmitTime /= fSpeedRate;
		m_fLifeTime /= fSpeedRate;
	}
}
