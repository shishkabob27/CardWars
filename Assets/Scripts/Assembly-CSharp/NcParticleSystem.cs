using UnityEngine;

public class NcParticleSystem : NcEffectBehaviour
{
	public enum ParticleDestruct
	{
		NONE = 0,
		COLLISION = 1,
		WORLD_Y = 2,
	}

	public float m_fStartDelayTime;
	public bool m_bBurst;
	public float m_fBurstRepeatTime;
	public int m_nBurstRepeatCount;
	public int m_fBurstEmissionCount;
	public float m_fEmitTime;
	public float m_fSleepTime;
	public bool m_bScaleWithTransform;
	public bool m_bWorldSpace;
	public float m_fStartSizeRate;
	public float m_fStartLifeTimeRate;
	public float m_fStartEmissionRate;
	public float m_fStartSpeedRate;
	public float m_fRenderLengthRate;
	public float m_fLegacyMinMeshNormalVelocity;
	public float m_fLegacyMaxMeshNormalVelocity;
	public float m_fShurikenSpeedRate;
	public ParticleDestruct m_ParticleDestruct;
	public LayerMask m_CollisionLayer;
	public float m_fCollisionRadius;
	public float m_fDestructPosY;
	public GameObject m_AttachPrefab;
	public float m_fPrefabScale;
	public float m_fPrefabSpeed;
	public float m_fPrefabLifeTime;
}
