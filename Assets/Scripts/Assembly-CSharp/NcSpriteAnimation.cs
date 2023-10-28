using UnityEngine;

public class NcSpriteAnimation : NcEffectAniBehaviour
{
	public enum TEXTURE_TYPE
	{
		TileTexture,
		TrimTexture,
		SpriteFactory
	}

	public enum PLAYMODE
	{
		DEFAULT,
		INVERSE,
		PINGPONG,
		RANDOM,
		SELECT
	}

	public TEXTURE_TYPE m_TextureType;

	public PLAYMODE m_PlayMode;

	public float m_fDelayTime;

	public int m_nStartFrame;

	public int m_nFrameCount;

	public int m_nSelectFrame;

	public bool m_bLoop = true;

	public int m_nLoopStartFrame;

	public int m_nLoopFrameCount;

	public int m_nLoopingCount;

	public bool m_bAutoDestruct;

	public float m_fFps = 10f;

	public int m_nTilingX = 2;

	public int m_nTilingY = 2;

	public GameObject m_NcSpriteFactoryPrefab;

	protected NcSpriteFactory m_NcSpriteFactoryCom;

	public NcSpriteFactory.NcFrameInfo[] m_NcSpriteFrameInfos;

	public float m_fUvScale = 1f;

	public int m_nSpriteFactoryIndex;

	public NcSpriteFactory.MESH_TYPE m_MeshType;

	public NcSpriteFactory.ALIGN_TYPE m_AlignType = NcSpriteFactory.ALIGN_TYPE.CENTER;

	public bool m_bTrimCenterAlign;

	protected bool m_bCreateBuiltInPlane;

	[HideInInspector]
	public bool m_bBuildSpriteObj;

	[HideInInspector]
	public bool m_bNeedRebuildAlphaChannel;

	[HideInInspector]
	public AnimationCurve m_curveAlphaWeight;

	protected Vector2 m_size;

	protected Renderer m_Renderer;

	protected float m_fStartTime;

	protected int m_nLastIndex = -999;

	protected int m_nLastSeqIndex = -1;

	protected bool m_bInPartLoop;

	protected bool m_bBreakLoop;

	protected Vector2[] m_MeshUVsByTileTexture;

	public override int GetAnimationState()
	{
		if (!base.enabled || !NcEffectBehaviour.IsActive(base.gameObject))
		{
			return -1;
		}
		if (m_fStartTime == 0f || !IsEndAnimation())
		{
			return 1;
		}
		return 0;
	}

	public float GetDurationTime()
	{
		return (float)((m_PlayMode != PLAYMODE.PINGPONG) ? m_nFrameCount : (m_nFrameCount * 2 - 1)) / m_fFps;
	}

	public int GetShowIndex()
	{
		return m_nLastIndex + m_nStartFrame;
	}

	public void SetBreakLoop()
	{
		m_bBreakLoop = true;
	}

	public bool IsInPartLoop()
	{
		return m_bInPartLoop;
	}

	public override void ResetAnimation()
	{
		m_nLastIndex = -1;
		m_nLastSeqIndex = -1;
		if (!base.enabled)
		{
			base.enabled = true;
		}
		Start();
	}

	public void SetSelectFrame(int nSelFrame)
	{
		m_nSelectFrame = nSelFrame;
		SetIndex(m_nSelectFrame);
	}

	public bool IsEmptyFrame()
	{
		return m_NcSpriteFrameInfos[m_nSelectFrame].m_bEmptyFrame;
	}

	public int GetMaxFrameCount()
	{
		if (m_TextureType == TEXTURE_TYPE.TileTexture)
		{
			return m_nTilingX * m_nTilingY;
		}
		return m_NcSpriteFrameInfos.Length;
	}

	public int GetValidFrameCount()
	{
		if (m_TextureType == TEXTURE_TYPE.TileTexture)
		{
			return m_nTilingX * m_nTilingY - m_nStartFrame;
		}
		return m_NcSpriteFrameInfos.Length - m_nStartFrame;
	}

	private void Awake()
	{
		if (m_NcSpriteFactoryPrefab == null && base.gameObject.GetComponent<NcSpriteFactory>() != null)
		{
			m_NcSpriteFactoryPrefab = base.gameObject;
		}
		if ((bool)m_NcSpriteFactoryPrefab && m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null)
		{
			m_NcSpriteFactoryCom = m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
		}
		if (m_MeshFilter == null && base.gameObject.GetComponent<MeshFilter>() != null)
		{
			m_MeshFilter = base.gameObject.GetComponent<MeshFilter>();
		}
		if (m_nLoopFrameCount == 0)
		{
			m_nLoopFrameCount = m_nFrameCount - m_nLoopStartFrame;
		}
	}

	private void ResetLocalValue()
	{
		m_size = new Vector2(1f / (float)m_nTilingX, 1f / (float)m_nTilingY);
		m_Renderer = GetComponent<Renderer>();
		m_fStartTime = NcEffectBehaviour.GetEngineTime();
		m_nFrameCount = ((m_nFrameCount > 0) ? m_nFrameCount : (m_nTilingX * m_nTilingY));
		m_nLastIndex = -999;
		m_nLastSeqIndex = -1;
		m_bInPartLoop = false;
		m_bBreakLoop = false;
	}

	private void Start()
	{
		ResetLocalValue();
		if (m_Renderer == null)
		{
			base.enabled = false;
			return;
		}
		if (m_PlayMode == PLAYMODE.SELECT)
		{
			SetIndex(m_nSelectFrame);
			return;
		}
		if (0f < m_fDelayTime)
		{
			m_Renderer.enabled = false;
			return;
		}
		if (m_PlayMode == PLAYMODE.RANDOM)
		{
			SetIndex(Random.Range(0, m_nFrameCount - 1));
			return;
		}
		InitAnimationTimer();
		SetIndex(0);
	}

	private void Update()
	{
		if (m_PlayMode == PLAYMODE.SELECT || m_Renderer == null || m_nTilingX * m_nTilingY == 0)
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
			m_Renderer.enabled = true;
		}
		if (m_PlayMode == PLAYMODE.RANDOM)
		{
			return;
		}
		int num = (int)(m_Timer.GetTime() * m_fFps);
		if (num == 0 && m_NcSpriteFactoryCom != null)
		{
			m_NcSpriteFactoryCom.OnAnimationStartFrame(this);
		}
		if (m_NcSpriteFactoryCom != null && m_nFrameCount <= 0)
		{
			m_NcSpriteFactoryCom.OnAnimationLastFrame(this, 0);
			return;
		}
		if (((m_PlayMode != PLAYMODE.PINGPONG) ? m_nFrameCount : (m_nFrameCount * 2 - 1)) <= num)
		{
			if (!m_bLoop)
			{
				if (!(m_NcSpriteFactoryCom != null) || !m_NcSpriteFactoryCom.OnAnimationLastFrame(this, 1))
				{
					UpdateEndAnimation();
				}
				return;
			}
			if (m_PlayMode == PLAYMODE.PINGPONG)
			{
				if (m_NcSpriteFactoryCom != null && num % (m_nFrameCount * 2 - 2) == 1 && m_NcSpriteFactoryCom.OnAnimationLastFrame(this, num / (m_nFrameCount * 2 - 1)))
				{
					return;
				}
			}
			else if (m_NcSpriteFactoryCom != null && num % m_nFrameCount == 0 && m_NcSpriteFactoryCom.OnAnimationLastFrame(this, num / m_nFrameCount))
			{
				return;
			}
		}
		SetIndex(num);
	}

	public void SetSpriteFactoryIndex(int nSpriteFactoryIndex, bool bRunImmediate)
	{
		if (m_NcSpriteFactoryCom == null)
		{
			if (!m_NcSpriteFactoryPrefab || !(m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null))
			{
				return;
			}
			m_NcSpriteFactoryCom = m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
		}
		NcSpriteFactory.NcSpriteNode spriteNode = m_NcSpriteFactoryCom.GetSpriteNode(nSpriteFactoryIndex);
		m_bBuildSpriteObj = false;
		m_bAutoDestruct = false;
		m_fUvScale = m_NcSpriteFactoryCom.m_fUvScale;
		m_nSpriteFactoryIndex = nSpriteFactoryIndex;
		m_nStartFrame = 0;
		m_nFrameCount = spriteNode.m_nFrameCount;
		m_fFps = spriteNode.m_fFps;
		m_bLoop = spriteNode.m_bLoop;
		m_nLoopStartFrame = spriteNode.m_nLoopStartFrame;
		m_nLoopFrameCount = spriteNode.m_nLoopFrameCount;
		m_nLoopingCount = spriteNode.m_nLoopingCount;
		m_NcSpriteFrameInfos = spriteNode.m_FrameInfos;
		ResetLocalValue();
	}

	private int GetPartLoopFrameIndex(int nSeqIndex)
	{
		if (nSeqIndex < 0)
		{
			return -1;
		}
		int num = nSeqIndex - m_nLoopStartFrame;
		if (num < 0)
		{
			return nSeqIndex;
		}
		int num2 = num / m_nLoopFrameCount;
		if (m_nLoopingCount == 0 || num2 < m_nLoopingCount)
		{
			return num % m_nLoopFrameCount + m_nLoopStartFrame;
		}
		return num - m_nLoopFrameCount * (m_nLoopingCount - 1) + m_nLoopStartFrame;
	}

	private int GetPartLoopCount(int nSeqIndex)
	{
		if (nSeqIndex < 0)
		{
			return -1;
		}
		int num = nSeqIndex - m_nLoopStartFrame;
		if (num < 0)
		{
			return -1;
		}
		int num2 = num / m_nLoopFrameCount;
		if (m_nLoopingCount == 0 || num2 < m_nLoopingCount)
		{
			return num2;
		}
		return m_nLoopingCount;
	}

	private int CalcPartLoopInfo(int nSeqIndex, ref int nLoopCount)
	{
		if (m_nLoopFrameCount <= 0)
		{
			return 0;
		}
		if (m_bBreakLoop)
		{
			nLoopCount = GetPartLoopCount(nSeqIndex);
			UpdateEndAnimation();
			m_bBreakLoop = false;
			return m_nLoopStartFrame + m_nLoopFrameCount;
		}
		if (nSeqIndex < m_nLoopStartFrame)
		{
			return nSeqIndex;
		}
		m_bInPartLoop = true;
		int partLoopCount = GetPartLoopCount(m_nLastSeqIndex);
		int num = GetPartLoopFrameIndex(nSeqIndex);
		nLoopCount = GetPartLoopCount(nSeqIndex);
		int num2 = 0;
		int num3 = partLoopCount;
		while (num3 < Mathf.Min(nLoopCount, m_nLoopFrameCount - 1))
		{
			if (base.transform.parent != null)
			{
				base.transform.parent.SendMessage("OnSpriteAnimationLoopStart", nLoopCount, SendMessageOptions.DontRequireReceiver);
			}
			num3++;
			num2++;
		}
		if (0 < m_nLoopingCount && m_nLoopingCount <= nLoopCount)
		{
			m_bInPartLoop = false;
			if (base.transform.parent != null)
			{
				base.transform.parent.SendMessage("OnSpriteAnimationLoopEnd", nLoopCount, SendMessageOptions.DontRequireReceiver);
			}
			if (m_nFrameCount <= num)
			{
				num = m_nFrameCount - 1;
				UpdateEndAnimation();
			}
		}
		return num;
	}

	private void UpdateEndAnimation()
	{
		base.enabled = false;
		OnEndAnimation();
		if (m_bAutoDestruct)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void SetIndex(int nSeqIndex)
	{
		if (!(m_Renderer != null))
		{
			return;
		}
		int num = (m_nLastSeqIndex = nSeqIndex);
		int nLoopCount = nSeqIndex / m_nFrameCount;
		switch (m_PlayMode)
		{
		case PLAYMODE.DEFAULT:
			num = ((!m_bLoop) ? (nSeqIndex % m_nFrameCount) : (CalcPartLoopInfo(nSeqIndex, ref nLoopCount) % m_nFrameCount));
			break;
		case PLAYMODE.INVERSE:
			num = m_nFrameCount - num % m_nFrameCount - 1;
			break;
		case PLAYMODE.PINGPONG:
			nLoopCount = num / (m_nFrameCount * 2 - ((num == 0) ? 1 : 2));
			num %= m_nFrameCount * 2 - ((num == 0) ? 1 : 2);
			if (m_nFrameCount <= num)
			{
				num = m_nFrameCount - num % m_nFrameCount - 2;
			}
			break;
		case PLAYMODE.SELECT:
			num = nSeqIndex % m_nFrameCount;
			break;
		}
		if (num == m_nLastIndex)
		{
			return;
		}
		if (m_TextureType == TEXTURE_TYPE.TileTexture)
		{
			int num2 = (num + m_nStartFrame) % m_nTilingX;
			int num3 = (num + m_nStartFrame) / m_nTilingX;
			Vector2 mainTextureOffset = new Vector2((float)num2 * m_size.x, 1f - m_size.y - (float)num3 * m_size.y);
			if (!UpdateMeshUVsByTileTexture(new Rect(mainTextureOffset.x, mainTextureOffset.y, m_size.x, m_size.y)))
			{
				m_Renderer.material.mainTextureOffset = mainTextureOffset;
				m_Renderer.material.mainTextureScale = m_size;
				AddRuntimeMaterial(m_Renderer.material);
			}
		}
		else if (m_TextureType == TEXTURE_TYPE.TrimTexture)
		{
			UpdateSpriteTexture(num, true);
		}
		else if (m_TextureType == TEXTURE_TYPE.SpriteFactory)
		{
			UpdateFactoryTexture(num, true);
		}
		if (m_NcSpriteFactoryCom != null)
		{
			m_NcSpriteFactoryCom.OnAnimationChangingFrame(this, m_nLastIndex, num, nLoopCount);
		}
		m_nLastIndex = num;
	}

	private void UpdateSpriteTexture(int nSelIndex, bool bShowEffect)
	{
		nSelIndex += m_nStartFrame;
		if (m_NcSpriteFrameInfos != null && nSelIndex >= 0 && m_NcSpriteFrameInfos.Length > nSelIndex)
		{
			CreateBuiltInPlane(nSelIndex);
			UpdateBuiltInPlane(nSelIndex);
		}
	}

	private void UpdateFactoryTexture(int nSelIndex, bool bShowEffect)
	{
		nSelIndex += m_nStartFrame;
		if (m_NcSpriteFrameInfos != null && nSelIndex >= 0 && m_NcSpriteFrameInfos.Length > nSelIndex && UpdateFactoryMaterial() && m_NcSpriteFactoryCom.IsValidFactory())
		{
			CreateBuiltInPlane(nSelIndex);
			UpdateBuiltInPlane(nSelIndex);
		}
	}

	public bool UpdateFactoryMaterial()
	{
		if (m_NcSpriteFactoryPrefab == null)
		{
			return false;
		}
		if (m_NcSpriteFactoryPrefab.GetComponent<Renderer>() == null || m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial == null || m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial.mainTexture == null)
		{
			return false;
		}
		if (GetComponent<Renderer>() == null)
		{
			return false;
		}
		if (m_NcSpriteFactoryCom == null)
		{
			return false;
		}
		if (m_nSpriteFactoryIndex < 0 || m_NcSpriteFactoryCom.GetSpriteNodeCount() <= m_nSpriteFactoryIndex)
		{
			return false;
		}
		if (m_NcSpriteFactoryCom.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation)
		{
			return false;
		}
		GetComponent<Renderer>().sharedMaterial = m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial;
		return true;
	}

	private void CreateBuiltInPlane(int nSelIndex)
	{
		if (m_bCreateBuiltInPlane)
		{
			return;
		}
		m_bCreateBuiltInPlane = true;
		if (m_MeshFilter == null)
		{
			if (base.gameObject.GetComponent<MeshFilter>() != null)
			{
				m_MeshFilter = base.gameObject.GetComponent<MeshFilter>();
			}
			else
			{
				m_MeshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
		}
		NcSpriteFactory.CreatePlane(m_MeshFilter, m_fUvScale, m_NcSpriteFrameInfos[nSelIndex], m_TextureType != 0 && m_bTrimCenterAlign, m_AlignType, m_MeshType);
	}

	private void UpdateBuiltInPlane(int nSelIndex)
	{
		NcSpriteFactory.UpdatePlane(m_MeshFilter, m_fUvScale, m_NcSpriteFrameInfos[nSelIndex], m_TextureType != 0 && m_bTrimCenterAlign, m_AlignType);
		NcSpriteFactory.UpdateMeshUVs(m_MeshFilter, m_NcSpriteFrameInfos[nSelIndex].m_TextureUvOffset);
	}

	private bool UpdateMeshUVsByTileTexture(Rect uv)
	{
		if (m_MeshFilter != null && m_MeshUVsByTileTexture == null)
		{
			return false;
		}
		if (m_MeshFilter == null)
		{
			m_MeshFilter = (MeshFilter)base.gameObject.GetComponent(typeof(MeshFilter));
		}
		if (m_MeshFilter == null || m_MeshFilter.sharedMesh == null)
		{
			return false;
		}
		if (m_MeshUVsByTileTexture == null)
		{
			for (int i = 0; i < m_MeshFilter.sharedMesh.uv.Length; i++)
			{
				if (m_MeshFilter.sharedMesh.uv[i].x != 0f && m_MeshFilter.sharedMesh.uv[i].x != 1f)
				{
					return false;
				}
				if (m_MeshFilter.sharedMesh.uv[i].y != 0f && m_MeshFilter.sharedMesh.uv[i].y != 1f)
				{
					return false;
				}
			}
			m_MeshUVsByTileTexture = m_MeshFilter.sharedMesh.uv;
		}
		Vector2[] array = new Vector2[m_MeshUVsByTileTexture.Length];
		for (int j = 0; j < m_MeshUVsByTileTexture.Length; j++)
		{
			if (m_MeshUVsByTileTexture[j].x == 0f)
			{
				array[j].x = uv.x;
			}
			if (m_MeshUVsByTileTexture[j].y == 0f)
			{
				array[j].y = uv.y;
			}
			if (m_MeshUVsByTileTexture[j].x == 1f)
			{
				array[j].x = uv.x + uv.width;
			}
			if (m_MeshUVsByTileTexture[j].y == 1f)
			{
				array[j].y = uv.y + uv.height;
			}
		}
		m_MeshFilter.mesh.uv = array;
		return true;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime *= fSpeedRate;
		m_fFps *= fSpeedRate;
	}
}
