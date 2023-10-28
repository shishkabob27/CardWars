using System;
using System.Collections.Generic;
using UnityEngine;

public class NcSpriteFactory : NcEffectBehaviour
{
	[Serializable]
	public class NcFrameInfo
	{
		public int m_nFrameIndex;

		public bool m_bEmptyFrame;

		public int m_nTexWidth;

		public int m_nTexHeight;

		public Rect m_TextureUvOffset;

		public Rect m_FrameUvOffset;

		public Vector2 m_FrameScale;

		public Vector2 m_scaleFactor;
	}

	[Serializable]
	[SerializeField]
	public class NcSpriteNode
	{
		public bool m_bIncludedAtlas = true;

		public string m_TextureGUID = string.Empty;

		public string m_TextureName = string.Empty;

		public float m_fMaxTextureAlpha = 1f;

		public string m_SpriteName = string.Empty;

		public NcFrameInfo[] m_FrameInfos;

		public int m_nTilingX = 1;

		public int m_nTilingY = 1;

		public int m_nStartFrame;

		public int m_nFrameCount = 1;

		public bool m_bLoop;

		public int m_nLoopStartFrame;

		public int m_nLoopFrameCount;

		public int m_nLoopingCount;

		public float m_fFps = 20f;

		public float m_fTime;

		public int m_nNextSpriteIndex = -1;

		public int m_nTestMode;

		public float m_fTestSpeed = 1f;

		public bool m_bEffectInstantiate = true;

		public GameObject m_EffectPrefab;

		public int m_nEffectFrame;

		public bool m_bEffectOnlyFirst = true;

		public bool m_bEffectDetach = true;

		public float m_fEffectSpeed = 1f;

		public float m_fEffectScale = 1f;

		public Vector3 m_EffectPos = Vector3.zero;

		public Vector3 m_EffectRot = Vector3.zero;

		public AudioClip m_AudioClip;

		public int m_nSoundFrame;

		public bool m_bSoundOnlyFirst = true;

		public bool m_bSoundLoop;

		public float m_fSoundVolume = 1f;

		public float m_fSoundPitch = 1f;

		public NcSpriteNode GetClone()
		{
			return null;
		}

		public int GetStartFrame()
		{
			if (m_FrameInfos == null || m_FrameInfos.Length == 0)
			{
				return 0;
			}
			return m_FrameInfos[0].m_nFrameIndex;
		}

		public void SetEmpty()
		{
			m_FrameInfos = null;
			m_TextureGUID = string.Empty;
		}

		public bool IsEmptyTexture()
		{
			return m_TextureGUID == string.Empty;
		}

		public bool IsUnused()
		{
			return !m_bIncludedAtlas;
		}
	}

	[SerializeField]
	public enum MESH_TYPE
	{
		BuiltIn_Plane,
		BuiltIn_TwosidePlane
	}

	public enum ALIGN_TYPE
	{
		TOP,
		CENTER,
		BOTTOM
	}

	public enum SPRITE_TYPE
	{
		NcSpriteTexture,
		NcSpriteAnimation
	}

	public enum SHOW_TYPE
	{
		NONE,
		ALL,
		SPRITE,
		ANIMATION,
		EFFECT
	}

	public SPRITE_TYPE m_SpriteType;

	public List<NcSpriteNode> m_SpriteList;

	public int m_nCurrentIndex;

	public int m_nMaxAtlasTextureSize = 2048;

	public bool m_bNeedRebuild = true;

	public int m_nBuildStartIndex;

	public bool m_bTrimBlack = true;

	public bool m_bTrimAlpha = true;

	public float m_fUvScale = 1f;

	public float m_fTextureRatio = 1f;

	public GameObject m_CurrentEffect;

	public NcAttachSound m_CurrentSound;

	protected bool m_bEndSprite = true;

	public SHOW_TYPE m_ShowType = SHOW_TYPE.SPRITE;

	public bool m_bShowEffect = true;

	public bool m_bTestMode = true;

	public bool m_bSequenceMode;

	protected bool m_bbInstance;

	public bool IsUnused(int nNodeIndex)
	{
		return m_SpriteList[nNodeIndex].IsUnused() || nNodeIndex < m_nBuildStartIndex;
	}

	public NcSpriteNode GetSpriteNode(int nIndex)
	{
		if (m_SpriteList == null || nIndex < 0 || m_SpriteList.Count <= nIndex)
		{
			return null;
		}
		return m_SpriteList[nIndex];
	}

	public NcSpriteNode GetSpriteNode(string spriteName)
	{
		if (m_SpriteList == null)
		{
			return null;
		}
		foreach (NcSpriteNode sprite in m_SpriteList)
		{
			if (sprite.m_SpriteName == spriteName)
			{
				return sprite;
			}
		}
		return null;
	}

	public int GetSpriteNodeIndex(string spriteName)
	{
		if (m_SpriteList == null)
		{
			return -1;
		}
		for (int i = 0; i < m_SpriteList.Count; i++)
		{
			if (m_SpriteList[i].m_SpriteName == spriteName)
			{
				return i;
			}
		}
		return -1;
	}

	public NcSpriteNode SetSpriteNode(int nIndex, NcSpriteNode newInfo)
	{
		if (m_SpriteList == null || nIndex < 0 || m_SpriteList.Count <= nIndex)
		{
			return null;
		}
		NcSpriteNode result = m_SpriteList[nIndex];
		m_SpriteList[nIndex] = newInfo;
		return result;
	}

	public int AddSpriteNode()
	{
		NcSpriteNode item = new NcSpriteNode();
		if (m_SpriteList == null)
		{
			m_SpriteList = new List<NcSpriteNode>();
		}
		m_SpriteList.Add(item);
		return m_SpriteList.Count - 1;
	}

	public int AddSpriteNode(NcSpriteNode addSpriteNode)
	{
		if (m_SpriteList == null)
		{
			m_SpriteList = new List<NcSpriteNode>();
		}
		m_SpriteList.Add(addSpriteNode.GetClone());
		m_bNeedRebuild = true;
		return m_SpriteList.Count - 1;
	}

	public void DeleteSpriteNode(int nIndex)
	{
		if (m_SpriteList != null && nIndex >= 0 && m_SpriteList.Count > nIndex)
		{
			m_bNeedRebuild = true;
			m_SpriteList.Remove(m_SpriteList[nIndex]);
		}
	}

	public void MoveSpriteNode(int nSrcIndex, int nTarIndex)
	{
		NcSpriteNode item = m_SpriteList[nSrcIndex];
		m_SpriteList.Remove(item);
		m_SpriteList.Insert(nTarIndex, item);
	}

	public void ClearAllSpriteNode()
	{
		if (m_SpriteList != null)
		{
			m_bNeedRebuild = true;
			m_SpriteList.Clear();
		}
	}

	public int GetSpriteNodeCount()
	{
		if (m_SpriteList == null)
		{
			return 0;
		}
		return m_SpriteList.Count;
	}

	public NcSpriteNode GetCurrentSpriteNode()
	{
		if (m_SpriteList == null || m_SpriteList.Count <= m_nCurrentIndex)
		{
			return null;
		}
		return m_SpriteList[m_nCurrentIndex];
	}

	public Rect GetSpriteUvRect(int nStriteIndex, int nFrameIndex)
	{
		if (m_SpriteList.Count <= nStriteIndex || m_SpriteList[nStriteIndex].m_FrameInfos == null || m_SpriteList[nStriteIndex].m_FrameInfos.Length <= nFrameIndex)
		{
			return new Rect(0f, 0f, 0f, 0f);
		}
		return m_SpriteList[nStriteIndex].m_FrameInfos[nFrameIndex].m_TextureUvOffset;
	}

	public bool IsValidFactory()
	{
		if (m_bNeedRebuild)
		{
			return false;
		}
		return true;
	}

	private void Awake()
	{
		m_bbInstance = true;
	}

	public NcEffectBehaviour SetSprite(int nNodeIndex)
	{
		return SetSprite(nNodeIndex, true);
	}

	public NcEffectBehaviour SetSprite(string spriteName)
	{
		if (m_SpriteList == null)
		{
			return null;
		}
		int num = 0;
		foreach (NcSpriteNode sprite in m_SpriteList)
		{
			if (sprite.m_SpriteName == spriteName)
			{
				return SetSprite(num, true);
			}
			num++;
		}
		return null;
	}

	public NcEffectBehaviour SetSprite(int nNodeIndex, bool bRunImmediate)
	{
		if (m_SpriteList == null || nNodeIndex < 0 || m_SpriteList.Count <= nNodeIndex)
		{
			return null;
		}
		if (bRunImmediate)
		{
			OnChangingSprite(m_nCurrentIndex, nNodeIndex);
		}
		m_nCurrentIndex = nNodeIndex;
		NcSpriteAnimation component = GetComponent<NcSpriteAnimation>();
		if (component != null)
		{
			component.SetSpriteFactoryIndex(nNodeIndex, false);
			if (bRunImmediate)
			{
				component.ResetAnimation();
			}
		}
		NcSpriteTexture component2 = GetComponent<NcSpriteTexture>();
		if (component2 != null)
		{
			component2.SetSpriteFactoryIndex(nNodeIndex, -1, false);
			if (bRunImmediate)
			{
				CreateEffectObject();
			}
		}
		if (component != null)
		{
			return component;
		}
		if (component != null)
		{
			return component2;
		}
		return null;
	}

	public int GetCurrentSpriteIndex()
	{
		return m_nCurrentIndex;
	}

	public bool IsEndSprite()
	{
		if (m_SpriteList == null || m_nCurrentIndex < 0 || m_SpriteList.Count <= m_nCurrentIndex)
		{
			return true;
		}
		if (IsUnused(m_nCurrentIndex) || m_SpriteList[m_nCurrentIndex].IsEmptyTexture())
		{
			return true;
		}
		return m_bEndSprite;
	}

	private void CreateEffectObject()
	{
		if (!m_bbInstance || !m_bShowEffect)
		{
			return;
		}
		DestroyEffectObject();
		if (base.transform.parent != null)
		{
			base.transform.parent.SendMessage("OnSpriteListEffectFrame", m_SpriteList[m_nCurrentIndex], SendMessageOptions.DontRequireReceiver);
		}
		if (m_SpriteList[m_nCurrentIndex].m_bEffectInstantiate)
		{
			m_CurrentEffect = CreateSpriteEffect(m_nCurrentIndex, base.transform);
			if (base.transform.parent != null)
			{
				base.transform.parent.SendMessage("OnSpriteListEffectInstance", m_CurrentEffect, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public GameObject CreateSpriteEffect(int nSrcSpriteIndex, Transform parentTrans)
	{
		GameObject gameObject = null;
		if (m_SpriteList[nSrcSpriteIndex].m_EffectPrefab != null)
		{
			gameObject = CreateGameObject("Effect_" + m_SpriteList[nSrcSpriteIndex].m_EffectPrefab.name);
			if (gameObject == null)
			{
				return null;
			}
			ChangeParent(parentTrans, gameObject.transform, true, null);
			NcAttachPrefab ncAttachPrefab = gameObject.AddComponent<NcAttachPrefab>();
			ncAttachPrefab.m_AttachPrefab = m_SpriteList[nSrcSpriteIndex].m_EffectPrefab;
			ncAttachPrefab.m_fPrefabSpeed = m_SpriteList[nSrcSpriteIndex].m_fEffectSpeed;
			ncAttachPrefab.m_bDetachParent = m_SpriteList[nSrcSpriteIndex].m_bEffectDetach;
			ncAttachPrefab.UpdateImmediately();
			gameObject.transform.localScale *= m_SpriteList[nSrcSpriteIndex].m_fEffectScale;
			gameObject.transform.localPosition += m_SpriteList[nSrcSpriteIndex].m_EffectPos;
			gameObject.transform.localRotation *= Quaternion.Euler(m_SpriteList[nSrcSpriteIndex].m_EffectRot);
		}
		return gameObject;
	}

	private void DestroyEffectObject()
	{
		if (m_CurrentEffect != null)
		{
			UnityEngine.Object.Destroy(m_CurrentEffect);
		}
		m_CurrentEffect = null;
	}

	private void CreateSoundObject(NcSpriteNode ncSpriteNode)
	{
		if (m_bShowEffect && ncSpriteNode.m_AudioClip != null)
		{
			if (m_CurrentSound == null)
			{
				m_CurrentSound = base.gameObject.AddComponent<NcAttachSound>();
			}
			m_CurrentSound.m_AudioClip = ncSpriteNode.m_AudioClip;
			m_CurrentSound.m_bLoop = ncSpriteNode.m_bSoundLoop;
			m_CurrentSound.m_fVolume = ncSpriteNode.m_fSoundVolume;
			m_CurrentSound.m_fPitch = ncSpriteNode.m_fSoundPitch;
			m_CurrentSound.enabled = true;
			m_CurrentSound.Replay();
		}
	}

	public void OnChangingSprite(int nOldNodeIndex, int nNewNodeIndex)
	{
		m_bEndSprite = false;
		DestroyEffectObject();
	}

	public void OnAnimationStartFrame(NcSpriteAnimation spriteCom)
	{
	}

	public void OnAnimationChangingFrame(NcSpriteAnimation spriteCom, int nOldIndex, int nNewIndex, int nLoopCount)
	{
		if (m_SpriteList.Count > m_nCurrentIndex)
		{
			if (m_SpriteList[m_nCurrentIndex].m_EffectPrefab != null && (nOldIndex < m_SpriteList[m_nCurrentIndex].m_nEffectFrame || nNewIndex <= nOldIndex) && m_SpriteList[m_nCurrentIndex].m_nEffectFrame <= nNewIndex && (nLoopCount == 0 || !m_SpriteList[m_nCurrentIndex].m_bEffectOnlyFirst))
			{
				CreateEffectObject();
			}
			if (m_SpriteList[m_nCurrentIndex].m_AudioClip != null && (nOldIndex < m_SpriteList[m_nCurrentIndex].m_nSoundFrame || nNewIndex <= nOldIndex) && m_SpriteList[m_nCurrentIndex].m_nSoundFrame <= nNewIndex && (nLoopCount == 0 || !m_SpriteList[m_nCurrentIndex].m_bSoundOnlyFirst))
			{
				CreateSoundObject(m_SpriteList[m_nCurrentIndex]);
			}
		}
	}

	public bool OnAnimationLastFrame(NcSpriteAnimation spriteCom, int nLoopCount)
	{
		if (m_SpriteList.Count <= m_nCurrentIndex)
		{
			return false;
		}
		m_bEndSprite = true;
		if (m_bSequenceMode)
		{
			if (m_nCurrentIndex < GetSpriteNodeCount() - 1)
			{
				if (((!m_SpriteList[m_nCurrentIndex].m_bLoop) ? 1 : 3) == nLoopCount)
				{
					SetSprite(m_nCurrentIndex + 1);
					return true;
				}
			}
			else
			{
				SetSprite(0);
			}
		}
		else
		{
			NcSpriteAnimation ncSpriteAnimation = SetSprite(m_SpriteList[m_nCurrentIndex].m_nNextSpriteIndex) as NcSpriteAnimation;
			if (ncSpriteAnimation != null)
			{
				ncSpriteAnimation.ResetAnimation();
				return true;
			}
		}
		return false;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
	}

	public static void CreatePlane(MeshFilter meshFilter, float fUvScale, NcFrameInfo ncSpriteFrameInfo, bool bTrimCenterAlign, ALIGN_TYPE alignType, MESH_TYPE m_MeshType)
	{
		Vector2 vector = new Vector2(fUvScale * ncSpriteFrameInfo.m_FrameScale.x, fUvScale * ncSpriteFrameInfo.m_FrameScale.y);
		float num;
		switch (alignType)
		{
		case ALIGN_TYPE.BOTTOM:
			num = 1f * vector.y;
			break;
		case ALIGN_TYPE.TOP:
			num = -1f * vector.y;
			break;
		default:
			num = 0f;
			break;
		}
		float num2 = num;
		Rect frameUvOffset = ncSpriteFrameInfo.m_FrameUvOffset;
		if (bTrimCenterAlign)
		{
			frameUvOffset.center = Vector2.zero;
		}
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3(frameUvOffset.xMax * vector.x, frameUvOffset.yMax * vector.y + num2),
			new Vector3(frameUvOffset.xMax * vector.x, frameUvOffset.yMin * vector.y + num2),
			new Vector3(frameUvOffset.xMin * vector.x, frameUvOffset.yMin * vector.y + num2),
			new Vector3(frameUvOffset.xMin * vector.x, frameUvOffset.yMax * vector.y + num2)
		};
		Color[] colors = new Color[4]
		{
			Color.white,
			Color.white,
			Color.white,
			Color.white
		};
		Vector3[] normals = new Vector3[4]
		{
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f)
		};
		Vector4[] tangents = new Vector4[4]
		{
			new Vector4(1f, 0f, 0f, -1f),
			new Vector4(1f, 0f, 0f, -1f),
			new Vector4(1f, 0f, 0f, -1f),
			new Vector4(1f, 0f, 0f, -1f)
		};
		int[] triangles = ((m_MeshType == MESH_TYPE.BuiltIn_Plane) ? new int[6] { 1, 2, 0, 0, 2, 3 } : new int[12]
		{
			1, 2, 0, 0, 2, 3, 1, 0, 3, 3,
			2, 1
		});
		Vector2[] uv = new Vector2[4]
		{
			new Vector2(1f, 1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 0f),
			new Vector2(0f, 1f)
		};
		meshFilter.mesh.Clear();
		meshFilter.mesh.vertices = vertices;
		meshFilter.mesh.colors = colors;
		meshFilter.mesh.normals = normals;
		meshFilter.mesh.tangents = tangents;
		meshFilter.mesh.triangles = triangles;
		meshFilter.mesh.uv = uv;
		meshFilter.mesh.RecalculateBounds();
	}

	public static void UpdatePlane(MeshFilter meshFilter, float fUvScale, NcFrameInfo ncSpriteFrameInfo, bool bTrimCenterAlign, ALIGN_TYPE alignType)
	{
		Vector2 vector = new Vector2(fUvScale * ncSpriteFrameInfo.m_FrameScale.x, fUvScale * ncSpriteFrameInfo.m_FrameScale.y);
		float num;
		switch (alignType)
		{
		case ALIGN_TYPE.BOTTOM:
			num = 1f * vector.y;
			break;
		case ALIGN_TYPE.TOP:
			num = -1f * vector.y;
			break;
		default:
			num = 0f;
			break;
		}
		float num2 = num;
		Rect frameUvOffset = ncSpriteFrameInfo.m_FrameUvOffset;
		if (bTrimCenterAlign)
		{
			frameUvOffset.center = Vector2.zero;
		}
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3(frameUvOffset.xMax * vector.x, frameUvOffset.yMax * vector.y + num2),
			new Vector3(frameUvOffset.xMax * vector.x, frameUvOffset.yMin * vector.y + num2),
			new Vector3(frameUvOffset.xMin * vector.x, frameUvOffset.yMin * vector.y + num2),
			new Vector3(frameUvOffset.xMin * vector.x, frameUvOffset.yMax * vector.y + num2)
		};
		meshFilter.mesh.vertices = vertices;
		meshFilter.mesh.RecalculateBounds();
	}

	public static void UpdateMeshUVs(MeshFilter meshFilter, Rect uv)
	{
		Vector2[] uv2 = new Vector2[4]
		{
			new Vector2(uv.x + uv.width, uv.y + uv.height),
			new Vector2(uv.x + uv.width, uv.y),
			new Vector2(uv.x, uv.y),
			new Vector2(uv.x, uv.y + uv.height)
		};
		meshFilter.mesh.uv = uv2;
	}
}
