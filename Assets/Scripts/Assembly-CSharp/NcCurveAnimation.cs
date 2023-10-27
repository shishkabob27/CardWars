using System;
using UnityEngine;
using System.Collections.Generic;

public class NcCurveAnimation : NcEffectAniBehaviour
{
	[Serializable]
	public class NcInfoCurve
	{
		public enum APPLY_TYPE
		{
			NONE = 0,
			POSITION = 1,
			ROTATION = 2,
			SCALE = 3,
			MATERIAL_COLOR = 4,
			TEXTUREUV = 5,
			MESH_COLOR = 6,
		}

		public bool m_bEnabled;
		public string m_CurveName;
		public AnimationCurve m_AniCurve;
		public APPLY_TYPE m_ApplyType;
		public bool[] m_bApplyOption;
		public bool m_bRecursively;
		public float m_fValueScale;
		public Vector4 m_FromColor;
		public Vector4 m_ToColor;
		public int m_nTag;
		public int m_nSortGroup;
		public Vector4 m_OriginalValue;
		public Vector4 m_BeforeValue;
		public Vector4[] m_ChildOriginalColorValues;
		public Vector4[] m_ChildBeforeColorValues;
	}

	[SerializeField]
	public List<NcCurveAnimation.NcInfoCurve> m_CurveInfoList;
	public float m_fDelayTime;
	public float m_fDurationTime;
	public bool m_bAutoDestruct;
}
