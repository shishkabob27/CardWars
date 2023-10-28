using UnityEngine;

public class CWPlayCreatureAnimation : MonoBehaviour
{
	private Animation anim;

	public GameObject animTarget;

	public string animName;

	private void Start()
	{
		if ((bool)animTarget)
		{
			anim = animTarget.GetComponent<Animation>();
		}
		else
		{
			anim = GetComponent<Animation>();
		}
	}

	private void OnClick()
	{
		if (anim == null)
		{
			anim = animTarget.GetComponent<Animation>();
		}
		if (anim != null && animName != string.Empty)
		{
			anim.Play(FindAnimName(animName, anim));
			anim.CrossFadeQueued(FindAnimName("Idle", anim));
		}
	}

	private string FindAnimName(string str, Animation anim)
	{
		string empty = string.Empty;
		foreach (AnimationState item in anim.GetComponent<Animation>())
		{
			if (item.name.EndsWith(str))
			{
				empty = item.name;
				break;
			}
		}
		return empty;
	}
}
