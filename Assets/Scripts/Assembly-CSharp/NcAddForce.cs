using UnityEngine;

public class NcAddForce : NcEffectBehaviour
{
	public Vector3 m_AddForce = new Vector3(0f, 300f, 0f);

	public Vector3 m_RandomRange = new Vector3(100f, 100f, 100f);

	public ForceMode m_ForceMode;

	private void Start()
	{
		if (base.enabled)
		{
			AddForce();
		}
	}

	private void AddForce()
	{
		if (GetComponent<Rigidbody>() != null)
		{
			Vector3 force = new Vector3(Random.Range(0f - m_RandomRange.x, m_RandomRange.x) + m_AddForce.x, Random.Range(0f - m_RandomRange.y, m_RandomRange.y) + m_AddForce.y, Random.Range(0f - m_RandomRange.z, m_RandomRange.z) + m_AddForce.z);
			GetComponent<Rigidbody>().AddForce(force, m_ForceMode);
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
	}
}
