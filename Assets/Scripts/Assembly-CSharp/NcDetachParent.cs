using UnityEngine;

public class NcDetachParent : NcEffectBehaviour
{
	public bool m_bFollowParentTransform = true;

	public bool m_bParentHideToStartDestroy = true;

	public float m_fSmoothDestroyTime = 2f;

	public bool m_bDisableEmit = true;

	public bool m_bSmoothHide = true;

	public bool m_bMeshFilterOnlySmoothHide;

	protected bool m_bStartDetach;

	protected float m_fStartDestroyTime;

	protected GameObject m_ParentGameObject;

	protected NcDetachObject m_ncDetachObject;

	protected NcTransformTool m_OriginalPos = new NcTransformTool();

	public void SetDestroyValue(bool bParentHideToStart, bool bStartDisableEmit, float fSmoothDestroyTime, bool bSmoothHide, bool bMeshFilterOnlySmoothHide)
	{
		m_bParentHideToStartDestroy = bParentHideToStart;
		m_bDisableEmit = bStartDisableEmit;
		m_bSmoothHide = bSmoothHide;
		m_fSmoothDestroyTime = fSmoothDestroyTime;
		m_bMeshFilterOnlySmoothHide = bMeshFilterOnlySmoothHide;
	}

	protected override void OnDestroy()
	{
		if (m_ncDetachObject != null)
		{
			Object.Destroy(m_ncDetachObject);
		}
		base.OnDestroy();
	}

	private void Update()
	{
		if (!m_bStartDetach)
		{
			m_bStartDetach = true;
			if (base.transform.parent != null)
			{
				m_ParentGameObject = base.transform.parent.gameObject;
				m_ncDetachObject = NcDetachObject.Create(m_ParentGameObject, base.transform.gameObject);
			}
			GameObject rootInstanceEffect = NcEffectBehaviour.GetRootInstanceEffect();
			if (m_bFollowParentTransform)
			{
				m_OriginalPos.SetLocalTransform(base.transform);
				ChangeParent(rootInstanceEffect.transform, base.transform, false, null);
				m_OriginalPos.CopyToLocalTransform(base.transform);
			}
			else
			{
				ChangeParent(rootInstanceEffect.transform, base.transform, false, null);
			}
			if (!m_bParentHideToStartDestroy)
			{
				StartDestroy();
			}
		}
		if (0f < m_fStartDestroyTime)
		{
			if (0f < m_fSmoothDestroyTime)
			{
				if (m_bSmoothHide)
				{
					float num = 1f - (NcEffectBehaviour.GetEngineTime() - m_fStartDestroyTime) / m_fSmoothDestroyTime;
					if (num < 0f)
					{
						num = 0f;
					}
					if (m_bMeshFilterOnlySmoothHide)
					{
						MeshFilter[] componentsInChildren = base.transform.GetComponentsInChildren<MeshFilter>(true);
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							Color[] colors = componentsInChildren[i].mesh.colors;
							for (int j = 0; j < colors.Length; j++)
							{
								Color color = colors[j];
								color.a = Mathf.Min(color.a, num);
								colors[j] = color;
							}
							componentsInChildren[i].mesh.colors = colors;
						}
					}
					else
					{
						Renderer[] componentsInChildren2 = base.transform.GetComponentsInChildren<Renderer>(true);
						foreach (Renderer renderer in componentsInChildren2)
						{
							string materialColorName = NcEffectBehaviour.GetMaterialColorName(renderer.sharedMaterial);
							if (materialColorName != null)
							{
								Color color2 = renderer.material.GetColor(materialColorName);
								color2.a = Mathf.Min(color2.a, num);
								renderer.material.SetColor(materialColorName, color2);
							}
						}
					}
				}
				if (m_fStartDestroyTime + m_fSmoothDestroyTime < NcEffectBehaviour.GetEngineTime())
				{
					Object.Destroy(base.gameObject);
				}
			}
		}
		else if (m_bParentHideToStartDestroy && (m_ParentGameObject == null || !NcEffectBehaviour.IsActive(m_ParentGameObject)))
		{
			StartDestroy();
		}
		if (m_bFollowParentTransform && m_ParentGameObject != null && m_ParentGameObject.transform != null)
		{
			NcTransformTool ncTransformTool = new NcTransformTool();
			ncTransformTool.SetTransform(m_OriginalPos);
			ncTransformTool.AddTransform(m_ParentGameObject.transform);
			ncTransformTool.CopyToLocalTransform(base.transform);
		}
	}

	private void StartDestroy()
	{
		m_fStartDestroyTime = NcEffectBehaviour.GetEngineTime();
		if (m_bDisableEmit)
		{
			DisableEmit();
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fSmoothDestroyTime /= fSpeedRate;
	}
}
