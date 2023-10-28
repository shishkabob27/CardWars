using UnityEngine;

public class NcAttachSound : NcEffectBehaviour
{
	public enum PLAY_TYPE
	{
		StopAndPlay,
		UniquePlay,
		MultiPlay
	}

	public PLAY_TYPE m_PlayType;

	public bool m_bPlayOnActive;

	public float m_fDelayTime;

	public float m_fRepeatTime;

	public int m_nRepeatCount;

	public AudioClip m_AudioClip;

	public int m_nPriority = 128;

	public bool m_bLoop;

	public float m_fVolume = 1f;

	public float m_fPitch = 1f;

	protected AudioSource m_AudioSource;

	protected float m_fStartTime;

	protected int m_nCreateCount;

	protected bool m_bStartAttach;

	protected bool m_bEnable = true;

	public override int GetAnimationState()
	{
		if ((base.enabled && NcEffectBehaviour.IsActive(base.gameObject)) || (m_AudioSource != null && (m_AudioSource.isPlaying || NcEffectBehaviour.GetEngineTime() < m_fStartTime + m_fDelayTime)))
		{
			return 1;
		}
		return 0;
	}

	public void Replay()
	{
		m_bStartAttach = false;
		m_bEnable = true;
	}

	private void OnEnable()
	{
		if (m_bPlayOnActive)
		{
			Replay();
		}
	}

	private void Update()
	{
		if (m_AudioClip == null)
		{
			base.enabled = false;
		}
		else
		{
			if (!m_bEnable)
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
				CreateAttachSound();
				if (0f < m_fRepeatTime && (m_nRepeatCount == 0 || m_nCreateCount < m_nRepeatCount))
				{
					m_fStartTime = NcEffectBehaviour.GetEngineTime();
					m_fDelayTime = m_fRepeatTime;
				}
				else
				{
					m_bEnable = false;
				}
			}
		}
	}

	public void CreateAttachSound()
	{
		if (m_PlayType == PLAY_TYPE.MultiPlay)
		{
			if (m_AudioSource == null)
			{
				m_AudioSource = base.gameObject.AddComponent<AudioSource>();
			}
			m_AudioSource.clip = m_AudioClip;
			m_AudioSource.priority = m_nPriority;
			m_AudioSource.loop = m_bLoop;
			m_AudioSource.volume = m_fVolume;
			m_AudioSource.pitch = m_fPitch;
			m_AudioSource.playOnAwake = false;
			m_AudioSource.Play();
		}
		else
		{
			NsSharedManager.inst.PlaySharedAudioSource(m_PlayType == PLAY_TYPE.UniquePlay, m_AudioClip, m_nPriority, m_bLoop, m_fVolume, m_fPitch);
		}
		m_nCreateCount++;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime /= fSpeedRate;
		m_fRepeatTime /= fSpeedRate;
	}
}
