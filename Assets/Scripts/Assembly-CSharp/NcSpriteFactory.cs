using System;
using UnityEngine;
using System.Collections.Generic;

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
	public class NcSpriteNode
	{
		public bool m_bIncludedAtlas;
		public string m_TextureGUID;
		public string m_TextureName;
		public float m_fMaxTextureAlpha;
		public string m_SpriteName;
		public NcSpriteFactory.NcFrameInfo[] m_FrameInfos;
		public int m_nTilingX;
		public int m_nTilingY;
		public int m_nStartFrame;
		public int m_nFrameCount;
		public bool m_bLoop;
		public int m_nLoopStartFrame;
		public int m_nLoopFrameCount;
		public int m_nLoopingCount;
		public float m_fFps;
		public float m_fTime;
		public int m_nNextSpriteIndex;
		public int m_nTestMode;
		public float m_fTestSpeed;
		public bool m_bEffectInstantiate;
		public GameObject m_EffectPrefab;
		public int m_nEffectFrame;
		public bool m_bEffectOnlyFirst;
		public bool m_bEffectDetach;
		public float m_fEffectSpeed;
		public float m_fEffectScale;
		public Vector3 m_EffectPos;
		public Vector3 m_EffectRot;
		public AudioClip m_AudioClip;
		public int m_nSoundFrame;
		public bool m_bSoundOnlyFirst;
		public bool m_bSoundLoop;
		public float m_fSoundVolume;
		public float m_fSoundPitch;
	}

	public enum SPRITE_TYPE
	{
		NcSpriteTexture = 0,
		NcSpriteAnimation = 1,
	}

	public enum SHOW_TYPE
	{
		NONE = 0,
		ALL = 1,
		SPRITE = 2,
		ANIMATION = 3,
		EFFECT = 4,
	}

	public enum MESH_TYPE
	{
		BuiltIn_Plane = 0,
		BuiltIn_TwosidePlane = 1,
	}

	public enum ALIGN_TYPE
	{
		TOP = 0,
		CENTER = 1,
		BOTTOM = 2,
	}

	public SPRITE_TYPE m_SpriteType;
	public List<NcSpriteFactory.NcSpriteNode> m_SpriteList;
	public int m_nCurrentIndex;
	public int m_nMaxAtlasTextureSize;
	public bool m_bNeedRebuild;
	public int m_nBuildStartIndex;
	public bool m_bTrimBlack;
	public bool m_bTrimAlpha;
	public float m_fUvScale;
	public float m_fTextureRatio;
	public GameObject m_CurrentEffect;
	public NcAttachSound m_CurrentSound;
	public SHOW_TYPE m_ShowType;
	public bool m_bShowEffect;
	public bool m_bTestMode;
	public bool m_bSequenceMode;
}
