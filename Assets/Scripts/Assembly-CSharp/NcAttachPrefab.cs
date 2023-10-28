using UnityEngine;

public class NcAttachPrefab : NcEffectBehaviour
{
	public enum AttachType
	{
		Active,
		Destroy
	}

	public AttachType m_AttachType;

	public float m_fDelayTime;

	public float m_fRepeatTime;

	public int m_nRepeatCount;

	public GameObject m_AttachPrefab;

	public float m_fPrefabSpeed = 1f;

	public float m_fPrefabLifeTime;

	public bool m_bWorldSpace;

	public Vector3 m_AddStartPos = Vector3.zero;

	public Vector3 m_AccumStartRot = Vector3.zero;

	public Vector3 m_RandomRange = Vector3.zero;

	public int m_nSpriteFactoryIndex = -1;

	[HideInInspector]
	public bool m_bDetachParent;

	protected float m_fStartTime;

	protected int m_nCreateCount;

	protected bool m_bStartAttach;

	protected GameObject m_CreateGameObject;

	protected bool m_bEnabled;

	public override int GetAnimationState()
	{
		if (base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && m_AttachPrefab != null)
		{
			if (m_AttachType == AttachType.Active && ((m_nRepeatCount == 0 && m_nCreateCount < 1) || (0f < m_fRepeatTime && m_nRepeatCount == 0) || (0 < m_nRepeatCount && m_nCreateCount < m_nRepeatCount)))
			{
				return 1;
			}
			if (m_AttachType == AttachType.Destroy)
			{
				return 1;
			}
		}
		return 0;
	}

	public void UpdateImmediately()
	{
		Update();
	}

	public void CreateAttachInstance()
	{
		CreateAttachGameObject();
	}

	public GameObject GetInstanceObject()
	{
		if (m_CreateGameObject == null)
		{
			UpdateImmediately();
		}
		return m_CreateGameObject;
	}

	public void SetEnable(bool bEnable)
	{
		m_bEnabled = bEnable;
	}

	private void Awake()
	{
		m_bEnabled = base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && GetComponent<NcDontActive>() == null;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_AttachPrefab == null || m_AttachType != 0)
		{
			return;
		}
		if (!m_bStartAttach)
		{
			m_fStartTime = NcEffectBehaviour.GetEngineTime();
			m_bStartAttach = true;
		}
		if (m_fStartTime + m_fDelayTime <= NcEffectBehaviour.GetEngineTime())
		{
			CreateAttachPrefab();
			if ((0f < m_fRepeatTime && m_nRepeatCount == 0) || m_nCreateCount < m_nRepeatCount)
			{
				m_fStartTime = NcEffectBehaviour.GetEngineTime();
				m_fDelayTime = m_fRepeatTime;
			}
			else
			{
				base.enabled = false;
			}
		}
	}

	protected override void OnDestroy()
	{
		if (m_bEnabled && NcEffectBehaviour.IsSafe() && m_AttachType == AttachType.Destroy && m_AttachPrefab != null)
		{
			CreateAttachPrefab();
		}
		base.OnDestroy();
	}

	private void CreateAttachPrefab()
	{
		m_nCreateCount++;
		CreateAttachGameObject();
		if ((m_fRepeatTime == 0f || m_AttachType == AttachType.Destroy) && 0 < m_nRepeatCount && m_nCreateCount < m_nRepeatCount)
		{
			CreateAttachPrefab();
		}
	}

	private void CreateAttachGameObject()
	{
		m_CreateGameObject = CreateGameObject(GetTargetGameObject(), (!(GetTargetGameObject() == base.gameObject)) ? base.transform : null, m_AttachPrefab);
		if (m_CreateGameObject == null)
		{
			return;
		}
		Vector3 position = m_CreateGameObject.transform.position;
		m_CreateGameObject.transform.position = m_AddStartPos + new Vector3(Random.Range(0f - m_RandomRange.x, m_RandomRange.x) + position.x, Random.Range(0f - m_RandomRange.y, m_RandomRange.y) + position.y, Random.Range(0f - m_RandomRange.z, m_RandomRange.z) + position.z);
		m_CreateGameObject.transform.localRotation *= Quaternion.Euler(m_AccumStartRot.x * (float)m_nCreateCount, m_AccumStartRot.y * (float)m_nCreateCount, m_AccumStartRot.z * (float)m_nCreateCount);
		GameObject createGameObject = m_CreateGameObject;
		createGameObject.name = createGameObject.name + " " + m_nCreateCount;
		NcEffectBehaviour.SetActiveRecursively(m_CreateGameObject, true);
		NcEffectBehaviour.AdjustSpeedRuntime(m_CreateGameObject, m_fPrefabSpeed);
		if (0f < m_fPrefabLifeTime)
		{
			NcAutoDestruct ncAutoDestruct = m_CreateGameObject.GetComponent<NcAutoDestruct>();
			if (ncAutoDestruct == null)
			{
				ncAutoDestruct = m_CreateGameObject.AddComponent<NcAutoDestruct>();
			}
			ncAutoDestruct.m_fLifeTime = m_fPrefabLifeTime;
		}
		if (m_bDetachParent)
		{
			NcDetachParent component = m_CreateGameObject.GetComponent<NcDetachParent>();
			if (component == null)
			{
				component = m_CreateGameObject.AddComponent<NcDetachParent>();
			}
		}
		if (0 <= m_nSpriteFactoryIndex)
		{
			NcSpriteFactory component2 = m_CreateGameObject.GetComponent<NcSpriteFactory>();
			if ((bool)component2)
			{
				component2.SetSprite(m_nSpriteFactoryIndex, false);
			}
		}
	}

	private GameObject GetTargetGameObject()
	{
		if (m_bWorldSpace || m_AttachType == AttachType.Destroy)
		{
			return NcEffectBehaviour.GetRootInstanceEffect();
		}
		return base.gameObject;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime /= fSpeedRate;
		m_fRepeatTime /= fSpeedRate;
		m_fPrefabLifeTime /= fSpeedRate;
		m_fPrefabSpeed *= fSpeedRate;
	}

	public static void Ng_ChangeLayerWithChild(GameObject rootObj, int nLayer)
	{
		if (!(rootObj == null))
		{
			rootObj.layer = nLayer;
			for (int i = 0; i < rootObj.transform.GetChildCount(); i++)
			{
				Ng_ChangeLayerWithChild(rootObj.transform.GetChild(i).gameObject, nLayer);
			}
		}
	}
}
