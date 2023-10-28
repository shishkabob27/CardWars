using UnityEngine;

public class NcRotation : NcEffectBehaviour
{
	public bool m_bWorldSpace;

	public Vector3 m_vRotationValue = new Vector3(0f, 360f, 0f);

	private void Update()
	{
		base.transform.Rotate(NcEffectBehaviour.GetEngineDeltaTime() * m_vRotationValue.x, NcEffectBehaviour.GetEngineDeltaTime() * m_vRotationValue.y, NcEffectBehaviour.GetEngineDeltaTime() * m_vRotationValue.z, (!m_bWorldSpace) ? Space.Self : Space.World);
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_vRotationValue *= fSpeedRate;
	}
}
