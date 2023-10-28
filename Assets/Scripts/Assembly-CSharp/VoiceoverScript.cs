using UnityEngine;

public class VoiceoverScript : MonoBehaviour
{
	public AudioClip ImmortalMaiseWalker;

	public AudioClip P1_SetupPhase_1;

	public AudioClip P1_SetupPhase_2;

	public AudioClip P1_SetupPhase_3;

	public AudioClip P1_BattlePhase_1;

	public AudioClip P2_SetupPhase_1;

	public AudioClip P2_BattlePhase_1;

	public AudioClip P2_BattlePhase_2;

	public AudioClip P1_Wins;

	public AudioClip P2_Wins;

	private int P1_SetupPhases;

	private int P1_BattlePhases;

	private int P2_SetupPhases;

	private int P2_BattlePhases;

	public Renderer Mouth;

	public Texture Mouth_MPandB;

	public Texture Mouth_AltTH;

	public Texture Mouth_OpenSmall;

	public Texture Mouth_AandI;

	public Texture Mouth_O;

	public Texture Mouth_Consonants;

	public Texture Mouth_LandTH;

	public Texture Mouth_AltConsonants;

	public Texture Mouth_WandQ;

	public Texture Mouth_OpenSmall2;

	public bool JakePlaysMaiseWalker;

	public bool AudioPlayed;

	public float Timer;

	private static VoiceoverScript g_voiceover;

	private void Awake()
	{
		g_voiceover = this;
	}

	public static VoiceoverScript GetInstance()
	{
		return g_voiceover;
	}

	private void Update()
	{
		if (JakePlaysMaiseWalker)
		{
			Timer += Time.deltaTime;
			if (Timer > 1.2f && !AudioPlayed)
			{
				AudioPlayed = true;
				SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), ImmortalMaiseWalker);
			}
			if (Timer > 1.4333f)
			{
				Mouth.material.mainTexture = Mouth_AltTH;
			}
			if (Timer > 1.4667f)
			{
				Mouth.material.mainTexture = Mouth_OpenSmall;
			}
			if (Timer > 1.6333f)
			{
				Mouth.material.mainTexture = Mouth_AandI;
			}
			if ((double)Timer > 1.7)
			{
				Mouth.material.mainTexture = Mouth_MPandB;
			}
			if (Timer > 1.7667f)
			{
				Mouth.material.mainTexture = Mouth_O;
			}
			if (Timer > 1.9667f)
			{
				Mouth.material.mainTexture = Mouth_Consonants;
			}
			if (Timer > 2.0333f)
			{
				Mouth.material.mainTexture = Mouth_AandI;
			}
			if (Timer > 2.1333f)
			{
				Mouth.material.mainTexture = Mouth_LandTH;
			}
			if (Timer > 2.2f)
			{
				Mouth.material.mainTexture = Mouth_MPandB;
			}
			if (Timer > 2.2667f)
			{
				Mouth.material.mainTexture = Mouth_AandI;
			}
			if (Timer > 2.4667f)
			{
				Mouth.material.mainTexture = Mouth_Consonants;
			}
			if (Timer > 2.5333f)
			{
				Mouth.material.mainTexture = Mouth_AltConsonants;
			}
			if (Timer > 2.6f)
			{
				Mouth.material.mainTexture = Mouth_WandQ;
			}
			if (Timer > 2.6333f)
			{
				Mouth.material.mainTexture = Mouth_O;
			}
			if (Timer > 2.6667f)
			{
				Mouth.material.mainTexture = Mouth_AandI;
			}
			if (Timer > 2.8667f)
			{
				Mouth.material.mainTexture = Mouth_Consonants;
			}
			if (Timer > 2.9f)
			{
				Mouth.material.mainTexture = Mouth_AandI;
			}
			if (Timer > 3.1333f)
			{
				Mouth.material.mainTexture = Mouth_OpenSmall2;
			}
			if (Timer > 3.3f)
			{
				Mouth.material.mainTexture = Mouth_MPandB;
				JakePlaysMaiseWalker = false;
				Timer = 0f;
				AudioPlayed = false;
			}
		}
	}

	public void LandscapePhase()
	{
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), GetComponent<AudioSource>().clip);
	}

	public void P1SetupPhase()
	{
		if (P1_SetupPhases == 0)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), P1_SetupPhase_1);
			P1_SetupPhases++;
		}
		else if (P1_SetupPhases == 1)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), P1_SetupPhase_2);
			P1_SetupPhases++;
		}
		else if (P1_SetupPhases == 2)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), P1_SetupPhase_3);
			P1_SetupPhases++;
		}
	}

	public void P1BattlePhase()
	{
		if (P1_BattlePhases == 0)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), P1_BattlePhase_1);
			P1_BattlePhases++;
		}
	}

	public void P2SetupPhase()
	{
		if (P2_SetupPhases == 0)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), P2_SetupPhase_1);
			P2_SetupPhases++;
		}
	}

	public void P2BattlePhase()
	{
		if (P2_BattlePhases == 0)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), P2_BattlePhase_1);
			P2_BattlePhases++;
		}
		else if (P2_BattlePhases == 1)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), P2_BattlePhase_2);
			P2_BattlePhases++;
		}
	}

	public void P1Wins()
	{
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), P1_Wins);
	}

	public void P2Wins()
	{
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), P2_Wins);
	}
}
