using UnityEngine;

public class NcParticleEmit : NcEffectBehaviour
{
	public enum AttachType
	{
		Active,
		Destroy
	}

	public AttachType m_AttachType;

	public float m_fDelayTime;

	public float m_fRepeatTime;

	public int m_nRepeatCount;

	public GameObject m_ParticlePrefab;

	public int m_EmitCount = 10;

	public Vector3 m_AddStartPos = Vector3.zero;

	public Vector3 m_RandomRange = Vector3.zero;

	protected float m_fStartTime;

	protected int m_nCreateCount;

	protected bool m_bStartAttach;

	protected GameObject m_CreateGameObject;

	protected bool m_bEnabled;

	protected ParticleSystem m_ps;

	public override int GetAnimationState()
	{
		if (base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && m_ParticlePrefab != null)
		{
			if (m_AttachType == AttachType.Active && ((m_nRepeatCount == 0 && m_nCreateCount < 1) || (0f < m_fRepeatTime && m_nRepeatCount == 0) || (0 < m_nRepeatCount && m_nCreateCount < m_nRepeatCount)))
			{
				return 1;
			}
			if (m_AttachType == AttachType.Destroy)
			{
				return 1;
			}
		}
		return 0;
	}

	public void UpdateImmediately()
	{
		Update();
	}

	public void EmitSharedParticle()
	{
		CreateAttachSharedParticle();
	}

	public GameObject GetInstanceObject()
	{
		if (m_CreateGameObject == null)
		{
			UpdateImmediately();
		}
		return m_CreateGameObject;
	}

	public void SetEnable(bool bEnable)
	{
		m_bEnabled = bEnable;
	}

	private void Awake()
	{
		m_bEnabled = base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && GetComponent<NcDontActive>() == null;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_ParticlePrefab == null || m_AttachType != 0)
		{
			return;
		}
		if (!m_bStartAttach)
		{
			m_fStartTime = NcEffectBehaviour.GetEngineTime();
			m_bStartAttach = true;
		}
		if (m_fStartTime + m_fDelayTime <= NcEffectBehaviour.GetEngineTime())
		{
			CreateAttachPrefab();
			if ((0f < m_fRepeatTime && m_nRepeatCount == 0) || m_nCreateCount < m_nRepeatCount)
			{
				m_fStartTime = NcEffectBehaviour.GetEngineTime();
				m_fDelayTime = m_fRepeatTime;
			}
			else
			{
				base.enabled = false;
			}
		}
	}

	protected override void OnDestroy()
	{
		if (m_bEnabled && NcEffectBehaviour.IsSafe() && m_AttachType == AttachType.Destroy && m_ParticlePrefab != null)
		{
			CreateAttachPrefab();
		}
		base.OnDestroy();
	}

	private void CreateAttachPrefab()
	{
		m_nCreateCount++;
		CreateAttachSharedParticle();
		if ((m_fRepeatTime == 0f || m_AttachType == AttachType.Destroy) && 0 < m_nRepeatCount && m_nCreateCount < m_nRepeatCount)
		{
			CreateAttachPrefab();
		}
	}

	private void CreateAttachSharedParticle()
	{
		if (m_CreateGameObject == null)
		{
			m_CreateGameObject = NsSharedManager.inst.GetSharedParticleGameObject(m_ParticlePrefab);
		}
		if (m_CreateGameObject == null)
		{
			return;
		}
		Vector3 vector = base.transform.position + m_AddStartPos + m_ParticlePrefab.transform.position;
		m_CreateGameObject.transform.position = new Vector3(Random.Range(0f - m_RandomRange.x, m_RandomRange.x) + vector.x, Random.Range(0f - m_RandomRange.y, m_RandomRange.y) + vector.y, Random.Range(0f - m_RandomRange.z, m_RandomRange.z) + vector.z);
		if (m_CreateGameObject.GetComponent<ParticleEmitter>() != null)
		{
			m_CreateGameObject.GetComponent<ParticleEmitter>().Emit(m_EmitCount);
			return;
		}
		if (m_ps == null)
		{
			m_ps = m_CreateGameObject.GetComponent<ParticleSystem>();
		}
		if (m_ps != null)
		{
			m_ps.Emit(m_EmitCount);
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime /= fSpeedRate;
		m_fRepeatTime /= fSpeedRate;
	}
}
