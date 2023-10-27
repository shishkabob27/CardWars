using UnityEngine;

public class NcAttachPrefab : NcEffectBehaviour
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
	public GameObject m_AttachPrefab;
	public float m_fPrefabSpeed;
	public float m_fPrefabLifeTime;
	public bool m_bWorldSpace;
	public Vector3 m_AddStartPos;
	public Vector3 m_AccumStartRot;
	public Vector3 m_RandomRange;
	public int m_nSpriteFactoryIndex;
	public bool m_bDetachParent;
}
