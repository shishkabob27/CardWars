using UnityEngine;

public class NcAutoDestruct : NcEffectBehaviour
{
	public enum CollisionType
	{
		NONE,
		COLLISION,
		WORLD_Y
	}

	public float m_fLifeTime = 2f;

	public float m_fSmoothDestroyTime;

	public bool m_bDisableEmit = true;

	public bool m_bSmoothHide = true;

	public bool m_bMeshFilterOnlySmoothHide;

	protected bool m_bEndNcCurveAnimation;

	public CollisionType m_CollisionType;

	public LayerMask m_CollisionLayer = -1;

	public float m_fCollisionRadius = 0.3f;

	public float m_fDestructPosY = 0.2f;

	protected float m_fStartTime;

	protected float m_fStartDestroyTime;

	protected NcCurveAnimation m_NcCurveAnimation;

	public static NcAutoDestruct CreateAutoDestruct(GameObject baseGameObject, float fLifeTime, float fDestroyTime, bool bSmoothHide, bool bMeshFilterOnlySmoothHide)
	{
		NcAutoDestruct ncAutoDestruct = baseGameObject.AddComponent<NcAutoDestruct>();
		ncAutoDestruct.m_fLifeTime = fLifeTime;
		ncAutoDestruct.m_fSmoothDestroyTime = fDestroyTime;
		ncAutoDestruct.m_bSmoothHide = bSmoothHide;
		ncAutoDestruct.m_bMeshFilterOnlySmoothHide = bMeshFilterOnlySmoothHide;
		if (NcEffectBehaviour.IsActive(baseGameObject))
		{
			ncAutoDestruct.Start();
			ncAutoDestruct.Update();
		}
		return ncAutoDestruct;
	}

	private void Awake()
	{
		m_bEndNcCurveAnimation = false;
		m_fStartTime = 0f;
		m_NcCurveAnimation = null;
	}

	private void Start()
	{
		m_fStartTime = NcEffectBehaviour.GetEngineTime();
		if (m_bEndNcCurveAnimation)
		{
			m_NcCurveAnimation = GetComponent<NcCurveAnimation>();
		}
	}

	private void Update()
	{
		if (0f < m_fStartDestroyTime)
		{
			if (!(0f < m_fSmoothDestroyTime))
			{
				return;
			}
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
				AutoDestruct();
			}
		}
		else
		{
			if (0f < m_fStartTime && m_fStartTime + m_fLifeTime <= NcEffectBehaviour.GetEngineTime())
			{
				StartDestroy();
			}
			if (m_bEndNcCurveAnimation && m_NcCurveAnimation != null && 1f <= m_NcCurveAnimation.GetElapsedRate())
			{
				StartDestroy();
			}
		}
	}

	private void FixedUpdate()
	{
		if (0f < m_fStartDestroyTime)
		{
			return;
		}
		bool flag = false;
		if (m_CollisionType == CollisionType.NONE)
		{
			return;
		}
		if (m_CollisionType == CollisionType.COLLISION)
		{
			if (Physics.CheckSphere(base.transform.position, m_fCollisionRadius, m_CollisionLayer))
			{
				flag = true;
			}
		}
		else if (m_CollisionType == CollisionType.WORLD_Y && base.transform.position.y <= m_fDestructPosY)
		{
			flag = true;
		}
		if (flag)
		{
			StartDestroy();
		}
	}

	private void StartDestroy()
	{
		if (m_fSmoothDestroyTime <= 0f)
		{
			AutoDestruct();
			return;
		}
		m_fStartDestroyTime = NcEffectBehaviour.GetEngineTime();
		if (m_bDisableEmit)
		{
			DisableEmit();
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fLifeTime /= fSpeedRate;
		m_fSmoothDestroyTime /= fSpeedRate;
	}

	private void AutoDestruct()
	{
		Object.DestroyObject(base.gameObject);
	}
}
