using UnityEngine;

public class FxmTestSetting : MonoBehaviour
{
	public int m_nPlayIndex;

	public int m_nTransIndex;

	public FxmTestControls.AXIS m_nTransAxis = FxmTestControls.AXIS.Z;

	public float m_fTransRate = 1f;

	public float m_fStartPosition;

	public float m_fDistPerTime = 10f;

	public int m_nRotateIndex;

	public int m_nMultiShotCount = 1;

	protected float[] m_fPlayToolbarTimes = new float[8] { 1f, 1f, 1f, 0.3f, 0.6f, 1f, 2f, 3f };

	public string[] GetPlayContents()
	{
		int num = 3;
		GUIContent[] hcEffectControls_Play = FxmTestControls.GetHcEffectControls_Play(0f, 0f, m_fPlayToolbarTimes[1], m_fPlayToolbarTimes[num], m_fPlayToolbarTimes[num + 1], m_fPlayToolbarTimes[num + 2], m_fPlayToolbarTimes[num + 3], m_fPlayToolbarTimes[num + 4]);
		return NgConvert.ContentsToStrings(hcEffectControls_Play);
	}
}
