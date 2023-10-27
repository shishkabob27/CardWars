using UnityEngine;

public class NcAutoDestruct : NcEffectBehaviour
{
	public enum CollisionType
	{
		NONE = 0,
		COLLISION = 1,
		WORLD_Y = 2,
	}

	public float m_fLifeTime;
	public float m_fSmoothDestroyTime;
	public bool m_bDisableEmit;
	public bool m_bSmoothHide;
	public bool m_bMeshFilterOnlySmoothHide;
	public CollisionType m_CollisionType;
	public LayerMask m_CollisionLayer;
	public float m_fCollisionRadius;
	public float m_fDestructPosY;
}
