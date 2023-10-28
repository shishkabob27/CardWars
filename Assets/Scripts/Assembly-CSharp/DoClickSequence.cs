using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoClickSequence : MonoBehaviour
{
	public List<string> buttonSequence = new List<string>();

	private void OnClick()
	{
		StartCoroutine(Navigate());
	}

	private IEnumerator Navigate()
	{
		foreach (string objname in buttonSequence)
		{
			GameObject gobj2 = null;
			do
			{
				gobj2 = GameObject.Find(objname);
				if (gobj2 != null)
				{
					gobj2.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				}
				yield return null;
			}
			while (gobj2 == null);
		}
	}
}
