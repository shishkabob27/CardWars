using UnityEngine;

public class CWBattleRingPlayAnim : MonoBehaviour
{
	private Animation anim;

	public GameObject animTarget;

	public string animName;

	private void Start()
	{
		Refresh();
	}

	private void Refresh()
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
		Refresh();
		if (!(anim == null) && anim != null && animName != string.Empty)
		{
			anim.Play("Crash");
			SummonScript component = anim.gameObject.GetComponent<SummonScript>();
			if (component.Idle != string.Empty)
			{
				anim.CrossFadeQueued(component.Idle);
			}
		}
	}

	private string FindAnimName(string str, Animation anim)
	{
		string empty = string.Empty;
		foreach (AnimationState item in anim)
		{
			if (item.name.EndsWith(str))
			{
				empty = item.name;
			}
		}
		return empty;
	}
}
