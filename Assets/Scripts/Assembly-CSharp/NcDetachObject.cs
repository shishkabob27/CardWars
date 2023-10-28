using UnityEngine;

public class NcDetachObject : NcEffectBehaviour
{
	public GameObject m_LinkGameObject;

	public static NcDetachObject Create(GameObject parentObj, GameObject linkObject)
	{
		NcDetachObject ncDetachObject = parentObj.AddComponent<NcDetachObject>();
		ncDetachObject.m_LinkGameObject = linkObject;
		return ncDetachObject;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		if (bRuntime)
		{
			NcEffectBehaviour.AdjustSpeedRuntime(m_LinkGameObject, fSpeedRate);
		}
	}
}
