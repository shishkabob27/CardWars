using UnityEngine;

public class VOController : MonoBehaviour
{
	public static int VOEVENT_COUNT = 12;

	public AudioClip[] OpeningClips;

	public AudioClip[] WinClips;

	public AudioClip[] LoseClips;

	public AudioClip[] DestroyCreatureClips;

	public AudioClip[] PlayCreatureClips;

	public AudioClip[] PlayBuildingClips;

	public AudioClip[] PlaySpellClips;

	public AudioClip[] CreatureDestroyedClips;

	public AudioClip[] BuildingDestroyedClips;

	public AudioClip[] FloopClips;

	public AudioClip[] HeroDamagedClips;

	public AudioClip[] LeaderAbilityClips;

	private AudioClip[][] VOClips;

	private PlayerType owner;

	public bool IsPlaying
	{
		get
		{
			return GetComponent<AudioSource>().isPlaying;
		}
	}

	public PlayerType Owner
	{
		get
		{
			return owner;
		}
		set
		{
			owner = value;
			VOManager.Instance.AddVOController(owner, this);
		}
	}

	private void Awake()
	{
		VOClips = new AudioClip[VOEVENT_COUNT][];
		VOClips[0] = OpeningClips;
		VOClips[1] = WinClips;
		VOClips[2] = LoseClips;
		VOClips[3] = DestroyCreatureClips;
		VOClips[4] = PlayCreatureClips;
		VOClips[5] = PlayBuildingClips;
		VOClips[6] = PlaySpellClips;
		VOClips[7] = CreatureDestroyedClips;
		VOClips[8] = BuildingDestroyedClips;
		VOClips[9] = FloopClips;
		VOClips[10] = HeroDamagedClips;
		VOClips[11] = LeaderAbilityClips;
	}

	public void PlayEvent(VOEvent vo)
	{
		AudioClip[] array = VOClips[(int)vo];
		if (array.Length > 0)
		{
			int num = Random.Range(0, array.Length);
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), array[num]);
		}
	}
}
