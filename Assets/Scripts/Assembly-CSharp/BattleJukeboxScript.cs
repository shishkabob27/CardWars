using UnityEngine;

public class BattleJukeboxScript : MonoBehaviour
{
	public AudioClip DefaultTheme;

	public AudioClip Theme1;

	public AudioClip Theme2;

	public AudioClip Theme3;

	public AudioClip Theme4;

	public AudioClip Theme5;

	public AudioClip Theme6;

	public AudioClip Theme7;

	public AudioClip Theme8;

	public AudioClip Theme9;

	public AudioClip Theme10;

	public AudioClip Theme11;

	public AudioClip BattleIntroTheme;

	public AudioClip BattleEndTheme;

	private float volume = 1f;

	private bool initialized;

	public float Volume
	{
		get
		{
			return volume;
		}
		set
		{
			volume = value;
			SLOTMusic sLOTMusic = base.gameObject.GetComponent(typeof(SLOTMusic)) as SLOTMusic;
			if (sLOTMusic != null)
			{
				sLOTMusic.volume = volume;
				SLOTGameSingleton<SLOTAudioManager>.GetInstance().UpdateAudioVolumes();
			}
		}
	}

	public void Refresh()
	{
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		CharacterData opponent = activeQuest.Opponent;
		string battleMusic = opponent.BattleMusic;
		AudioClip audioClip = DefaultTheme;
		if (battleMusic != null)
		{
			if (Theme1 != null && Theme1.name == battleMusic)
			{
				audioClip = Theme1;
			}
			if (Theme2 != null && Theme2.name == battleMusic)
			{
				audioClip = Theme2;
			}
			if (Theme3 != null && Theme3.name == battleMusic)
			{
				audioClip = Theme3;
			}
			if (Theme4 != null && Theme4.name == battleMusic)
			{
				audioClip = Theme4;
			}
			if (Theme5 != null && Theme5.name == battleMusic)
			{
				audioClip = Theme5;
			}
			if (Theme6 != null && Theme6.name == battleMusic)
			{
				audioClip = Theme6;
			}
			if (Theme7 != null && Theme7.name == battleMusic)
			{
				audioClip = Theme7;
			}
			if (Theme8 != null && Theme8.name == battleMusic)
			{
				audioClip = Theme8;
			}
			if (Theme9 != null && Theme9.name == battleMusic)
			{
				audioClip = Theme9;
			}
			if (Theme10 != null && Theme10.name == battleMusic)
			{
				audioClip = Theme10;
			}
			if (Theme11 != null && Theme11.name == battleMusic)
			{
				audioClip = Theme11;
			}
		}
		AudioSource component = GetComponent<AudioSource>();
		if (component != null && audioClip != null)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayMusic(component, audioClip, 0f, volume);
		}
	}

	public void PlayBattleIntro()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (component != null && BattleIntroTheme != null)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(component, BattleIntroTheme);
		}
	}

	public void PlayBattleEnd()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (component != null && BattleEndTheme != null)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayMusic(component, BattleEndTheme, 0f, 1f);
		}
	}

	private void Update()
	{
		if (!initialized)
		{
			SessionManager instance = SessionManager.GetInstance();
			if (instance.IsReady())
			{
				initialized = true;
			}
		}
	}
}
