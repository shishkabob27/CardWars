using UnityEngine;

public class NcSpriteTexture : NcEffectBehaviour
{
	public GameObject m_NcSpriteFactoryPrefab;
	public NcSpriteFactory.NcFrameInfo[] m_NcSpriteFrameInfos;
	public float m_fUvScale;
	public int m_nSpriteFactoryIndex;
	public int m_nFrameIndex;
	public NcSpriteFactory.MESH_TYPE m_MeshType;
	public NcSpriteFactory.ALIGN_TYPE m_AlignType;
}
