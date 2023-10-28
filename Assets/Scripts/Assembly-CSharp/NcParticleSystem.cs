using System.Reflection;
using UnityEngine;

public class NcParticleSystem : NcEffectBehaviour
{
	public enum ParticleDestruct
	{
		NONE,
		COLLISION,
		WORLD_Y
	}

	protected const int m_nAllocBufCount = 50;

	protected bool m_bDisabledEmit;

	public float m_fStartDelayTime;

	public bool m_bBurst;

	public float m_fBurstRepeatTime = 0.5f;

	public int m_nBurstRepeatCount;

	public int m_fBurstEmissionCount = 10;

	public float m_fEmitTime;

	public float m_fSleepTime;

	public bool m_bScaleWithTransform = true;

	public bool m_bWorldSpace = true;

	public float m_fStartSizeRate = 1f;

	public float m_fStartLifeTimeRate = 1f;

	public float m_fStartEmissionRate = 1f;

	public float m_fStartSpeedRate = 1f;

	public float m_fRenderLengthRate = 1f;

	public float m_fLegacyMinMeshNormalVelocity = 10f;

	public float m_fLegacyMaxMeshNormalVelocity = 10f;

	public float m_fShurikenSpeedRate = 1f;

	protected bool m_bStart;

	protected Vector3 m_OldPos = Vector3.zero;

	protected bool m_bLegacyRuntimeScale = true;

	public ParticleDestruct m_ParticleDestruct;

	public LayerMask m_CollisionLayer = -1;

	public float m_fCollisionRadius = 0.3f;

	public float m_fDestructPosY = 0.2f;

	public GameObject m_AttachPrefab;

	public float m_fPrefabScale = 1f;

	public float m_fPrefabSpeed = 1f;

	public float m_fPrefabLifeTime = 2f;

	protected bool m_bSleep;

	protected float m_fStartTime;

	protected float m_fDurationStartTime;

	protected float m_fEmitStartTime;

	protected int m_nCreateCount;

	protected bool m_bScalePreRender;

	protected bool m_bMeshParticleEmitter;

	protected ParticleSystem m_ps;

	protected ParticleEmitter m_pe;

	protected ParticleAnimator m_pa;

	protected ParticleRenderer m_pr;

	protected ParticleSystem.Particle[] m_BufPsParts;

	protected ParticleSystem.Particle[] m_BufColliderOriParts;

	protected ParticleSystem.Particle[] m_BufColliderConParts;

	public void SetDisableEmit()
	{
		m_bDisabledEmit = true;
	}

	public bool IsShuriken()
	{
		return GetComponent<ParticleSystem>() != null;
	}

	public bool IsLegacy()
	{
		return GetComponent<ParticleEmitter>() != null && GetComponent<ParticleEmitter>().enabled;
	}

	public override int GetAnimationState()
	{
		if (!base.enabled || !NcEffectBehaviour.IsActive(base.gameObject))
		{
			return -1;
		}
		if (m_bBurst)
		{
			if (0 < m_nBurstRepeatCount)
			{
				if (m_nCreateCount < m_nBurstRepeatCount)
				{
					return 1;
				}
				return 0;
			}
			return 1;
		}
		if (0f < m_fStartDelayTime)
		{
			return 1;
		}
		if (0f < m_fEmitTime && m_fSleepTime <= 0f)
		{
			if (m_nCreateCount < 1)
			{
				return 1;
			}
			return 0;
		}
		return -1;
	}

	public bool IsMeshParticleEmitter()
	{
		return m_bMeshParticleEmitter;
	}

	private void Awake()
	{
		if (IsShuriken())
		{
			m_ps = GetComponent<ParticleSystem>();
			return;
		}
		m_pe = GetComponent<ParticleEmitter>();
		m_pa = GetComponent<ParticleAnimator>();
		m_pr = GetComponent<ParticleRenderer>();
		if (m_pe != null)
		{
			m_bMeshParticleEmitter = m_pe.ToString().Contains("MeshParticleEmitter");
		}
	}

	private void OnEnable()
	{
		if (m_bScaleWithTransform)
		{
			AddRenderEventCall();
		}
	}

	private void OnDisable()
	{
		if (m_bScaleWithTransform)
		{
			RemoveRenderEventCall();
		}
	}

	private void Start()
	{
		m_bStart = true;
		if (!m_bDisabledEmit)
		{
			m_OldPos = base.transform.position;
			m_fDurationStartTime = (m_fEmitStartTime = (m_fStartTime = NcEffectBehaviour.GetEngineTime()));
			if (IsShuriken())
			{
				ShurikenInitParticle();
			}
			else
			{
				LegacyInitParticle();
			}
			if (m_bBurst || 0f < m_fStartDelayTime)
			{
				SetEnableParticle(false);
			}
		}
	}

	private void Update()
	{
		if (m_bDisabledEmit)
		{
			return;
		}
		if (0f < m_fStartDelayTime)
		{
			if (m_fStartTime + m_fStartDelayTime <= NcEffectBehaviour.GetEngineTime())
			{
				m_fEmitStartTime = NcEffectBehaviour.GetEngineTime();
				m_fDurationStartTime = NcEffectBehaviour.GetEngineTime();
				m_fStartDelayTime = 0f;
				SetEnableParticle(true);
			}
		}
		else if (m_bBurst)
		{
			if (m_fDurationStartTime <= NcEffectBehaviour.GetEngineTime() && (m_nBurstRepeatCount == 0 || m_nCreateCount < m_nBurstRepeatCount))
			{
				m_fDurationStartTime = m_fBurstRepeatTime + NcEffectBehaviour.GetEngineTime();
				m_nCreateCount++;
				if (IsShuriken())
				{
					m_ps.Emit(m_fBurstEmissionCount);
				}
				else if (m_pe != null)
				{
					m_pe.Emit(m_fBurstEmissionCount);
				}
			}
		}
		else if (m_bSleep)
		{
			if (m_fEmitStartTime + m_fEmitTime + m_fSleepTime < NcEffectBehaviour.GetEngineTime())
			{
				SetEnableParticle(true);
				m_fEmitStartTime = NcEffectBehaviour.GetEngineTime();
				m_bSleep = false;
			}
		}
		else if (0f < m_fEmitTime && m_fEmitStartTime + m_fEmitTime < NcEffectBehaviour.GetEngineTime())
		{
			m_nCreateCount++;
			SetEnableParticle(false);
			if (0f < m_fSleepTime)
			{
				m_bSleep = true;
			}
			else
			{
				m_fEmitTime = 0f;
			}
		}
	}

	private void FixedUpdate()
	{
		if (m_ParticleDestruct == ParticleDestruct.NONE)
		{
			return;
		}
		bool flag = false;
		if (IsShuriken())
		{
			if (!(m_ps != null))
			{
				return;
			}
			AllocateParticleSystem(ref m_BufColliderOriParts);
			AllocateParticleSystem(ref m_BufColliderConParts);
			m_ps.GetParticles(m_BufColliderOriParts);
			m_ps.GetParticles(m_BufColliderConParts);
			ShurikenScaleParticle(m_BufColliderConParts, m_ps.particleCount, m_bScaleWithTransform, true);
			for (int i = 0; i < m_ps.particleCount; i++)
			{
				bool flag2 = false;
				Vector3 position = ((!m_bWorldSpace) ? base.transform.TransformPoint(m_BufColliderConParts[i].position) : m_BufColliderConParts[i].position);
				if (m_ParticleDestruct == ParticleDestruct.COLLISION)
				{
					if (Physics.CheckSphere(position, m_fCollisionRadius, m_CollisionLayer))
					{
						flag2 = true;
					}
				}
				else if (m_ParticleDestruct == ParticleDestruct.WORLD_Y && position.y <= m_fDestructPosY)
				{
					flag2 = true;
				}
				if (flag2 && 0f < m_BufColliderOriParts[i].remainingLifetime)
				{
					m_BufColliderOriParts[i].remainingLifetime = 0f;
					flag = true;
					CreateAttachPrefab(position, m_BufColliderConParts[i].size * m_fPrefabScale);
				}
			}
			if (flag)
			{
				m_ps.SetParticles(m_BufColliderOriParts, m_ps.particleCount);
			}
		}
		else
		{
			if (!(m_pe != null))
			{
				return;
			}
			Particle[] particles = m_pe.particles;
			Particle[] particles2 = m_pe.particles;
			LegacyScaleParticle(particles2, m_bScaleWithTransform, true);
			for (int j = 0; j < particles2.Length; j++)
			{
				bool flag3 = false;
				Vector3 position = ((!m_bWorldSpace) ? base.transform.TransformPoint(particles2[j].position) : particles2[j].position);
				if (m_ParticleDestruct == ParticleDestruct.COLLISION)
				{
					if (Physics.CheckSphere(position, m_fCollisionRadius, m_CollisionLayer))
					{
						flag3 = true;
					}
				}
				else if (m_ParticleDestruct == ParticleDestruct.WORLD_Y && position.y <= m_fDestructPosY)
				{
					flag3 = true;
				}
				if (flag3 && 0f < particles[j].energy)
				{
					particles[j].energy = 0f;
					flag = true;
					CreateAttachPrefab(position, particles2[j].size * m_fPrefabScale);
				}
			}
			if (flag)
			{
				m_pe.particles = particles;
			}
		}
	}

	private void OnPreRender()
	{
		if (m_bStart && m_bScaleWithTransform)
		{
			m_bScalePreRender = true;
			if (IsShuriken())
			{
				ShurikenSetRuntimeParticleScale(true);
			}
			else
			{
				LegacySetRuntimeParticleScale(true);
			}
		}
	}

	private void OnPostRender()
	{
		if (!m_bStart)
		{
			return;
		}
		if (m_bScalePreRender)
		{
			if (IsShuriken())
			{
				ShurikenSetRuntimeParticleScale(false);
			}
			else
			{
				LegacySetRuntimeParticleScale(false);
			}
		}
		m_OldPos = base.transform.position;
		m_bScalePreRender = false;
	}

	private void CreateAttachPrefab(Vector3 position, float size)
	{
		if (m_AttachPrefab == null)
		{
			return;
		}
		GameObject gameObject = CreateGameObject(m_AttachPrefab, m_AttachPrefab.transform.position + position, m_AttachPrefab.transform.rotation);
		if (gameObject == null)
		{
			return;
		}
		ChangeParent(NcEffectBehaviour.GetRootInstanceEffect().transform, gameObject.transform, false, null);
		NcTransformTool.CopyLossyToLocalScale(gameObject.transform.lossyScale * size, gameObject.transform);
		NcEffectBehaviour.AdjustSpeedRuntime(gameObject, m_fPrefabSpeed);
		if (0f < m_fPrefabLifeTime)
		{
			NcAutoDestruct ncAutoDestruct = gameObject.GetComponent<NcAutoDestruct>();
			if (ncAutoDestruct == null)
			{
				ncAutoDestruct = gameObject.AddComponent<NcAutoDestruct>();
			}
			ncAutoDestruct.m_fLifeTime = m_fPrefabLifeTime;
		}
	}

	private void AddRenderEventCall()
	{
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera in allCameras)
		{
			NsRenderManager nsRenderManager = camera.GetComponent<NsRenderManager>();
			if (nsRenderManager == null)
			{
				nsRenderManager = camera.gameObject.AddComponent<NsRenderManager>();
			}
			nsRenderManager.AddRenderEventCall(this);
		}
	}

	private void RemoveRenderEventCall()
	{
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera in allCameras)
		{
			NsRenderManager component = camera.GetComponent<NsRenderManager>();
			if (component != null)
			{
				component.RemoveRenderEventCall(this);
			}
		}
	}

	private void SetEnableParticle(bool bEnable)
	{
		if (m_ps != null)
		{
			m_ps.enableEmission = bEnable;
		}
		if (m_pe != null)
		{
			m_pe.emit = bEnable;
		}
	}

	public float GetScaleMinMeshNormalVelocity()
	{
		return m_fLegacyMinMeshNormalVelocity * ((!m_bScaleWithTransform) ? 1f : NcTransformTool.GetTransformScaleMeanValue(base.transform));
	}

	public float GetScaleMaxMeshNormalVelocity()
	{
		return m_fLegacyMaxMeshNormalVelocity * ((!m_bScaleWithTransform) ? 1f : NcTransformTool.GetTransformScaleMeanValue(base.transform));
	}

	private void LegacyInitParticle()
	{
		if (m_pe != null)
		{
			LegacySetParticle();
		}
	}

	private void LegacySetParticle()
	{
		ParticleEmitter pe = m_pe;
		ParticleAnimator pa = m_pa;
		ParticleRenderer pr = m_pr;
		if (pe == null || pr == null)
		{
			return;
		}
		if (m_bLegacyRuntimeScale)
		{
			Vector3 b = Vector3.one * m_fStartSpeedRate;
			float fStartSpeedRate = m_fStartSpeedRate;
			pe.minSize *= m_fStartSizeRate;
			pe.maxSize *= m_fStartSizeRate;
			pe.minEnergy *= m_fStartLifeTimeRate;
			pe.maxEnergy *= m_fStartLifeTimeRate;
			pe.minEmission *= m_fStartEmissionRate;
			pe.maxEmission *= m_fStartEmissionRate;
			pe.worldVelocity = Vector3.Scale(pe.worldVelocity, b);
			pe.localVelocity = Vector3.Scale(pe.localVelocity, b);
			pe.rndVelocity = Vector3.Scale(pe.rndVelocity, b);
			pe.angularVelocity *= fStartSpeedRate;
			pe.rndAngularVelocity *= fStartSpeedRate;
			pe.emitterVelocityScale *= fStartSpeedRate;
			if (pa != null)
			{
				pa.rndForce = Vector3.Scale(pa.rndForce, b);
				pa.force = Vector3.Scale(pa.force, b);
			}
			pr.lengthScale *= m_fRenderLengthRate;
			return;
		}
		Vector3 b2 = ((!m_bScaleWithTransform) ? Vector3.one : pe.transform.lossyScale) * m_fStartSpeedRate;
		float num = ((!m_bScaleWithTransform) ? 1f : NcTransformTool.GetTransformScaleMeanValue(pe.transform)) * m_fStartSpeedRate;
		float num2 = ((!m_bScaleWithTransform) ? 1f : NcTransformTool.GetTransformScaleMeanValue(pe.transform)) * m_fStartSizeRate;
		pe.minSize *= num2;
		pe.maxSize *= num2;
		pe.minEnergy *= m_fStartLifeTimeRate;
		pe.maxEnergy *= m_fStartLifeTimeRate;
		pe.minEmission *= m_fStartEmissionRate;
		pe.maxEmission *= m_fStartEmissionRate;
		pe.worldVelocity = Vector3.Scale(pe.worldVelocity, b2);
		pe.localVelocity = Vector3.Scale(pe.localVelocity, b2);
		pe.rndVelocity = Vector3.Scale(pe.rndVelocity, b2);
		pe.angularVelocity *= num;
		pe.rndAngularVelocity *= num;
		pe.emitterVelocityScale *= num;
		if (pa != null)
		{
			pa.rndForce = Vector3.Scale(pa.rndForce, b2);
			pa.force = Vector3.Scale(pa.force, b2);
		}
		pr.lengthScale *= m_fRenderLengthRate;
	}

	private void LegacyParticleSpeed(float fSpeed)
	{
		ParticleEmitter pe = m_pe;
		ParticleAnimator pa = m_pa;
		ParticleRenderer pr = m_pr;
		if (!(pe == null) && !(pr == null))
		{
			Vector3 b = Vector3.one * fSpeed;
			pe.minEnergy /= fSpeed;
			pe.maxEnergy /= fSpeed;
			pe.worldVelocity = Vector3.Scale(pe.worldVelocity, b);
			pe.localVelocity = Vector3.Scale(pe.localVelocity, b);
			pe.rndVelocity = Vector3.Scale(pe.rndVelocity, b);
			pe.angularVelocity *= fSpeed;
			pe.rndAngularVelocity *= fSpeed;
			pe.emitterVelocityScale *= fSpeed;
			if (pa != null)
			{
				pa.rndForce = Vector3.Scale(pa.rndForce, b);
				pa.force = Vector3.Scale(pa.force, b);
			}
		}
	}

	private void LegacySetRuntimeParticleScale(bool bScale)
	{
		if (m_bLegacyRuntimeScale && m_pe != null)
		{
			Particle[] particles = m_pe.particles;
			m_pe.particles = LegacyScaleParticle(particles, bScale, true);
		}
	}

	public Particle[] LegacyScaleParticle(Particle[] parts, bool bScale, bool bPosUpdate)
	{
		float num = ((!bScale) ? (1f / NcTransformTool.GetTransformScaleMeanValue(base.transform)) : NcTransformTool.GetTransformScaleMeanValue(base.transform));
		for (int i = 0; i < parts.Length; i++)
		{
			if (!IsMeshParticleEmitter())
			{
				if (m_bWorldSpace)
				{
					if (bPosUpdate)
					{
						Vector3 vector = m_OldPos - base.transform.position;
						if (bScale)
						{
							parts[i].position -= vector * (1f - 1f / num);
						}
					}
					parts[i].position -= base.transform.position;
					parts[i].position *= num;
					parts[i].position += base.transform.position;
				}
				else
				{
					parts[i].position *= num;
				}
			}
			parts[i].angularVelocity *= num;
			parts[i].velocity *= num;
			parts[i].size *= num;
		}
		return parts;
	}

	private void ShurikenInitParticle()
	{
		if (m_ps != null)
		{
			m_ps.startSize *= m_fStartSizeRate;
			m_ps.startLifetime *= m_fStartLifeTimeRate;
			m_ps.emissionRate *= m_fStartEmissionRate;
			m_ps.startSpeed *= m_fStartSpeedRate;
			ParticleSystemRenderer component = GetComponent<ParticleSystemRenderer>();
			if (component != null)
			{
				float num = (float)Ng_GetProperty(component, "lengthScale");
				Ng_SetProperty(component, "lengthScale", num * m_fRenderLengthRate);
			}
		}
	}

	private void AllocateParticleSystem(ref ParticleSystem.Particle[] tmpPsParts)
	{
		if (tmpPsParts == null || tmpPsParts.Length < m_ps.particleCount)
		{
			tmpPsParts = new ParticleSystem.Particle[m_ps.particleCount + 50];
		}
	}

	private void ShurikenSetRuntimeParticleScale(bool bScale)
	{
		if (m_ps != null)
		{
			AllocateParticleSystem(ref m_BufPsParts);
			m_ps.GetParticles(m_BufPsParts);
			m_BufPsParts = ShurikenScaleParticle(m_BufPsParts, m_ps.particleCount, bScale, true);
			m_ps.SetParticles(m_BufPsParts, m_ps.particleCount);
		}
	}

	public ParticleSystem.Particle[] ShurikenScaleParticle(ParticleSystem.Particle[] parts, int nCount, bool bScale, bool bPosUpdate)
	{
		float num = ((!bScale) ? (1f / NcTransformTool.GetTransformScaleMeanValue(base.transform)) : NcTransformTool.GetTransformScaleMeanValue(base.transform));
		for (int i = 0; i < nCount; i++)
		{
			if (m_bWorldSpace)
			{
				if (bPosUpdate)
				{
					Vector3 vector = m_OldPos - base.transform.position;
					if (bScale)
					{
						parts[i].position -= vector * (1f - 1f / num);
					}
				}
				parts[i].position -= base.transform.position;
				parts[i].position *= num;
				parts[i].position += base.transform.position;
			}
			else
			{
				parts[i].position *= num;
			}
			parts[i].size *= num;
		}
		return parts;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fStartDelayTime /= fSpeedRate;
		m_fBurstRepeatTime /= fSpeedRate;
		m_fEmitTime /= fSpeedRate;
		m_fSleepTime /= fSpeedRate;
		m_fShurikenSpeedRate *= fSpeedRate;
		LegacyParticleSpeed(fSpeedRate);
		m_fPrefabLifeTime /= fSpeedRate;
		m_fPrefabSpeed *= fSpeedRate;
	}

	public static void Ng_SetProperty(object srcObj, string fieldName, object newValue)
	{
		PropertyInfo property = srcObj.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null && property.CanWrite)
		{
			property.SetValue(srcObj, newValue, null);
		}
	}

	public static object Ng_GetProperty(object srcObj, string fieldName)
	{
		object result = null;
		PropertyInfo property = srcObj.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null && property.CanRead && property.GetIndexParameters().Length == 0)
		{
			result = property.GetValue(srcObj, null);
		}
		return result;
	}
}
