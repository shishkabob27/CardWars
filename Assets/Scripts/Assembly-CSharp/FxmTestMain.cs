using UnityEngine;

public class FxmTestMain : MonoBehaviour
{
	public static FxmTestMain inst;

	public GUISkin m_GuiMainSkin;

	public FxmTestMouse m_FXMakerMouse;

	public FxmTestControls m_FXMakerControls;

	public AnimationCurve m_SimulateArcCurve;

	public GameObject m_GroupList;

	public int m_CurrentGroupIndex;

	public GameObject m_PrefabList;

	public int m_CurrentPrefabIndex;

	public bool m_bAutoChange = true;

	public bool m_bAutoSetting = true;

	protected GameObject m_OriginalEffectObject;

	protected GameObject m_InstanceEffectObject;

	private FxmTestMain()
	{
		inst = this;
	}

	public FxmTestMouse GetFXMakerMouse()
	{
		if (m_FXMakerMouse == null)
		{
			m_FXMakerMouse = GetComponentInChildren<FxmTestMouse>();
		}
		return m_FXMakerMouse;
	}

	public FxmTestControls GetFXMakerControls()
	{
		if (m_FXMakerControls == null)
		{
			m_FXMakerControls = GetComponent<FxmTestControls>();
		}
		return m_FXMakerControls;
	}

	private void Awake()
	{
		NgUtil.LogDevelop("Awake - FXMakerMain");
		GetFXMakerControls().enabled = true;
	}

	private void OnEnable()
	{
		NgUtil.LogDevelop("OnEnable - FXMakerMain");
	}

	private void Start()
	{
		if (0 < m_GroupList.transform.GetChildCount())
		{
			m_PrefabList = m_GroupList.transform.GetChild(0).gameObject;
		}
		if (m_PrefabList != null && 0 < m_PrefabList.transform.GetChildCount())
		{
			m_OriginalEffectObject = m_PrefabList.transform.GetChild(0).gameObject;
			CreateCurrentInstanceEffect(true);
		}
	}

	private void Update()
	{
	}

	public void OnGUI()
	{
		GUI.skin = m_GuiMainSkin;
		float num = Screen.width / 7;
		float num2 = Screen.height / 10;
		m_FXMakerControls.OnGUIControl();
		if (GUI.Button(new Rect(0f, 0f, num, num2), "GPrev"))
		{
			ChangeGroup(false);
		}
		if (GUI.Button(new Rect(num + 10f, 0f, num, num2), "GNext"))
		{
			ChangeGroup(true);
		}
		GUI.Box(new Rect(0f, num2 + 10f, num * 2f + 10f, 20f), m_GroupList.transform.GetChild(m_CurrentGroupIndex).name, GUI.skin.FindStyle("Hierarchy_Button"));
		if (GUI.Button(new Rect((float)Screen.width - num * 2f - 10f, 0f, num, num2), "EPrev"))
		{
			ChangeEffect(false);
		}
		if (GUI.Button(new Rect((float)Screen.width - num, 0f, num, num2), "ENext"))
		{
			ChangeEffect(true);
		}
		m_bAutoChange = GUI.Toggle(new Rect((float)Screen.width - num, num2 + 10f, num, 20f), m_bAutoChange, "AutoChange");
		bool flag = GUI.Toggle(new Rect((float)Screen.width - num * 2f - 10f, num2 + 10f, num, 20f), m_bAutoSetting, "AutoSetting");
		if (flag != m_bAutoSetting)
		{
			m_bAutoSetting = flag;
			if (!flag)
			{
				m_FXMakerControls.SetDefaultSetting();
			}
		}
		float num3 = GUI.VerticalSlider(new Rect(10f, num2 + 10f + 30f, 25f, (float)Screen.height - (num2 + 10f + 50f) - GetFXMakerControls().GetActionToolbarRect().height), GetFXMakerMouse().m_fDistance, GetFXMakerMouse().m_fDistanceMin, GetFXMakerMouse().m_fDistanceMax);
		if (num3 != GetFXMakerMouse().m_fDistance)
		{
			GetFXMakerMouse().SetDistance(num3);
		}
	}

	public void ChangeEffect(bool bNext)
	{
		if (m_PrefabList == null)
		{
			return;
		}
		if (bNext)
		{
			if (m_CurrentPrefabIndex >= m_PrefabList.transform.GetChildCount() - 1)
			{
				ChangeGroup(true);
				return;
			}
			m_CurrentPrefabIndex++;
		}
		else
		{
			if (m_CurrentPrefabIndex == 0)
			{
				ChangeGroup(false);
				return;
			}
			m_CurrentPrefabIndex--;
		}
		m_OriginalEffectObject = m_PrefabList.transform.GetChild(m_CurrentPrefabIndex).gameObject;
		CreateCurrentInstanceEffect(true);
	}

	public bool ChangeGroup(bool bNext)
	{
		if (bNext)
		{
			if (m_CurrentGroupIndex < m_GroupList.transform.GetChildCount() - 1)
			{
				m_CurrentGroupIndex++;
			}
			else
			{
				m_CurrentGroupIndex = 0;
			}
		}
		else if (m_CurrentGroupIndex == 0)
		{
			m_CurrentGroupIndex = m_GroupList.transform.GetChildCount() - 1;
		}
		else
		{
			m_CurrentGroupIndex--;
		}
		m_PrefabList = m_GroupList.transform.GetChild(m_CurrentGroupIndex).gameObject;
		if (m_PrefabList != null && 0 < m_PrefabList.transform.GetChildCount())
		{
			m_CurrentPrefabIndex = 0;
			m_OriginalEffectObject = m_PrefabList.transform.GetChild(m_CurrentPrefabIndex).gameObject;
			CreateCurrentInstanceEffect(true);
			return true;
		}
		return true;
	}

	public bool IsCurrentEffectObject()
	{
		return m_OriginalEffectObject != null;
	}

	public GameObject GetOriginalEffectObject()
	{
		return m_OriginalEffectObject;
	}

	public void ChangeRoot_OriginalEffectObject(GameObject newRoot)
	{
		m_OriginalEffectObject = newRoot;
	}

	public void ChangeRoot_InstanceEffectObject(GameObject newRoot)
	{
		m_InstanceEffectObject = newRoot;
	}

	public GameObject GetInstanceEffectObject()
	{
		return m_InstanceEffectObject;
	}

	public void ClearCurrentEffectObject(GameObject effectRoot, bool bClearEventObject)
	{
		if (bClearEventObject)
		{
			GameObject instanceRoot = GetInstanceRoot();
			if (instanceRoot != null)
			{
				NgObject.RemoveAllChildObject(instanceRoot, true);
			}
		}
		NgObject.RemoveAllChildObject(effectRoot, true);
		m_OriginalEffectObject = null;
		CreateCurrentInstanceEffect(null);
	}

	public void CreateCurrentInstanceEffect(bool bRunAction)
	{
		FxmTestSetting component = m_PrefabList.GetComponent<FxmTestSetting>();
		if (m_bAutoSetting && component != null)
		{
			m_FXMakerControls.AutoSetting(component.m_nPlayIndex, component.m_nTransIndex, component.m_nTransAxis, component.m_fDistPerTime, component.m_nRotateIndex, component.m_nMultiShotCount, component.m_fTransRate, component.m_fStartPosition);
		}
		NgUtil.LogDevelop("CreateCurrentInstanceEffect() - bRunAction - " + bRunAction);
		if (CreateCurrentInstanceEffect(m_OriginalEffectObject) && bRunAction)
		{
			m_FXMakerControls.RunActionControl();
		}
	}

	public GameObject GetInstanceRoot()
	{
		return NcEffectBehaviour.GetRootInstanceEffect();
	}

	private bool CreateCurrentInstanceEffect(GameObject gameObj)
	{
		NgUtil.LogDevelop("CreateCurrentInstanceEffect() - gameObj - " + gameObj);
		GameObject instanceRoot = GetInstanceRoot();
		NgObject.RemoveAllChildObject(instanceRoot, true);
		if (gameObj != null)
		{
			GameObject gameObject = Object.Instantiate(gameObj);
			NcEffectBehaviour.PreloadTexture(gameObject);
			gameObject.transform.parent = instanceRoot.transform;
			m_InstanceEffectObject = gameObject;
			NgObject.SetActiveRecursively(gameObject, true);
			m_FXMakerControls.SetStartTime();
			return true;
		}
		m_InstanceEffectObject = null;
		return false;
	}
}
