using UnityEngine;

public class FxmTestControls : MonoBehaviour
{
	public enum AXIS
	{
		X,
		Y,
		Z
	}

	protected const int m_nRepeatIndex = 3;

	public bool m_bMinimize;

	protected int m_nTriangles;

	protected int m_nVertices;

	protected int m_nMeshCount;

	protected int m_nParticleCount;

	protected int m_nPlayIndex;

	protected int m_nTransIndex;

	protected float[] m_fPlayToolbarTimes = new float[8] { 1f, 1f, 1f, 0.3f, 0.6f, 1f, 2f, 3f };

	protected AXIS m_nTransAxis = AXIS.Z;

	protected float m_fDelayCreateTime = 0.2f;

	protected bool m_bCalledDelayCreate;

	protected bool m_bStartAliveAnimation;

	protected float m_fDistPerTime = 10f;

	protected int m_nRotateIndex;

	protected int m_nMultiShotCount = 1;

	protected float m_fTransRate = 1f;

	protected float m_fStartPosition;

	public float m_fTimeScale = 1f;

	protected float m_fPlayStartTime;

	protected float m_fOldTimeScale = 1f;

	protected float m_fCreateTime;

	public float GetTimeScale()
	{
		return m_fTimeScale;
	}

	public bool IsRepeat()
	{
		return 3 <= m_nPlayIndex;
	}

	public bool IsAutoRepeat()
	{
		return m_nPlayIndex == 0;
	}

	public float GetRepeatTime()
	{
		return m_fPlayToolbarTimes[m_nPlayIndex];
	}

	public void SetStartTime()
	{
		m_fPlayStartTime = Time.time;
	}

	private void LoadPrefs()
	{
		m_nPlayIndex = PlayerPrefs.GetInt("FxmTestControls.m_nPlayIndex", m_nPlayIndex);
		m_nTransIndex = PlayerPrefs.GetInt("FxmTestControls.m_nTransIndex", m_nTransIndex);
		m_fTimeScale = PlayerPrefs.GetFloat("FxmTestControls.m_fTimeScale", m_fTimeScale);
		m_fDistPerTime = PlayerPrefs.GetFloat("FxmTestControls.m_fDistPerTime", m_fDistPerTime);
		m_nRotateIndex = PlayerPrefs.GetInt("FxmTestControls.m_nRotateIndex", m_nRotateIndex);
		m_nTransAxis = (AXIS)PlayerPrefs.GetInt("FxmTestControls.m_nTransAxis", (int)m_nTransAxis);
		m_bMinimize = PlayerPrefs.GetInt("FxmTestControls.m_bMinimize", m_bMinimize ? 1 : 0) == 1;
		SetTimeScale(m_fTimeScale);
	}

	private void SavePrefs()
	{
		PlayerPrefs.SetInt("FxmTestControls.m_nPlayIndex", m_nPlayIndex);
		PlayerPrefs.SetInt("FxmTestControls.m_nTransIndex", m_nTransIndex);
		PlayerPrefs.SetFloat("FxmTestControls.m_fTimeScale", m_fTimeScale);
		PlayerPrefs.SetFloat("FxmTestControls.m_fDistPerTime", m_fDistPerTime);
		PlayerPrefs.SetInt("FxmTestControls.m_nRotateIndex", m_nRotateIndex);
		PlayerPrefs.SetInt("FxmTestControls.m_nTransAxis", (int)m_nTransAxis);
	}

	public void SetDefaultSetting()
	{
		m_nPlayIndex = 0;
		m_nTransIndex = 0;
		m_nTransAxis = AXIS.Z;
		m_fDistPerTime = 10f;
		m_nRotateIndex = 0;
		m_nMultiShotCount = 1;
		m_fTransRate = 1f;
		m_fStartPosition = 0f;
		SavePrefs();
	}

	public void AutoSetting(int nPlayIndex, int nTransIndex, AXIS nTransAxis, float fDistPerTime, int nRotateIndex, int nMultiShotCount, float fTransRate, float fStartAdjustRate)
	{
		m_nPlayIndex = nPlayIndex;
		m_nTransIndex = nTransIndex;
		m_nTransAxis = nTransAxis;
		m_fDistPerTime = fDistPerTime;
		m_nRotateIndex = nRotateIndex;
		m_nMultiShotCount = nMultiShotCount;
		m_fTransRate = fTransRate;
		m_fStartPosition = fStartAdjustRate;
	}

	private void Awake()
	{
		NgUtil.LogDevelop("Awake - m_FXMakerControls");
		LoadPrefs();
	}

	private void OnEnable()
	{
		NgUtil.LogDevelop("OnEnable - m_FXMakerControls");
		LoadPrefs();
	}

	private void Start()
	{
	}

	private void Update()
	{
		m_fTimeScale = Time.timeScale;
		if (FxmTestMain.inst.GetInstanceEffectObject() == null && !IsAutoRepeat())
		{
			DelayCreateInstanceEffect(false);
			return;
		}
		NgObject.GetMeshInfo(NcEffectBehaviour.GetRootInstanceEffect(), true, out m_nVertices, out m_nTriangles, out m_nMeshCount);
		m_nParticleCount = 0;
		ParticleSystem[] componentsInChildren = NcEffectBehaviour.GetRootInstanceEffect().GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			m_nParticleCount += particleSystem.particleCount;
		}
		ParticleEmitter[] componentsInChildren2 = NcEffectBehaviour.GetRootInstanceEffect().GetComponentsInChildren<ParticleEmitter>();
		ParticleEmitter[] array2 = componentsInChildren2;
		foreach (ParticleEmitter particleEmitter in array2)
		{
			m_nParticleCount += particleEmitter.particleCount;
		}
		if (m_fDelayCreateTime < Time.time - m_fPlayStartTime)
		{
			if (IsRepeat() && m_fCreateTime + GetRepeatTime() < Time.time)
			{
				DelayCreateInstanceEffect(false);
			}
			if (m_nTransIndex == 0 && IsAutoRepeat() && !m_bCalledDelayCreate && !IsAliveAnimation())
			{
				DelayCreateInstanceEffect(false);
			}
		}
	}

	private bool IsAliveAnimation()
	{
		GameObject rootInstanceEffect = NcEffectBehaviour.GetRootInstanceEffect();
		Transform[] componentsInChildren = rootInstanceEffect.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			int num = -1;
			int num2 = -1;
			bool flag = false;
			NcEffectBehaviour[] components = transform.GetComponents<NcEffectBehaviour>();
			NcEffectBehaviour[] array2 = components;
			foreach (NcEffectBehaviour ncEffectBehaviour in array2)
			{
				switch (ncEffectBehaviour.GetAnimationState())
				{
				case 1:
					num = 1;
					break;
				case 0:
					num = 0;
					break;
				}
			}
			if (transform.GetComponent<ParticleSystem>() != null)
			{
				num2 = 0;
				if (NgObject.IsActive(transform.gameObject) && ((transform.GetComponent<ParticleSystem>().enableEmission && transform.GetComponent<ParticleSystem>().IsAlive()) || 0 < transform.GetComponent<ParticleSystem>().particleCount))
				{
					num2 = 1;
				}
			}
			if (num2 < 1 && transform.GetComponent<ParticleEmitter>() != null)
			{
				num2 = 0;
				if (NgObject.IsActive(transform.gameObject) && (transform.GetComponent<ParticleEmitter>().emit || 0 < transform.GetComponent<ParticleEmitter>().particleCount))
				{
					num2 = 1;
				}
			}
			if (transform.GetComponent<Renderer>() != null && transform.GetComponent<Renderer>().enabled && NgObject.IsActive(transform.gameObject))
			{
				flag = true;
			}
			if (0 < num)
			{
				return true;
			}
			if (num2 == 1)
			{
				return true;
			}
			if (flag && (transform.GetComponent<MeshFilter>() != null || transform.GetComponent<TrailRenderer>() != null || transform.GetComponent<LineRenderer>() != null))
			{
				return true;
			}
		}
		return false;
	}

	public void OnGUIControl()
	{
		GUI.Window(10, GetActionToolbarRect(), winActionToolbar, "PrefabSimulate - " + ((!FxmTestMain.inst.IsCurrentEffectObject()) ? "Not Selected" : FxmTestMain.inst.GetOriginalEffectObject().name));
	}

	public Rect GetActionToolbarRect()
	{
		float num = (float)Screen.height * ((!m_bMinimize) ? 0.35f : 0.1f);
		return new Rect(0f, (float)Screen.height - num, Screen.width, num);
	}

	private string NgTooltipTooltip(string str)
	{
		return str;
	}

	public static GUIContent[] GetHcEffectControls_Play(float fAutoRet, float timeScale, float timeOneShot1, float timeRepeat1, float timeRepeat2, float timeRepeat3, float timeRepeat4, float timeRepeat5)
	{
		return new GUIContent[8]
		{
			new GUIContent("AutoRet", string.Empty),
			new GUIContent(timeScale.ToString("0.00") + "x S", string.Empty),
			new GUIContent(timeOneShot1.ToString("0.0") + "x S", string.Empty),
			new GUIContent(timeRepeat1.ToString("0.0") + "s R", string.Empty),
			new GUIContent(timeRepeat2.ToString("0.0") + "s R", string.Empty),
			new GUIContent(timeRepeat3.ToString("0.0") + "s R", string.Empty),
			new GUIContent(timeRepeat4.ToString("0.0") + "s R", string.Empty),
			new GUIContent(timeRepeat5.ToString("0.0") + "s R", string.Empty)
		};
	}

	public static GUIContent[] GetHcEffectControls_Trans(AXIS nTransAxis)
	{
		return new GUIContent[8]
		{
			new GUIContent("Stop", string.Empty),
			new GUIContent(nTransAxis.ToString() + " Move", string.Empty),
			new GUIContent(nTransAxis.ToString() + " Scale", string.Empty),
			new GUIContent("Arc", string.Empty),
			new GUIContent("Fall", string.Empty),
			new GUIContent("Raise", string.Empty),
			new GUIContent("Circle", string.Empty),
			new GUIContent("Tornado", string.Empty)
		};
	}

	public static GUIContent[] GetHcEffectControls_Rotate()
	{
		return new GUIContent[2]
		{
			new GUIContent("Rot", string.Empty),
			new GUIContent("Fix", string.Empty)
		};
	}

	private void winActionToolbar(int id)
	{
		Rect actionToolbarRect = GetActionToolbarRect();
		string text = string.Empty;
		string str = string.Empty;
		int num = 10;
		int count = 5;
		m_bMinimize = GUI.Toggle(new Rect(3f, 1f, FXMakerLayout.m_fMinimizeClickWidth, FXMakerLayout.m_fMinimizeClickHeight), m_bMinimize, "Mini");
		if (GUI.changed)
		{
			PlayerPrefs.SetInt("FxmTestControls.m_bMinimize", m_bMinimize ? 1 : 0);
		}
		GUI.changed = false;
		Rect innerHorizontalRect;
		Rect childVerticalRect;
		if (FXMakerLayout.m_bMinimizeAll || m_bMinimize)
		{
			count = 1;
			childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 0, 1);
			if (FxmTestMain.inst.IsCurrentEffectObject())
			{
				text = string.Format("P={0} M={1} T={2}", m_nParticleCount, m_nMeshCount, m_nTriangles);
				str = string.Format("ParticleCount = {0} MeshCount = {1}\n Mesh: Triangles = {2} Vertices = {3}", m_nParticleCount, m_nMeshCount, m_nTriangles, m_nVertices);
			}
			GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 0, 2), text);
			if (FxmTestMain.inst.IsCurrentEffectObject())
			{
				float rightValue = ((3 > m_nPlayIndex) ? 10f : m_fPlayToolbarTimes[m_nPlayIndex]);
				childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 0, 1);
				GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 2, 2), "ElapsedTime " + (Time.time - m_fPlayStartTime).ToString("0.000"));
				innerHorizontalRect = FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 4, 4);
				innerHorizontalRect.y += 5f;
				GUI.HorizontalSlider(innerHorizontalRect, Time.time - m_fPlayStartTime, 0f, rightValue);
				childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 0, 1);
				if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 8, 2), "Restart"))
				{
					CreateInstanceEffect();
				}
			}
			return;
		}
		childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 0, 2);
		if ((bool)NcEffectBehaviour.GetRootInstanceEffect())
		{
			text = string.Format("P = {0}\nM = {1}\nT = {2}", m_nParticleCount, m_nMeshCount, m_nTriangles);
			str = string.Format("ParticleCount = {0} MeshCount = {1}\n Mesh: Triangles = {2} Vertices = {3}", m_nParticleCount, m_nMeshCount, m_nTriangles, m_nVertices);
		}
		GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 0, 1), new GUIContent(text, NgTooltipTooltip(str)));
		if (FxmTestMain.inst.IsCurrentEffectObject())
		{
			bool flag = false;
			GUIContent[] hcEffectControls_Play = GetHcEffectControls_Play(0f, m_fTimeScale, m_fPlayToolbarTimes[1], m_fPlayToolbarTimes[3], m_fPlayToolbarTimes[4], m_fPlayToolbarTimes[5], m_fPlayToolbarTimes[6], m_fPlayToolbarTimes[7]);
			childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 0, 1);
			GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 1, 1), new GUIContent("Play", string.Empty));
			int nPlayIndex = FXMakerLayout.TooltipSelectionGrid(actionToolbarRect, FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 2, 8), m_nPlayIndex, hcEffectControls_Play, hcEffectControls_Play.Length);
			if (GUI.changed)
			{
				flag = true;
			}
			GUIContent[] hcEffectControls_Trans = GetHcEffectControls_Trans(m_nTransAxis);
			childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 1, 1);
			GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 1, 1), new GUIContent("Trans", string.Empty));
			int num2 = FXMakerLayout.TooltipSelectionGrid(actionToolbarRect, FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 2, 8), m_nTransIndex, hcEffectControls_Trans, hcEffectControls_Trans.Length);
			if (GUI.changed)
			{
				flag = true;
				if ((num2 == 1 || num2 == 2) && Input.GetMouseButtonUp(1))
				{
					if (m_nTransAxis == AXIS.Z)
					{
						m_nTransAxis = AXIS.X;
					}
					else
					{
						m_nTransAxis++;
					}
					PlayerPrefs.SetInt("FxmTestControls.m_nTransAxis", (int)m_nTransAxis);
				}
			}
			if (flag)
			{
				FxmTestMain.inst.CreateCurrentInstanceEffect(false);
				RunActionControl(nPlayIndex, num2);
				PlayerPrefs.SetInt("FxmTestControls.m_nPlayIndex", m_nPlayIndex);
				PlayerPrefs.SetInt("FxmTestControls.m_nTransIndex", m_nTransIndex);
			}
		}
		float fDistPerTime = m_fDistPerTime;
		childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 2, 1);
		GUIContent gUIContent = new GUIContent("DistPerTime", string.Empty);
		GUIContent gUIContent2 = gUIContent;
		gUIContent2.text = gUIContent2.text + " " + m_fDistPerTime.ToString("00.00");
		GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 0, 2), gUIContent);
		innerHorizontalRect = FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 2, 5);
		innerHorizontalRect.y += 5f;
		fDistPerTime = GUI.HorizontalSlider(innerHorizontalRect, fDistPerTime, 0.1f, 40f);
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num * 2, 14, 1), new GUIContent("<", string.Empty)))
		{
			fDistPerTime = (int)(fDistPerTime - 1f);
		}
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num * 2, 15, 1), new GUIContent(">", string.Empty)))
		{
			fDistPerTime = (int)(fDistPerTime + 1f);
		}
		if (fDistPerTime != m_fDistPerTime)
		{
			m_fDistPerTime = ((fDistPerTime != 0f) ? fDistPerTime : 0.1f);
			PlayerPrefs.SetFloat("FxmTestControls.m_fDistPerTime", m_fDistPerTime);
			if (0 < m_nTransIndex)
			{
				CreateInstanceEffect();
			}
		}
		if (NgLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 9, 1), new GUIContent("Multi", m_nMultiShotCount.ToString()), true))
		{
			if (Input.GetMouseButtonUp(0))
			{
				m_nMultiShotCount++;
				if (4 < m_nMultiShotCount)
				{
					m_nMultiShotCount = 1;
				}
			}
			else
			{
				m_nMultiShotCount = 1;
			}
			CreateInstanceEffect();
		}
		GUIContent[] hcEffectControls_Rotate = GetHcEffectControls_Rotate();
		childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 2, 1);
		int num3 = FXMakerLayout.TooltipSelectionGrid(actionToolbarRect, FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 8, 1), m_nRotateIndex, hcEffectControls_Rotate, hcEffectControls_Rotate.Length);
		if (num3 != m_nRotateIndex)
		{
			m_nRotateIndex = num3;
			PlayerPrefs.SetInt("FxmTestControls.m_nRotateIndex", m_nRotateIndex);
			if (0 < m_nTransIndex)
			{
				CreateInstanceEffect();
			}
		}
		float fTimeScale = m_fTimeScale;
		childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 3, 1);
		gUIContent = new GUIContent("TimeScale", string.Empty);
		GUIContent gUIContent3 = gUIContent;
		gUIContent3.text = gUIContent3.text + " " + m_fTimeScale.ToString("0.00");
		GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 0, 2), gUIContent);
		innerHorizontalRect = FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 2, 5);
		innerHorizontalRect.y += 5f;
		fTimeScale = GUI.HorizontalSlider(innerHorizontalRect, fTimeScale, 0f, 3f);
		if (fTimeScale == 0f)
		{
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 7, 1), new GUIContent("Resume", string.Empty)))
			{
				fTimeScale = m_fOldTimeScale;
			}
		}
		else if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 7, 1), new GUIContent("Pause", string.Empty)))
		{
			fTimeScale = 0f;
		}
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 8, 1), new GUIContent("Reset", string.Empty)))
		{
			fTimeScale = 1f;
		}
		SetTimeScale(fTimeScale);
		if (FxmTestMain.inst.IsCurrentEffectObject())
		{
			float rightValue2 = ((3 > m_nPlayIndex) ? 10f : m_fPlayToolbarTimes[m_nPlayIndex]);
			childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 4, 1);
			gUIContent = new GUIContent("ElapsedTime", string.Empty);
			GUIContent gUIContent4 = gUIContent;
			gUIContent4.text = gUIContent4.text + " " + (Time.time - m_fPlayStartTime).ToString("0.000");
			GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 0, 2), gUIContent);
			innerHorizontalRect = FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 2, 5);
			innerHorizontalRect.y += 5f;
			GUI.HorizontalSlider(innerHorizontalRect, Time.time - m_fPlayStartTime, 0f, rightValue2);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num * 2, 14, 1), new GUIContent("+.5", string.Empty)))
			{
				SetTimeScale(1f);
				Invoke("invokeStopTimer", 0.5f);
			}
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num * 2, 15, 1), new GUIContent("+.1", string.Empty)))
			{
				SetTimeScale(0.4f);
				Invoke("invokeStopTimer", 0.1f);
			}
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num * 2, 16, 1), new GUIContent("+.05", string.Empty)))
			{
				SetTimeScale(0.2f);
				Invoke("invokeStopTimer", 0.05f);
			}
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num * 2, 17, 1), new GUIContent("+.01", string.Empty)))
			{
				SetTimeScale(0.04f);
				Invoke("invokeStopTimer", 0.01f);
			}
			childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 3, 2);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 9, 1), new GUIContent("Restart", string.Empty)))
			{
				CreateInstanceEffect();
			}
		}
	}

	private void invokeStopTimer()
	{
		SetTimeScale(0f);
	}

	public void RunActionControl()
	{
		RunActionControl(m_nPlayIndex, m_nTransIndex);
	}

	protected void RunActionControl(int nPlayIndex, int nTransIndex)
	{
		NgUtil.LogDevelop("RunActionControl() - nPlayIndex " + nPlayIndex);
		CancelInvoke();
		m_bCalledDelayCreate = false;
		ResumeTimeScale();
		m_bStartAliveAnimation = false;
		switch (nPlayIndex)
		{
		case 2:
			SetTimeScale(m_fPlayToolbarTimes[nPlayIndex]);
			break;
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
			if (nPlayIndex != m_nPlayIndex)
			{
				nTransIndex = 0;
			}
			break;
		}
		if (0 < nTransIndex)
		{
			float num = ((!(Camera.main != null)) ? 1f : (Vector3.Magnitude(Camera.main.transform.position) * 0.8f)) * m_fTransRate;
			GameObject instanceEffectObject = FxmTestMain.inst.GetInstanceEffectObject();
			GameObject gameObject = NgObject.CreateGameObject(instanceEffectObject.transform.parent.gameObject, "simulate");
			FxmTestSimulate fxmTestSimulate = gameObject.AddComponent<FxmTestSimulate>();
			instanceEffectObject.transform.parent = gameObject.transform;
			FxmTestMain.inst.ChangeRoot_InstanceEffectObject(gameObject);
			fxmTestSimulate.Init(this, m_nMultiShotCount);
			switch (nTransIndex)
			{
			case 1:
				fxmTestSimulate.SimulateMove(m_nTransAxis, num, m_fDistPerTime, m_nRotateIndex == 0);
				break;
			case 2:
				fxmTestSimulate.SimulateScale(m_nTransAxis, num * 0.3f, m_fStartPosition, m_fDistPerTime, m_nRotateIndex == 0);
				break;
			case 3:
				fxmTestSimulate.SimulateArc(num * 0.7f, m_fDistPerTime, m_nRotateIndex == 0);
				break;
			case 4:
				fxmTestSimulate.SimulateFall(num * 0.7f, m_fDistPerTime, m_nRotateIndex == 0);
				break;
			case 5:
				fxmTestSimulate.SimulateRaise(num * 0.7f, m_fDistPerTime, m_nRotateIndex == 0);
				break;
			case 6:
				fxmTestSimulate.SimulateCircle(num * 0.5f, m_fDistPerTime, m_nRotateIndex == 0);
				break;
			case 7:
				fxmTestSimulate.SimulateTornado(num * 0.3f, num * 0.7f, m_fDistPerTime, m_nRotateIndex == 0);
				break;
			}
		}
		if (0 < nTransIndex && 3 <= nPlayIndex)
		{
			nPlayIndex = 0;
		}
		m_nPlayIndex = nPlayIndex;
		m_nTransIndex = nTransIndex;
		if (IsRepeat())
		{
			m_fCreateTime = Time.time;
		}
	}

	public void OnActionTransEnd()
	{
		DelayCreateInstanceEffect(true);
	}

	private void RotateFront(Transform target)
	{
		Quaternion localRotation = FxmTestMain.inst.GetOriginalEffectObject().transform.localRotation;
		Vector3 eulerAngles = localRotation.eulerAngles;
		switch (m_nRotateIndex)
		{
		case 1:
			eulerAngles.y += 90f;
			break;
		case 2:
			eulerAngles.y -= 90f;
			break;
		case 3:
			eulerAngles.z -= 90f;
			break;
		}
		localRotation.eulerAngles = eulerAngles;
		target.localRotation = localRotation;
	}

	private void DelayCreateInstanceEffect(bool bEndMove)
	{
		m_bCalledDelayCreate = true;
		Invoke("NextInstanceEffect", (float)((!bEndMove) ? 1 : 3) * m_fDelayCreateTime);
	}

	private void NextInstanceEffect()
	{
		if (FxmTestMain.inst.m_bAutoChange)
		{
			FxmTestMain.inst.ChangeEffect(true);
		}
		else
		{
			CreateInstanceEffect();
		}
	}

	private void CreateInstanceEffect()
	{
		if (FxmTestMain.inst.IsCurrentEffectObject())
		{
			FxmTestMain.inst.CreateCurrentInstanceEffect(true);
		}
	}

	private void SetTimeScale(float timeScale)
	{
		if (m_fTimeScale != timeScale || m_fTimeScale != Time.timeScale)
		{
			if (timeScale == 0f && m_fTimeScale != 0f)
			{
				m_fOldTimeScale = m_fTimeScale;
			}
			m_fTimeScale = timeScale;
			if (0.01f <= m_fTimeScale)
			{
				PlayerPrefs.SetFloat("FxmTestControls.m_fTimeScale", m_fTimeScale);
			}
			Time.timeScale = m_fTimeScale;
		}
	}

	public void ResumeTimeScale()
	{
		if (m_fTimeScale == 0f)
		{
			SetTimeScale(m_fOldTimeScale);
		}
	}
}
