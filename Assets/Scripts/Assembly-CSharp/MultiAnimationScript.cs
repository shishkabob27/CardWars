using UnityEngine;

public class MultiAnimationScript : MonoBehaviour
{
	public string startAnimName;

	public string idleAnimName;

	public string clickAnimName;

	public GameObject OutroEventReceiver;

	public string OutroFunction;

	private Animation anim;

	private bool reverse;

	private bool PlayingReverse;

	public float StartOutroFrame = 0.75f;

	public float ReverseSpeed = -1f;

	private void Start()
	{
		anim = base.gameObject.GetComponent<Animation>();
		if (anim == null)
		{
			anim = base.gameObject.AddComponent<Animation>();
		}
		if (startAnimName != null && startAnimName != string.Empty && !(anim != null))
		{
		}
	}

	private void OnEnable()
	{
		anim = base.gameObject.GetComponent<Animation>();
		if (anim == null)
		{
			anim = base.gameObject.AddComponent<Animation>();
		}
		if (startAnimName != null && startAnimName != string.Empty)
		{
			anim[GetAnimName(startAnimName)].time = 0f;
			anim[GetAnimName(startAnimName)].speed = 1f;
			anim.Play(GetAnimName(startAnimName));
			if (idleAnimName != null && idleAnimName != string.Empty)
			{
				anim.CrossFadeQueued(idleAnimName);
			}
			reverse = false;
			PlayingReverse = false;
		}
	}

	private void OnClick()
	{
		anim = base.gameObject.GetComponent<Animation>();
		if (anim == null)
		{
			anim = base.gameObject.AddComponent<Animation>();
		}
		if (clickAnimName != null && clickAnimName != string.Empty)
		{
			anim.Play(GetAnimName(clickAnimName));
			if (idleAnimName != null && idleAnimName != string.Empty)
			{
				anim.CrossFadeQueued(GetAnimName(idleAnimName));
			}
			reverse = false;
		}
	}

	private void PlayIdleAnim()
	{
		anim.Rewind();
		if (idleAnimName != null && idleAnimName != string.Empty)
		{
			anim.Play(GetAnimName(idleAnimName));
		}
		reverse = false;
	}

	private string GetAnimName(string str)
	{
		string empty = string.Empty;
		foreach (AnimationState item in anim.GetComponent<Animation>())
		{
			if (item.name.Contains(str))
			{
				empty = item.name;
				break;
			}
		}
		return empty;
	}

	public void PlayReverse()
	{
		if (anim == null)
		{
			anim = base.gameObject.GetComponent<Animation>();
		}
		if (startAnimName != null && startAnimName != string.Empty && anim != null)
		{
			AnimationState animationState = anim[GetAnimName(startAnimName)];
			animationState.speed = ReverseSpeed;
			animationState.time = anim[GetAnimName(startAnimName)].length;
			anim.Play(GetAnimName(startAnimName));
			reverse = true;
			PlayingReverse = true;
		}
	}

	public bool IsPlayingStartAnimRevert()
	{
		return PlayingReverse;
	}

	public void Outro()
	{
		if (reverse && OutroEventReceiver != null && OutroFunction != null)
		{
			OutroEventReceiver.SendMessage(OutroFunction);
		}
	}

	public void Update()
	{
		if (reverse && OutroEventReceiver != null && OutroFunction != null && anim[GetAnimName(startAnimName)].time <= StartOutroFrame)
		{
			Outro();
			reverse = false;
		}
	}
}
