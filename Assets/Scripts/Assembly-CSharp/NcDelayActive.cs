public class NcDelayActive : NcEffectBehaviour
{
	public string NotAvailable = "This component is not available.";

	public float m_fDelayTime;

	public bool m_bActiveRecursively = true;

	protected float m_fAliveTime;

	public float m_fParentDelayTime;

	protected bool m_bAddedInvoke;

	protected float m_fStartedTime;

	public float GetParentDelayTime(bool bCheckStarted)
	{
		return 0f;
	}
}
