using System;
using UnityEngine;

public class NcParticleSpiral : NcEffectBehaviour
{
	public struct SpiralSettings
	{
		public int numArms;

		public int numPPA;

		public float partSep;

		public float turnDist;

		public float vertDist;

		public float originOffset;

		public float turnSpeed;

		public float fade;

		public float size;
	}

	protected const int Min_numArms = 1;

	protected const int Max_numArms = 10;

	protected const int Min_numPPA = 20;

	protected const int Max_numPPA = 60;

	protected const float Min_partSep = -0.3f;

	protected const float Max_partSep = 0.3f;

	protected const float Min_turnDist = -1.5f;

	protected const float Max_turnDist = 1.5f;

	protected const float Min_vertDist = 0f;

	protected const float Max_vertDist = 0.5f;

	protected const float Min_originOffset = -3f;

	protected const float Max_originOffset = 3f;

	protected const float Min_turnSpeed = -180f;

	protected const float Max_turnSpeed = 180f;

	protected const float Min_fade = -1f;

	protected const float Max_fade = 1f;

	protected const float Min_size = -2f;

	protected const float Max_size = 2f;

	public float m_fDelayTime;

	protected float m_fStartTime;

	public GameObject m_ParticlePrefab;

	public int m_nNumberOfArms = 2;

	public int m_nParticlesPerArm = 100;

	public float m_fParticleSeparation = 0.05f;

	public float m_fTurnDistance = 0.5f;

	public float m_fVerticalTurnDistance;

	public float m_fOriginOffset;

	public float m_fTurnSpeed;

	public float m_fFadeValue;

	public float m_fSizeValue;

	public int m_nNumberOfSpawns = 9999999;

	public float m_fSpawnRate = 5f;

	private float timeOfLastSpawn = -1000f;

	private int spawnCount;

	private int totParticles;

	private SpiralSettings defaultSettings;

	public override int GetAnimationState()
	{
		if (base.enabled && NcEffectBehaviour.IsActive(base.gameObject))
		{
			if (NcEffectBehaviour.GetEngineTime() < m_fStartTime + m_fDelayTime + 0.1f)
			{
				return 1;
			}
			return -1;
		}
		return -1;
	}

	public void RandomizeEditor()
	{
		m_nNumberOfArms = UnityEngine.Random.Range(1, 10);
		m_nParticlesPerArm = UnityEngine.Random.Range(20, 60);
		m_fParticleSeparation = UnityEngine.Random.Range(-0.3f, 0.3f);
		m_fTurnDistance = UnityEngine.Random.Range(-1.5f, 1.5f);
		m_fVerticalTurnDistance = UnityEngine.Random.Range(0f, 0.5f);
		m_fOriginOffset = UnityEngine.Random.Range(-3f, 3f);
		m_fTurnSpeed = UnityEngine.Random.Range(-180f, 180f);
		m_fFadeValue = UnityEngine.Random.Range(-1f, 1f);
		m_fSizeValue = UnityEngine.Random.Range(-2f, 2f);
	}

	private void Start()
	{
		m_fStartTime = NcEffectBehaviour.GetEngineTime();
		if (m_ParticlePrefab == null)
		{
			ParticleEmitter component = GetComponent<ParticleEmitter>();
			if (component == null)
			{
				return;
			}
			component.emit = false;
		}
		defaultSettings = getSettings();
	}

	private void SpawnEffect()
	{
		GameObject gameObject;
		if (m_ParticlePrefab != null)
		{
			gameObject = CreateGameObject(m_ParticlePrefab);
			if (gameObject == null)
			{
				return;
			}
			ChangeParent(base.transform, gameObject.transform, true, null);
		}
		else
		{
			gameObject = base.gameObject;
		}
		ParticleEmitter component = gameObject.GetComponent<ParticleEmitter>();
		if (component == null)
		{
			return;
		}
		component.emit = false;
		component.useWorldSpace = false;
		ParticleAnimator component2 = component.transform.GetComponent<ParticleAnimator>();
		if (component2 != null)
		{
			component2.autodestruct = true;
		}
		component.Emit(m_nNumberOfArms * m_nParticlesPerArm);
		Particle[] particles = component.particles;
		float num = (float)Math.PI * 2f / (float)m_nNumberOfArms;
		for (int i = 0; i < m_nNumberOfArms; i++)
		{
			float num2 = 0f;
			float num3 = 0f;
			float f = (float)i * num;
			for (int j = 0; j < m_nParticlesPerArm; j++)
			{
				int num4 = i * m_nParticlesPerArm + j;
				num2 = m_fOriginOffset + m_fTurnDistance * num3;
				Vector3 position = gameObject.transform.localPosition;
				position.x += num2 * Mathf.Cos(num3);
				position.z += num2 * Mathf.Sin(num3);
				float x = position.x * Mathf.Cos(f) + position.z * Mathf.Sin(f);
				float z = (0f - position.x) * Mathf.Sin(f) + position.z * Mathf.Cos(f);
				position.x = x;
				position.z = z;
				position.y += (float)j * m_fVerticalTurnDistance;
				if (component.useWorldSpace)
				{
					position = base.transform.TransformPoint(position);
				}
				particles[num4].position = position;
				num3 += m_fParticleSeparation;
				if (m_fFadeValue != 0f)
				{
					particles[num4].energy = particles[num4].energy * (1f - Mathf.Abs(m_fFadeValue)) + particles[num4].energy * Mathf.Abs(m_fFadeValue) * (float)((!(m_fFadeValue < 0f)) ? (j + 1) : (m_nParticlesPerArm - j)) / (float)m_nParticlesPerArm;
				}
				if (m_fSizeValue != 0f)
				{
					particles[num4].size += Mathf.Abs(m_fSizeValue) * (float)((!(m_fSizeValue < 0f)) ? (j + 1) : (m_nParticlesPerArm - j)) / (float)m_nParticlesPerArm;
				}
			}
		}
		component.particles = particles;
	}

	private void Update()
	{
		if (!(NcEffectBehaviour.GetEngineTime() < m_fStartTime + m_fDelayTime) && m_fTurnSpeed != 0f)
		{
			base.transform.Rotate(base.transform.up * NcEffectBehaviour.GetEngineDeltaTime() * m_fTurnSpeed, Space.World);
		}
	}

	private void LateUpdate()
	{
		if (!(NcEffectBehaviour.GetEngineTime() < m_fStartTime + m_fDelayTime))
		{
			float num = NcEffectBehaviour.GetEngineTime() - timeOfLastSpawn;
			if (m_fSpawnRate <= num && spawnCount < m_nNumberOfSpawns)
			{
				SpawnEffect();
				timeOfLastSpawn = NcEffectBehaviour.GetEngineTime();
				spawnCount++;
			}
		}
	}

	public SpiralSettings getSettings()
	{
		SpiralSettings result = default(SpiralSettings);
		result.numArms = m_nNumberOfArms;
		result.numPPA = m_nParticlesPerArm;
		result.partSep = m_fParticleSeparation;
		result.turnDist = m_fTurnDistance;
		result.vertDist = m_fVerticalTurnDistance;
		result.originOffset = m_fOriginOffset;
		result.turnSpeed = m_fTurnSpeed;
		result.fade = m_fFadeValue;
		result.size = m_fSizeValue;
		return result;
	}

	public SpiralSettings resetEffect(bool killCurrent, SpiralSettings settings)
	{
		if (killCurrent)
		{
			killCurrentEffects();
		}
		m_nNumberOfArms = settings.numArms;
		m_nParticlesPerArm = settings.numPPA;
		m_fParticleSeparation = settings.partSep;
		m_fTurnDistance = settings.turnDist;
		m_fVerticalTurnDistance = settings.vertDist;
		m_fOriginOffset = settings.originOffset;
		m_fTurnSpeed = settings.turnSpeed;
		m_fFadeValue = settings.fade;
		m_fSizeValue = settings.size;
		SpawnEffect();
		timeOfLastSpawn = NcEffectBehaviour.GetEngineTime();
		spawnCount++;
		return getSettings();
	}

	public SpiralSettings resetEffectToDefaults(bool killCurrent)
	{
		return resetEffect(killCurrent, defaultSettings);
	}

	public SpiralSettings randomizeEffect(bool killCurrent)
	{
		if (killCurrent)
		{
			killCurrentEffects();
		}
		RandomizeEditor();
		SpawnEffect();
		timeOfLastSpawn = NcEffectBehaviour.GetEngineTime();
		spawnCount++;
		return getSettings();
	}

	private void killCurrentEffects()
	{
		ParticleEmitter[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleEmitter>();
		ParticleEmitter[] array = componentsInChildren;
		foreach (ParticleEmitter particleEmitter in array)
		{
			ParticleAnimator component = particleEmitter.transform.GetComponent<ParticleAnimator>();
			if (component != null)
			{
				component.autodestruct = true;
			}
			Particle[] particles = particleEmitter.particles;
			for (int j = 0; j < particles.Length; j++)
			{
				particles[j].energy = 0.1f;
			}
			particleEmitter.particles = particles;
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime /= fSpeedRate;
		m_fTurnSpeed *= fSpeedRate;
	}
}
