using UnityEngine;

public class NcSpriteAnimation : NcEffectAniBehaviour
{
	public enum TEXTURE_TYPE
	{
		TileTexture = 0,
		TrimTexture = 1,
		SpriteFactory = 2,
	}

	public enum PLAYMODE
	{
		DEFAULT = 0,
		INVERSE = 1,
		PINGPONG = 2,
		RANDOM = 3,
		SELECT = 4,
	}

	public TEXTURE_TYPE m_TextureType;
	public PLAYMODE m_PlayMode;
	public float m_fDelayTime;
	public int m_nStartFrame;
	public int m_nFrameCount;
	public int m_nSelectFrame;
	public bool m_bLoop;
	public int m_nLoopStartFrame;
	public int m_nLoopFrameCount;
	public int m_nLoopingCount;
	public bool m_bAutoDestruct;
	public float m_fFps;
	public int m_nTilingX;
	public int m_nTilingY;
	public GameObject m_NcSpriteFactoryPrefab;
	public NcSpriteFactory.NcFrameInfo[] m_NcSpriteFrameInfos;
	public float m_fUvScale;
	public int m_nSpriteFactoryIndex;
	public NcSpriteFactory.MESH_TYPE m_MeshType;
	public NcSpriteFactory.ALIGN_TYPE m_AlignType;
	public bool m_bTrimCenterAlign;
	public bool m_bBuildSpriteObj;
	public bool m_bNeedRebuildAlphaChannel;
	public AnimationCurve m_curveAlphaWeight;
}
