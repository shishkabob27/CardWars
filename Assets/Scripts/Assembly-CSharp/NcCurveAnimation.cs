using System;
using System.Collections.Generic;
using UnityEngine;

public class NcCurveAnimation : NcEffectAniBehaviour
{
	private class NcComparerCurve : IComparer<NcInfoCurve>
	{
		protected static float m_fEqualRange = 0.03f;

		protected static float m_fHDiv = 5f;

		public int Compare(NcInfoCurve a, NcInfoCurve b)
		{
			float num = a.m_AniCurve.Evaluate(m_fEqualRange / m_fHDiv) - b.m_AniCurve.Evaluate(m_fEqualRange / m_fHDiv);
			if (Mathf.Abs(num) < m_fEqualRange)
			{
				num = b.m_AniCurve.Evaluate(1f - m_fEqualRange / m_fHDiv) - a.m_AniCurve.Evaluate(1f - m_fEqualRange / m_fHDiv);
				if (Mathf.Abs(num) < m_fEqualRange)
				{
					return 0;
				}
			}
			return (int)(num * 1000f);
		}

		public static int GetSortGroup(NcInfoCurve info)
		{
			float num = info.m_AniCurve.Evaluate(m_fEqualRange / m_fHDiv);
			if (num < 0f - m_fEqualRange)
			{
				return 1;
			}
			if (m_fEqualRange < num)
			{
				return 3;
			}
			return 2;
		}
	}

	[Serializable]
	public class NcInfoCurve
	{
		public enum APPLY_TYPE
		{
			NONE,
			POSITION,
			ROTATION,
			SCALE,
			MATERIAL_COLOR,
			TEXTUREUV,
			MESH_COLOR
		}

		protected const float m_fOverDraw = 0.2f;

		public bool m_bEnabled = true;

		public string m_CurveName = string.Empty;

		public AnimationCurve m_AniCurve = new AnimationCurve();

		public static string[] m_TypeName = new string[7] { "None", "Position", "Rotation", "Scale", "MaterialColor", "TextureUV", "MeshColor" };

		public APPLY_TYPE m_ApplyType = APPLY_TYPE.POSITION;

		public bool[] m_bApplyOption = new bool[4] { false, true, false, false };

		public bool m_bRecursively;

		public float m_fValueScale = 1f;

		public Vector4 m_FromColor = Color.white;

		public Vector4 m_ToColor = Color.white;

		public int m_nTag;

		public int m_nSortGroup;

		public Vector4 m_OriginalValue;

		public Vector4 m_BeforeValue;

		public Vector4[] m_ChildOriginalColorValues;

		public Vector4[] m_ChildBeforeColorValues;

		public bool IsEnabled()
		{
			return m_bEnabled;
		}

		public void SetEnabled(bool bEnable)
		{
			m_bEnabled = bEnable;
		}

		public string GetCurveName()
		{
			return m_CurveName;
		}

		public NcInfoCurve GetClone()
		{
			NcInfoCurve ncInfoCurve = new NcInfoCurve();
			ncInfoCurve.m_AniCurve = new AnimationCurve(m_AniCurve.keys);
			ncInfoCurve.m_AniCurve.postWrapMode = m_AniCurve.postWrapMode;
			ncInfoCurve.m_AniCurve.preWrapMode = m_AniCurve.preWrapMode;
			ncInfoCurve.m_bEnabled = m_bEnabled;
			ncInfoCurve.m_CurveName = m_CurveName;
			ncInfoCurve.m_ApplyType = m_ApplyType;
			Array.Copy(m_bApplyOption, ncInfoCurve.m_bApplyOption, m_bApplyOption.Length);
			ncInfoCurve.m_fValueScale = m_fValueScale;
			ncInfoCurve.m_bRecursively = m_bRecursively;
			ncInfoCurve.m_FromColor = m_FromColor;
			ncInfoCurve.m_ToColor = m_ToColor;
			ncInfoCurve.m_nTag = m_nTag;
			ncInfoCurve.m_nSortGroup = m_nSortGroup;
			return ncInfoCurve;
		}

		public void CopyTo(NcInfoCurve target)
		{
			target.m_AniCurve = new AnimationCurve(m_AniCurve.keys);
			target.m_AniCurve.postWrapMode = m_AniCurve.postWrapMode;
			target.m_AniCurve.preWrapMode = m_AniCurve.preWrapMode;
			target.m_bEnabled = m_bEnabled;
			target.m_ApplyType = m_ApplyType;
			Array.Copy(m_bApplyOption, target.m_bApplyOption, m_bApplyOption.Length);
			target.m_fValueScale = m_fValueScale;
			target.m_bRecursively = m_bRecursively;
			target.m_FromColor = m_FromColor;
			target.m_ToColor = m_ToColor;
			target.m_nTag = m_nTag;
			target.m_nSortGroup = m_nSortGroup;
		}

		public int GetValueCount()
		{
			switch (m_ApplyType)
			{
			case APPLY_TYPE.POSITION:
				return 4;
			case APPLY_TYPE.ROTATION:
				return 4;
			case APPLY_TYPE.SCALE:
				return 3;
			case APPLY_TYPE.MATERIAL_COLOR:
				return 4;
			case APPLY_TYPE.TEXTUREUV:
				return 2;
			case APPLY_TYPE.MESH_COLOR:
				return 4;
			default:
				return 0;
			}
		}

		public string GetValueName(int nIndex)
		{
			string[] array;
			switch (m_ApplyType)
			{
			case APPLY_TYPE.POSITION:
			case APPLY_TYPE.ROTATION:
				array = new string[4] { "X", "Y", "Z", "World" };
				break;
			case APPLY_TYPE.SCALE:
				array = new string[4]
				{
					"X",
					"Y",
					"Z",
					string.Empty
				};
				break;
			case APPLY_TYPE.MATERIAL_COLOR:
				array = new string[4] { "R", "G", "B", "A" };
				break;
			case APPLY_TYPE.TEXTUREUV:
				array = new string[4]
				{
					"X",
					"Y",
					string.Empty,
					string.Empty
				};
				break;
			case APPLY_TYPE.MESH_COLOR:
				array = new string[4] { "R", "G", "B", "A" };
				break;
			default:
				array = new string[4]
				{
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty
				};
				break;
			}
			return array[nIndex];
		}

		public void SetDefaultValueScale()
		{
			switch (m_ApplyType)
			{
			case APPLY_TYPE.POSITION:
				m_fValueScale = 1f;
				break;
			case APPLY_TYPE.ROTATION:
				m_fValueScale = 360f;
				break;
			case APPLY_TYPE.SCALE:
				m_fValueScale = 1f;
				break;
			case APPLY_TYPE.MATERIAL_COLOR:
				break;
			case APPLY_TYPE.TEXTUREUV:
				m_fValueScale = 10f;
				break;
			case APPLY_TYPE.MESH_COLOR:
				break;
			case APPLY_TYPE.NONE:
				break;
			}
		}

		public Rect GetFixedDrawRange()
		{
			return new Rect(-0.2f, -1.2f, 1.4f, 2.4f);
		}

		public Rect GetVariableDrawRange()
		{
			Rect result = default(Rect);
			for (int i = 0; i < m_AniCurve.keys.Length; i++)
			{
				result.yMin = Mathf.Min(result.yMin, m_AniCurve[i].value);
				result.yMax = Mathf.Max(result.yMax, m_AniCurve[i].value);
			}
			int num = 20;
			for (int j = 0; j < num; j++)
			{
				float b = m_AniCurve.Evaluate((float)j / (float)num);
				result.yMin = Mathf.Min(result.yMin, b);
				result.yMax = Mathf.Max(result.yMax, b);
			}
			result.xMin = 0f;
			result.xMax = 1f;
			result.xMin -= result.width * 0.2f;
			result.xMax += result.width * 0.2f;
			result.yMin -= result.height * 0.2f;
			result.yMax += result.height * 0.2f;
			return result;
		}

		public Rect GetEditRange()
		{
			return new Rect(0f, -1f, 1f, 2f);
		}

		public void NormalizeCurveTime()
		{
			int num = 0;
			while (num < m_AniCurve.keys.Length)
			{
				Keyframe keyframe = m_AniCurve[num];
				float a = Mathf.Max(0f, keyframe.time);
				float num2 = Mathf.Min(1f, Mathf.Max(a, keyframe.time));
				if (num2 != keyframe.time)
				{
					Keyframe key = new Keyframe(num2, keyframe.value, keyframe.inTangent, keyframe.outTangent);
					m_AniCurve.RemoveKey(num);
					num = 0;
					m_AniCurve.AddKey(key);
				}
				else
				{
					num++;
				}
			}
		}
	}

	[SerializeField]
	public List<NcInfoCurve> m_CurveInfoList;

	public float m_fDelayTime;

	public float m_fDurationTime = 0.6f;

	public bool m_bAutoDestruct = true;

	protected float m_fStartTime;

	protected float m_fElapsedRate;

	protected Transform m_Transform;

	protected string m_ColorName;

	protected Material m_MainMaterial;

	protected string[] m_ChildColorNames;

	protected Renderer[] m_ChildRenderers;

	protected MeshFilter m_MainMeshFilter;

	protected MeshFilter[] m_ChildMeshFilters;

	protected NcUvAnimation m_NcUvAnimation;

	public override int GetAnimationState()
	{
		if (!base.enabled || !NcEffectBehaviour.IsActive(base.gameObject))
		{
			return -1;
		}
		if (0f < m_fDurationTime && (m_fStartTime == 0f || !IsEndAnimation()))
		{
			return 1;
		}
		return 0;
	}

	public override void ResetAnimation()
	{
		base.ResetAnimation();
		InitAnimation();
		UpdateAnimation(0f);
	}

	public float GetRepeatedRate()
	{
		return m_fElapsedRate;
	}

	private void Awake()
	{
	}

	private void Start()
	{
		m_fStartTime = NcEffectBehaviour.GetEngineTime();
		InitAnimation();
		if (0f < m_fDelayTime)
		{
			if ((bool)GetComponent<Renderer>())
			{
				GetComponent<Renderer>().enabled = false;
			}
		}
		else
		{
			InitAnimationTimer();
			UpdateAnimation(0f);
		}
	}

	private void LateUpdate()
	{
		if (m_fStartTime == 0f)
		{
			return;
		}
		if (m_fDelayTime != 0f)
		{
			if (NcEffectBehaviour.GetEngineTime() < m_fStartTime + m_fDelayTime)
			{
				return;
			}
			m_fDelayTime = 0f;
			InitAnimationTimer();
			if ((bool)GetComponent<Renderer>())
			{
				GetComponent<Renderer>().enabled = true;
			}
		}
		float time = m_Timer.GetTime();
		float fElapsedRate = time;
		if (m_fDurationTime != 0f)
		{
			fElapsedRate = time / m_fDurationTime;
		}
		UpdateAnimation(fElapsedRate);
	}

	private void InitAnimation()
	{
		m_fElapsedRate = 0f;
		m_Transform = base.transform;
		foreach (NcInfoCurve curveInfo in m_CurveInfoList)
		{
			if (!curveInfo.m_bEnabled)
			{
				continue;
			}
			switch (curveInfo.m_ApplyType)
			{
			case NcInfoCurve.APPLY_TYPE.POSITION:
				curveInfo.m_OriginalValue = Vector4.zero;
				curveInfo.m_BeforeValue = curveInfo.m_OriginalValue;
				break;
			case NcInfoCurve.APPLY_TYPE.ROTATION:
				curveInfo.m_OriginalValue = Vector4.zero;
				curveInfo.m_BeforeValue = curveInfo.m_OriginalValue;
				break;
			case NcInfoCurve.APPLY_TYPE.SCALE:
				curveInfo.m_OriginalValue = m_Transform.localScale;
				curveInfo.m_BeforeValue = curveInfo.m_OriginalValue;
				break;
			case NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR:
				if (curveInfo.m_bRecursively)
				{
					m_ChildRenderers = base.transform.GetComponentsInChildren<Renderer>(true);
					m_ChildColorNames = new string[m_ChildRenderers.Length];
					curveInfo.m_ChildOriginalColorValues = new Vector4[m_ChildRenderers.Length];
					curveInfo.m_ChildBeforeColorValues = new Vector4[m_ChildRenderers.Length];
					for (int j = 0; j < m_ChildRenderers.Length; j++)
					{
						Renderer renderer = m_ChildRenderers[j];
						m_ChildColorNames[j] = Ng_GetMaterialColorName(renderer.sharedMaterial);
						if (m_ChildColorNames[j] != null)
						{
							curveInfo.m_ChildOriginalColorValues[j] = renderer.material.GetColor(m_ChildColorNames[j]);
						}
						curveInfo.m_ChildBeforeColorValues[j] = Vector4.zero;
					}
				}
				else if (GetComponent<Renderer>() != null)
				{
					m_ColorName = Ng_GetMaterialColorName(GetComponent<Renderer>().sharedMaterial);
					if (m_ColorName != null)
					{
						curveInfo.m_OriginalValue = GetComponent<Renderer>().sharedMaterial.GetColor(m_ColorName);
					}
					curveInfo.m_BeforeValue = Vector4.zero;
				}
				break;
			case NcInfoCurve.APPLY_TYPE.TEXTUREUV:
				if (m_NcUvAnimation == null)
				{
					m_NcUvAnimation = GetComponent<NcUvAnimation>();
				}
				if ((bool)m_NcUvAnimation)
				{
					curveInfo.m_OriginalValue = new Vector4(m_NcUvAnimation.m_fScrollSpeedX, m_NcUvAnimation.m_fScrollSpeedY, 0f, 0f);
				}
				curveInfo.m_BeforeValue = curveInfo.m_OriginalValue;
				break;
			case NcInfoCurve.APPLY_TYPE.MESH_COLOR:
			{
				float t = curveInfo.m_AniCurve.Evaluate(0f);
				Color tarColor = Color.Lerp(curveInfo.m_FromColor, curveInfo.m_ToColor, t);
				if (curveInfo.m_bRecursively)
				{
					m_ChildMeshFilters = base.transform.GetComponentsInChildren<MeshFilter>(true);
					if (m_ChildMeshFilters != null && m_ChildMeshFilters.Length >= 0)
					{
						for (int i = 0; i < m_ChildMeshFilters.Length; i++)
						{
							ChangeMeshColor(m_ChildMeshFilters[i], tarColor);
						}
					}
				}
				else
				{
					m_MainMeshFilter = GetComponent<MeshFilter>();
					ChangeMeshColor(m_MainMeshFilter, tarColor);
				}
				break;
			}
			}
		}
	}

	private void UpdateAnimation(float fElapsedRate)
	{
		m_fElapsedRate = fElapsedRate;
		foreach (NcInfoCurve curveInfo in m_CurveInfoList)
		{
			if (!curveInfo.m_bEnabled)
			{
				continue;
			}
			float num = curveInfo.m_AniCurve.Evaluate(m_fElapsedRate);
			if (curveInfo.m_ApplyType != NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR && curveInfo.m_ApplyType != NcInfoCurve.APPLY_TYPE.MESH_COLOR)
			{
				num *= curveInfo.m_fValueScale;
			}
			switch (curveInfo.m_ApplyType)
			{
			case NcInfoCurve.APPLY_TYPE.POSITION:
				if (curveInfo.m_bApplyOption[3])
				{
					m_Transform.position += new Vector3(GetNextValue(curveInfo, 0, num), GetNextValue(curveInfo, 1, num), GetNextValue(curveInfo, 2, num));
				}
				else
				{
					m_Transform.localPosition += new Vector3(GetNextValue(curveInfo, 0, num), GetNextValue(curveInfo, 1, num), GetNextValue(curveInfo, 2, num));
				}
				break;
			case NcInfoCurve.APPLY_TYPE.ROTATION:
				if (curveInfo.m_bApplyOption[3])
				{
					m_Transform.rotation *= Quaternion.Euler(GetNextValue(curveInfo, 0, num), GetNextValue(curveInfo, 1, num), GetNextValue(curveInfo, 2, num));
				}
				else
				{
					m_Transform.localRotation *= Quaternion.Euler(GetNextValue(curveInfo, 0, num), GetNextValue(curveInfo, 1, num), GetNextValue(curveInfo, 2, num));
				}
				break;
			case NcInfoCurve.APPLY_TYPE.SCALE:
				m_Transform.localScale += new Vector3(GetNextScale(curveInfo, 0, num), GetNextScale(curveInfo, 1, num), GetNextScale(curveInfo, 2, num));
				break;
			case NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR:
				if (curveInfo.m_bRecursively)
				{
					if (m_ChildColorNames == null || m_ChildColorNames.Length < 0)
					{
						break;
					}
					for (int j = 0; j < m_ChildColorNames.Length; j++)
					{
						if (m_ChildColorNames[j] != null && m_ChildRenderers[j] != null)
						{
							SetChildMaterialColor(curveInfo, num, j);
						}
					}
				}
				else if (GetComponent<Renderer>() != null && m_ColorName != null)
				{
					if (m_MainMaterial == null)
					{
						m_MainMaterial = GetComponent<Renderer>().material;
						AddRuntimeMaterial(m_MainMaterial);
					}
					Color color = curveInfo.m_ToColor - curveInfo.m_OriginalValue;
					Color color2 = m_MainMaterial.GetColor(m_ColorName);
					for (int k = 0; k < 4; k++)
					{
						int index;
						int index2 = (index = k);
						float num2 = color2[index];
						color2[index2] = num2 + GetNextValue(curveInfo, k, color[k] * num);
					}
					m_MainMaterial.SetColor(m_ColorName, color2);
				}
				break;
			case NcInfoCurve.APPLY_TYPE.TEXTUREUV:
				if ((bool)m_NcUvAnimation)
				{
					m_NcUvAnimation.m_fScrollSpeedX += GetNextValue(curveInfo, 0, num);
					m_NcUvAnimation.m_fScrollSpeedY += GetNextValue(curveInfo, 1, num);
				}
				break;
			case NcInfoCurve.APPLY_TYPE.MESH_COLOR:
			{
				Color tarColor = Color.Lerp(curveInfo.m_FromColor, curveInfo.m_ToColor, num);
				if (curveInfo.m_bRecursively)
				{
					if (m_ChildMeshFilters != null && m_ChildMeshFilters.Length >= 0)
					{
						for (int i = 0; i < m_ChildMeshFilters.Length; i++)
						{
							ChangeMeshColor(m_ChildMeshFilters[i], tarColor);
						}
					}
				}
				else
				{
					ChangeMeshColor(m_MainMeshFilter, tarColor);
				}
				break;
			}
			}
		}
		if (m_fDurationTime != 0f && 1f < m_fElapsedRate)
		{
			if (!IsEndAnimation())
			{
				OnEndAnimation();
			}
			if (m_bAutoDestruct)
			{
				UnityEngine.Object.DestroyObject(base.gameObject);
			}
		}
	}

	private void ChangeMeshColor(MeshFilter mFilter, Color tarColor)
	{
		if (!(mFilter == null) && !(mFilter.mesh == null))
		{
			Color[] colors = mFilter.mesh.colors;
			for (int i = 0; i < colors.Length; i++)
			{
				colors[i] = tarColor;
			}
			mFilter.mesh.colors = colors;
		}
	}

	private void SetChildMaterialColor(NcInfoCurve curveInfo, float fValue, int arrayIndex)
	{
		Color color = curveInfo.m_ToColor - curveInfo.m_ChildOriginalColorValues[arrayIndex];
		Color color2 = m_ChildRenderers[arrayIndex].material.GetColor(m_ChildColorNames[arrayIndex]);
		for (int i = 0; i < 4; i++)
		{
			int index;
			int index2 = (index = i);
			float num = color2[index];
			color2[index2] = num + GetChildNextColorValue(curveInfo, i, color[i] * fValue, arrayIndex);
		}
		m_ChildRenderers[arrayIndex].material.SetColor(m_ChildColorNames[arrayIndex], color2);
	}

	private float GetChildNextColorValue(NcInfoCurve curveInfo, int nIndex, float fValue, int arrayIndex)
	{
		if (curveInfo.m_bApplyOption[nIndex])
		{
			float result = fValue - curveInfo.m_ChildBeforeColorValues[arrayIndex][nIndex];
			curveInfo.m_ChildBeforeColorValues[arrayIndex][nIndex] = fValue;
			return result;
		}
		return 0f;
	}

	private float GetNextValue(NcInfoCurve curveInfo, int nIndex, float fValue)
	{
		if (curveInfo.m_bApplyOption[nIndex])
		{
			float result = fValue - curveInfo.m_BeforeValue[nIndex];
			curveInfo.m_BeforeValue[nIndex] = fValue;
			return result;
		}
		return 0f;
	}

	private float GetNextScale(NcInfoCurve curveInfo, int nIndex, float fValue)
	{
		if (curveInfo.m_bApplyOption[nIndex])
		{
			float num = curveInfo.m_OriginalValue[nIndex] * (1f + fValue);
			float result = num - curveInfo.m_BeforeValue[nIndex];
			curveInfo.m_BeforeValue[nIndex] = num;
			return result;
		}
		return 0f;
	}

	public float GetElapsedRate()
	{
		return m_fElapsedRate;
	}

	public void CopyTo(NcCurveAnimation target, bool bCurveOnly)
	{
		target.m_CurveInfoList = new List<NcInfoCurve>();
		foreach (NcInfoCurve curveInfo in m_CurveInfoList)
		{
			target.m_CurveInfoList.Add(curveInfo.GetClone());
		}
		if (!bCurveOnly)
		{
			target.m_fDelayTime = m_fDelayTime;
			target.m_fDurationTime = m_fDurationTime;
		}
	}

	public void AppendTo(NcCurveAnimation target, bool bCurveOnly)
	{
		if (target.m_CurveInfoList == null)
		{
			target.m_CurveInfoList = new List<NcInfoCurve>();
		}
		foreach (NcInfoCurve curveInfo in m_CurveInfoList)
		{
			target.m_CurveInfoList.Add(curveInfo.GetClone());
		}
		if (!bCurveOnly)
		{
			target.m_fDelayTime = m_fDelayTime;
			target.m_fDurationTime = m_fDurationTime;
		}
	}

	public NcInfoCurve GetCurveInfo(int nIndex)
	{
		if (m_CurveInfoList == null || nIndex < 0 || m_CurveInfoList.Count <= nIndex)
		{
			return null;
		}
		return m_CurveInfoList[nIndex];
	}

	public NcInfoCurve GetCurveInfo(string curveName)
	{
		if (m_CurveInfoList == null)
		{
			return null;
		}
		foreach (NcInfoCurve curveInfo in m_CurveInfoList)
		{
			if (curveInfo.m_CurveName == curveName)
			{
				return curveInfo;
			}
		}
		return null;
	}

	public NcInfoCurve SetCurveInfo(int nIndex, NcInfoCurve newInfo)
	{
		if (m_CurveInfoList == null || nIndex < 0 || m_CurveInfoList.Count <= nIndex)
		{
			return null;
		}
		NcInfoCurve result = m_CurveInfoList[nIndex];
		m_CurveInfoList[nIndex] = newInfo;
		return result;
	}

	public int AddCurveInfo()
	{
		NcInfoCurve ncInfoCurve = new NcInfoCurve();
		ncInfoCurve.m_AniCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		ncInfoCurve.m_ToColor = Color.white;
		ncInfoCurve.m_ToColor.w = 0f;
		if (m_CurveInfoList == null)
		{
			m_CurveInfoList = new List<NcInfoCurve>();
		}
		m_CurveInfoList.Add(ncInfoCurve);
		return m_CurveInfoList.Count - 1;
	}

	public int AddCurveInfo(NcInfoCurve addCurveInfo)
	{
		if (m_CurveInfoList == null)
		{
			m_CurveInfoList = new List<NcInfoCurve>();
		}
		m_CurveInfoList.Add(addCurveInfo.GetClone());
		return m_CurveInfoList.Count - 1;
	}

	public void DeleteCurveInfo(int nIndex)
	{
		if (m_CurveInfoList != null && nIndex >= 0 && m_CurveInfoList.Count > nIndex)
		{
			m_CurveInfoList.Remove(m_CurveInfoList[nIndex]);
		}
	}

	public void ClearAllCurveInfo()
	{
		if (m_CurveInfoList != null)
		{
			m_CurveInfoList.Clear();
		}
	}

	public int GetCurveInfoCount()
	{
		if (m_CurveInfoList == null)
		{
			return 0;
		}
		return m_CurveInfoList.Count;
	}

	public void SortCurveInfo()
	{
		if (m_CurveInfoList == null)
		{
			return;
		}
		m_CurveInfoList.Sort(new NcComparerCurve());
		foreach (NcInfoCurve curveInfo in m_CurveInfoList)
		{
			curveInfo.m_nSortGroup = NcComparerCurve.GetSortGroup(curveInfo);
		}
	}

	public bool CheckInvalidOption()
	{
		bool result = false;
		for (int i = 0; i < m_CurveInfoList.Count; i++)
		{
			if (CheckInvalidOption(i))
			{
				result = true;
			}
		}
		return result;
	}

	public bool CheckInvalidOption(int nSrcIndex)
	{
		NcInfoCurve curveInfo = GetCurveInfo(nSrcIndex);
		if (curveInfo == null)
		{
			return false;
		}
		if (curveInfo.m_ApplyType != NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR && curveInfo.m_ApplyType != NcInfoCurve.APPLY_TYPE.SCALE && curveInfo.m_ApplyType != NcInfoCurve.APPLY_TYPE.TEXTUREUV)
		{
			return false;
		}
		return false;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime /= fSpeedRate;
		m_fDurationTime /= fSpeedRate;
	}

	public static string Ng_GetMaterialColorName(Material mat)
	{
		string[] array = new string[3] { "_Color", "_TintColor", "_EmisColor" };
		if (mat != null)
		{
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (mat.HasProperty(text))
				{
					return text;
				}
			}
		}
		return null;
	}
}
