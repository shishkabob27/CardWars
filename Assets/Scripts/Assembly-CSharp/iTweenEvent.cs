using UnityEngine;

public class iTweenEvent : MonoBehaviour
{
	public enum TweenType
	{
		AudioFrom = 0,
		AudioTo = 1,
		AudioUpdate = 2,
		CameraFadeFrom = 3,
		CameraFadeTo = 4,
		ColorFrom = 5,
		ColorTo = 6,
		ColorUpdate = 7,
		FadeFrom = 8,
		FadeTo = 9,
		FadeUpdate = 10,
		LookFrom = 11,
		LookTo = 12,
		LookUpdate = 13,
		MoveAdd = 14,
		MoveBy = 15,
		MoveFrom = 16,
		MoveTo = 17,
		MoveUpdate = 18,
		PunchPosition = 19,
		PunchRotation = 20,
		PunchScale = 21,
		RotateAdd = 22,
		RotateBy = 23,
		RotateFrom = 24,
		RotateTo = 25,
		RotateUpdate = 26,
		ScaleAdd = 27,
		ScaleBy = 28,
		ScaleFrom = 29,
		ScaleTo = 30,
		ScaleUpdate = 31,
		ShakePosition = 32,
		ShakeRotation = 33,
		ShakeScale = 34,
		Stab = 35,
	}

	public string tweenName;
	public bool playAutomatically;
	public float delay;
	public TweenType type;
	public bool showIconInInspector;
	[SerializeField]
	private string[] keys;
	[SerializeField]
	private int[] indexes;
	[SerializeField]
	private string[] metadatas;
	[SerializeField]
	private int[] ints;
	[SerializeField]
	private float[] floats;
	[SerializeField]
	private bool[] bools;
	[SerializeField]
	private string[] strings;
	[SerializeField]
	private Vector3[] vector3s;
	[SerializeField]
	private Color[] colors;
	[SerializeField]
	private Space[] spaces;
	[SerializeField]
	private iTween.EaseType[] easeTypes;
	[SerializeField]
	private iTween.LoopType[] loopTypes;
	[SerializeField]
	private GameObject[] gameObjects;
	[SerializeField]
	private Transform[] transforms;
	[SerializeField]
	private AudioClip[] audioClips;
	[SerializeField]
	private AudioSource[] audioSources;
	[SerializeField]
	private ArrayIndexes[] vector3Arrays;
	[SerializeField]
	private ArrayIndexes[] transformArrays;
	[SerializeField]
	private iTweenPath[] paths;
}
