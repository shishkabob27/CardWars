using UnityEngine;

public class FxmTestMain : MonoBehaviour
{
	private FxmTestMain()
	{
	}

	public GUISkin m_GuiMainSkin;
	public FxmTestMouse m_FXMakerMouse;
	public FxmTestControls m_FXMakerControls;
	public AnimationCurve m_SimulateArcCurve;
	public GameObject m_GroupList;
	public int m_CurrentGroupIndex;
	public GameObject m_PrefabList;
	public int m_CurrentPrefabIndex;
	public bool m_bAutoChange;
	public bool m_bAutoSetting;
}
