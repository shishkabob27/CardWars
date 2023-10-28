using UnityEngine;

public class NcDuplicator : NcEffectBehaviour
{
	public float m_fDuplicateTime = 0.1f;

	public int m_nDuplicateCount = 3;

	public float m_fDuplicateLifeTime;

	public Vector3 m_AddStartPos = Vector3.zero;

	public Vector3 m_AccumStartRot = Vector3.zero;

	public Vector3 m_RandomRange = Vector3.zero;

	protected int m_nCreateCount;

	protected float m_fStartTime;

	protected GameObject m_ClonObject;

	protected bool m_bInvoke;

	public override int GetAnimationState()
	{
		if (base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && (m_nDuplicateCount == 0 || (m_nDuplicateCount != 0 && m_nCreateCount < m_nDuplicateCount)))
		{
			return 1;
		}
		return 0;
	}

	public GameObject GetCloneObject()
	{
		return m_ClonObject;
	}

	private void Awake()
	{
		m_nCreateCount = 0;
		m_fStartTime = 0f - m_fDuplicateTime;
		m_ClonObject = null;
		m_bInvoke = false;
		if (base.enabled && base.transform.parent != null && base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && GetComponent<NcDontActive>() == null)
		{
			InitCloneObject();
		}
	}

	protected override void OnDestroy()
	{
		if (m_ClonObject != null)
		{
			Object.Destroy(m_ClonObject);
		}
		base.OnDestroy();
	}

	private void Start()
	{
		if (m_bInvoke)
		{
			m_fStartTime = NcEffectBehaviour.GetEngineTime();
			CreateCloneObject();
			InvokeRepeating("CreateCloneObject", m_fDuplicateTime, m_fDuplicateTime);
		}
	}

	private void Update()
	{
		if (!m_bInvoke && (m_nDuplicateCount == 0 || m_nCreateCount < m_nDuplicateCount) && m_fStartTime + m_fDuplicateTime <= NcEffectBehaviour.GetEngineTime())
		{
			m_fStartTime = NcEffectBehaviour.GetEngineTime();
			CreateCloneObject();
		}
	}

	private void InitCloneObject()
	{
		if (!(m_ClonObject == null))
		{
			return;
		}
		m_ClonObject = CreateGameObject(base.gameObject);
		NcEffectBehaviour.HideNcDelayActive(m_ClonObject);
		NcDuplicator component = m_ClonObject.GetComponent<NcDuplicator>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		NcDelayActive component2 = m_ClonObject.GetComponent<NcDelayActive>();
		if (component2 != null)
		{
			Object.Destroy(component2);
		}
		Component[] components = base.transform.GetComponents<Component>();
		for (int i = 0; i < components.Length; i++)
		{
			if (!(components[i] is Transform) && !(components[i] is NcDuplicator))
			{
				Object.Destroy(components[i]);
			}
		}
		NcEffectBehaviour.RemoveAllChildObject(base.gameObject, false);
	}

	private void CreateCloneObject()
	{
		if (m_ClonObject == null)
		{
			return;
		}
		GameObject gameObject = ((!(base.transform.parent == null)) ? CreateGameObject(base.transform.parent.gameObject, m_ClonObject) : CreateGameObject(base.gameObject));
		NcEffectBehaviour.SetActiveRecursively(gameObject, true);
		if (0f < m_fDuplicateLifeTime)
		{
			NcAutoDestruct ncAutoDestruct = gameObject.GetComponent<NcAutoDestruct>();
			if (ncAutoDestruct == null)
			{
				ncAutoDestruct = gameObject.AddComponent<NcAutoDestruct>();
			}
			ncAutoDestruct.m_fLifeTime = m_fDuplicateLifeTime;
		}
		Vector3 position = gameObject.transform.position;
		gameObject.transform.position = new Vector3(Random.Range(0f - m_RandomRange.x, m_RandomRange.x) + position.x, Random.Range(0f - m_RandomRange.y, m_RandomRange.y) + position.y, Random.Range(0f - m_RandomRange.z, m_RandomRange.z) + position.z);
		gameObject.transform.position += m_AddStartPos;
		gameObject.transform.localRotation *= Quaternion.Euler(m_AccumStartRot.x * (float)m_nCreateCount, m_AccumStartRot.y * (float)m_nCreateCount, m_AccumStartRot.z * (float)m_nCreateCount);
		gameObject.name = gameObject.name + " " + m_nCreateCount;
		m_nCreateCount++;
		if (m_bInvoke && m_nDuplicateCount <= m_nCreateCount)
		{
			CancelInvoke("CreateCloneObject");
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDuplicateTime /= fSpeedRate;
		m_fDuplicateLifeTime /= fSpeedRate;
		if (bRuntime && m_ClonObject != null)
		{
			NcEffectBehaviour.AdjustSpeedRuntime(m_ClonObject, fSpeedRate);
		}
	}
}
