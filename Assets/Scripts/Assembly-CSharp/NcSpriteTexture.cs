using UnityEngine;

public class NcSpriteTexture : NcEffectBehaviour
{
	public GameObject m_NcSpriteFactoryPrefab;

	protected NcSpriteFactory m_NcSpriteFactoryCom;

	public NcSpriteFactory.NcFrameInfo[] m_NcSpriteFrameInfos;

	public float m_fUvScale = 1f;

	public int m_nSpriteFactoryIndex;

	public int m_nFrameIndex;

	public NcSpriteFactory.MESH_TYPE m_MeshType;

	public NcSpriteFactory.ALIGN_TYPE m_AlignType = NcSpriteFactory.ALIGN_TYPE.CENTER;

	protected GameObject m_EffectObject;

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
	}

	private void Start()
	{
		UpdateSpriteTexture(true);
	}

	public void SetSpriteFactoryIndex(int nSpriteFactoryIndex, int nFrameIndex, bool bRunImmediate)
	{
		if (m_NcSpriteFactoryCom == null)
		{
			if (!m_NcSpriteFactoryPrefab || !(m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null))
			{
				return;
			}
			m_NcSpriteFactoryCom = m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
		}
		m_nSpriteFactoryIndex = nSpriteFactoryIndex;
		if (m_NcSpriteFactoryCom.IsValidFactory())
		{
			m_NcSpriteFrameInfos = m_NcSpriteFactoryCom.GetSpriteNode(nSpriteFactoryIndex).m_FrameInfos;
			m_nFrameIndex = ((0 > nFrameIndex) ? m_nFrameIndex : nFrameIndex);
			m_nFrameIndex = ((m_NcSpriteFrameInfos.Length != 0 && m_NcSpriteFrameInfos.Length > m_nFrameIndex) ? m_nFrameIndex : 0);
			m_fUvScale = m_NcSpriteFactoryCom.m_fUvScale;
			if (bRunImmediate)
			{
				UpdateSpriteTexture(bRunImmediate);
			}
		}
	}

	private void UpdateSpriteTexture(bool bShowEffect)
	{
		if (!UpdateSpriteMaterial() || !m_NcSpriteFactoryCom.IsValidFactory())
		{
			return;
		}
		if (m_NcSpriteFrameInfos.Length == 0)
		{
			SetSpriteFactoryIndex(m_nSpriteFactoryIndex, m_nFrameIndex, false);
		}
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
		NcSpriteFactory.CreatePlane(m_MeshFilter, m_fUvScale, m_NcSpriteFrameInfos[m_nFrameIndex], false, m_AlignType, m_MeshType);
		NcSpriteFactory.UpdateMeshUVs(m_MeshFilter, m_NcSpriteFrameInfos[m_nFrameIndex].m_TextureUvOffset);
		if (bShowEffect)
		{
			m_EffectObject = m_NcSpriteFactoryCom.CreateSpriteEffect(m_nSpriteFactoryIndex, base.transform);
		}
	}

	public bool UpdateSpriteMaterial()
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
		if (m_NcSpriteFactoryCom.m_SpriteType != 0)
		{
			return false;
		}
		GetComponent<Renderer>().sharedMaterial = m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial;
		return true;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
	}

	public override void OnUpdateToolData()
	{
	}
}
