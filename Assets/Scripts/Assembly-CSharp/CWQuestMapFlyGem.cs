using UnityEngine;

public class CWQuestMapFlyGem : MonoBehaviour
{
	public enum EarningType
	{
		GEM = 0,
		HEART = 1,
	}

	public GameObject flyingObj;
	public Transform parentTr;
	public Transform start;
	public Transform dest;
	public float time;
	public Vector3 heartDestinationSize;
	public Vector3 gemDestinationSize;
	public AudioClip gemEarnedSound;
	public EarningType earningType;
}
