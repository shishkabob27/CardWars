using UnityEngine;

public class NcChangeAlpha : NcEffectBehaviour
{
	public enum TARGET_TYPE
	{
		MeshColor,
		MaterialColor
	}

	public enum CHANGE_MODE
	{
		FromTo
	}

	public TARGET_TYPE m_TargetType = TARGET_TYPE.MaterialColor;

	public float m_fDelayTime = 2f;

	public float m_fChangeTime = 1f;

	public bool m_bRecursively = true;

	public CHANGE_MODE m_ChangeMode;

	public float m_fFromAlphaValue = 1f;

	public float m_fToMeshValue;

	protected float m_fStartTime;

	protected float m_fStartChangeTime;

	public static NcChangeAlpha SetChangeTime(GameObject baseGameObject, float fLifeTime, float fChangeTime, float fFromMeshAlphaValue, float fToMeshAlphaValue)
	{
		NcChangeAlpha ncChangeAlpha = baseGameObject.AddComponent<NcChangeAlpha>();
		ncChangeAlpha.SetChangeTime(fLifeTime, fChangeTime, fFromMeshAlphaValue, fToMeshAlphaValue);
		return ncChangeAlpha;
	}

	public void SetChangeTime(float fDelayTime, float fChangeTime, float fFromAlphaValue, float fToAlphaValue)
	{
		m_fDelayTime = fDelayTime;
		m_fChangeTime = fChangeTime;
		m_fFromAlphaValue = fFromAlphaValue;
		m_fToMeshValue = fToAlphaValue;
		if (NcEffectBehaviour.IsActive(base.gameObject))
		{
			Start();
			Update();
		}
	}

	public void Restart()
	{
		m_fStartTime = NcEffectBehaviour.GetEngineTime();
		m_fStartChangeTime = 0f;
		ChangeToAlpha(0f);
	}

	private void Awake()
	{
		m_fStartTime = 0f;
		m_fStartChangeTime = 0f;
	}

	private void Start()
	{
		Restart();
	}

	private void Update()
	{
		if (0f < m_fStartChangeTime)
		{
			if (0f < m_fChangeTime)
			{
				float num = (NcEffectBehaviour.GetEngineTime() - m_fStartChangeTime) / m_fChangeTime;
				if (1f < num)
				{
					num = 1f;
				}
				ChangeToAlpha(num);
			}
			else
			{
				ChangeToAlpha(1f);
			}
		}
		else if (0f < m_fStartTime && m_fStartTime + m_fDelayTime <= NcEffectBehaviour.GetEngineTime())
		{
			StartChange();
		}
	}

	private void StartChange()
	{
		m_fStartChangeTime = NcEffectBehaviour.GetEngineTime();
	}

	private void ChangeToAlpha(float fElapsedRate)
	{
		float num = Mathf.Lerp(m_fFromAlphaValue, m_fToMeshValue, fElapsedRate);
		if (m_TargetType == TARGET_TYPE.MeshColor)
		{
			MeshFilter[] array = ((!m_bRecursively) ? base.transform.GetComponents<MeshFilter>() : base.transform.GetComponentsInChildren<MeshFilter>(true));
			for (int i = 0; i < array.Length; i++)
			{
				Color[] colors = array[i].mesh.colors;
				for (int j = 0; j < colors.Length; j++)
				{
					Color color = colors[j];
					color.a = num;
					colors[j] = color;
				}
				array[i].mesh.colors = colors;
			}
		}
		else
		{
			Renderer[] array2 = ((!m_bRecursively) ? base.transform.GetComponents<Renderer>() : base.transform.GetComponentsInChildren<Renderer>(true));
			foreach (Renderer renderer in array2)
			{
				string materialColorName = NcEffectBehaviour.GetMaterialColorName(renderer.sharedMaterial);
				if (materialColorName != null)
				{
					Color color2 = renderer.material.GetColor(materialColorName);
					color2.a = num;
					renderer.material.SetColor(materialColorName, color2);
				}
			}
		}
		if (fElapsedRate == 1f && num == 0f)
		{
			NcEffectBehaviour.SetActiveRecursively(base.gameObject, false);
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime /= fSpeedRate;
		m_fChangeTime /= fSpeedRate;
	}
}
