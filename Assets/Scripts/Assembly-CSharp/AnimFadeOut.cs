using System.Collections.Generic;
using UnityEngine;

public class AnimFadeOut : MonoBehaviour
{
	public Animation anim;

	public List<GameObject> MatsToFadeOut;

	public float startFrame;

	public float endFrame;

	public string animName;

	private float frameTimeStart;

	private float frameTimeEnd;

	private float time;

	private void OnEnable()
	{
		frameTimeStart = startFrame / 30f;
		frameTimeEnd = endFrame / 30f;
		time = 0f;
	}

	private void Update()
	{
		if (!(anim != null))
		{
			return;
		}
		if (anim[GetAnimName(animName)].time >= frameTimeStart && anim[GetAnimName(animName)].time <= frameTimeEnd)
		{
			foreach (GameObject item in MatsToFadeOut)
			{
				Material material = item.GetComponent<Renderer>().material;
				float a = Mathf.Lerp(material.color.a, 0f, time);
				material.color = new Color(material.color.r, material.color.g, material.color.b, a);
			}
			if (time < 1f)
			{
				time += Time.deltaTime / (frameTimeEnd - frameTimeStart);
			}
		}
		else if (anim[GetAnimName(animName)].time < frameTimeStart)
		{
			time = 0f;
		}
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
}
