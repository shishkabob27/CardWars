using UnityEngine;
using System.Collections;

public class iTween : MonoBehaviour
{
	public enum LoopType
	{
		none = 0,
		loop = 1,
		pingPong = 2,
	}

	public enum EaseType
	{
		easeInQuad = 0,
		easeOutQuad = 1,
		easeInOutQuad = 2,
		easeInCubic = 3,
		easeOutCubic = 4,
		easeInOutCubic = 5,
		easeInQuart = 6,
		easeOutQuart = 7,
		easeInOutQuart = 8,
		easeInQuint = 9,
		easeOutQuint = 10,
		easeInOutQuint = 11,
		easeInSine = 12,
		easeOutSine = 13,
		easeInOutSine = 14,
		easeInExpo = 15,
		easeOutExpo = 16,
		easeInOutExpo = 17,
		easeInCirc = 18,
		easeOutCirc = 19,
		easeInOutCirc = 20,
		linear = 21,
		spring = 22,
		easeInBounce = 23,
		easeOutBounce = 24,
		easeInOutBounce = 25,
		easeInBack = 26,
		easeOutBack = 27,
		easeInOutBack = 28,
		easeInElastic = 29,
		easeOutElastic = 30,
		easeInOutElastic = 31,
		punch = 32,
	}

	private iTween(Hashtable h)
	{
	}

	public string id;
	public string type;
	public string method;
	public EaseType easeType;
	public float time;
	public float delay;
	public LoopType loopType;
	public bool isRunning;
	public bool isPaused;
	public string _name;
}
