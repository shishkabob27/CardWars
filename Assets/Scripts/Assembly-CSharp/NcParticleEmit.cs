using UnityEngine;

public class NcParticleEmit : NcEffectBehaviour
{
	public enum AttachType
	{
		Active = 0,
		Destroy = 1,
	}

	public AttachType m_AttachType;
	public float m_fDelayTime;
	public float m_fRepeatTime;
	public int m_nRepeatCount;
	public GameObject m_ParticlePrefab;
	public int m_EmitCount;
	public Vector3 m_AddStartPos;
	public Vector3 m_RandomRange;
}
