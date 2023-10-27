public class NcChangeAlpha : NcEffectBehaviour
{
	public enum TARGET_TYPE
	{
		MeshColor = 0,
		MaterialColor = 1,
	}

	public enum CHANGE_MODE
	{
		FromTo = 0,
	}

	public TARGET_TYPE m_TargetType;
	public float m_fDelayTime;
	public float m_fChangeTime;
	public bool m_bRecursively;
	public CHANGE_MODE m_ChangeMode;
	public float m_fFromAlphaValue;
	public float m_fToMeshValue;
}
