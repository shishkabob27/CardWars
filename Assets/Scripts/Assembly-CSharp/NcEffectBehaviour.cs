using System.Collections.Generic;
using UnityEngine;

public class NcEffectBehaviour : MonoBehaviour
{
	public class _RuntimeIntance
	{
		public GameObject m_ParentGameObject;

		public GameObject m_ChildGameObject;

		public _RuntimeIntance(GameObject parentGameObject, GameObject childGameObject)
		{
			m_ParentGameObject = parentGameObject;
			m_ChildGameObject = childGameObject;
		}
	}

	private static bool m_bShuttingDown;

	private static GameObject m_RootInstance;

	public float m_fUserTag;

	protected MeshFilter m_MeshFilter;

	protected List<Material> m_RuntimeMaterials;

	public NcEffectBehaviour()
	{
		m_MeshFilter = null;
	}

	public static float GetEngineTime()
	{
		if (Time.time == 0f)
		{
			return 1E-06f;
		}
		return Time.time;
	}

	public static float GetEngineDeltaTime()
	{
		return Time.deltaTime;
	}

	public virtual int GetAnimationState()
	{
		return -1;
	}

	public static GameObject GetRootInstanceEffect()
	{
		if (!IsSafe())
		{
			return null;
		}
		if (m_RootInstance == null)
		{
			m_RootInstance = GameObject.Find("_InstanceObject");
			if (m_RootInstance == null)
			{
				m_RootInstance = new GameObject("_InstanceObject");
			}
		}
		return m_RootInstance;
	}

	protected static void SetActive(GameObject target, bool bActive)
	{
		target.SetActive(bActive);
	}

	protected static void SetActiveRecursively(GameObject target, bool bActive)
	{
		int num = target.transform.GetChildCount() - 1;
		while (0 <= num)
		{
			if (num < target.transform.GetChildCount())
			{
				SetActiveRecursively(target.transform.GetChild(num).gameObject, bActive);
			}
			num--;
		}
		target.SetActive(bActive);
	}

	protected static bool IsActive(GameObject target)
	{
		return target.activeInHierarchy && target.activeSelf;
	}

	protected static void RemoveAllChildObject(GameObject parent, bool bImmediate)
	{
		int num = parent.transform.GetChildCount() - 1;
		while (0 <= num)
		{
			if (num < parent.transform.GetChildCount())
			{
				Transform child = parent.transform.GetChild(num);
				if (bImmediate)
				{
					Object.DestroyImmediate(child.gameObject);
				}
				else
				{
					Object.Destroy(child.gameObject);
				}
			}
			num--;
		}
	}

	public static void HideNcDelayActive(GameObject tarObj)
	{
		SetActiveRecursively(tarObj, false);
	}

	public static Texture[] PreloadTexture(GameObject tarObj)
	{
		if (tarObj == null)
		{
			return new Texture[0];
		}
		List<GameObject> list = new List<GameObject>();
		list.Add(tarObj);
		return PreloadTexture(tarObj, list);
	}

	private static Texture[] PreloadTexture(GameObject tarObj, List<GameObject> parentPrefabList)
	{
		if (!IsSafe())
		{
			return null;
		}
		Renderer[] componentsInChildren = tarObj.GetComponentsInChildren<Renderer>(true);
		List<Texture> list = new List<Texture>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length <= 0)
			{
				continue;
			}
			Material[] sharedMaterials = renderer.sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (material != null && material.mainTexture != null)
				{
					list.Add(material.mainTexture);
				}
			}
		}
		NcAttachPrefab[] componentsInChildren2 = tarObj.GetComponentsInChildren<NcAttachPrefab>(true);
		NcAttachPrefab[] array2 = componentsInChildren2;
		foreach (NcAttachPrefab ncAttachPrefab in array2)
		{
			if (ncAttachPrefab.m_AttachPrefab != null)
			{
				Texture[] array3 = PreloadPrefab(ncAttachPrefab.m_AttachPrefab, parentPrefabList, true);
				if (array3 == null)
				{
					ncAttachPrefab.m_AttachPrefab = null;
				}
				else
				{
					list.AddRange(array3);
				}
			}
		}
		NcParticleSystem[] componentsInChildren3 = tarObj.GetComponentsInChildren<NcParticleSystem>(true);
		NcParticleSystem[] array4 = componentsInChildren3;
		foreach (NcParticleSystem ncParticleSystem in array4)
		{
			if (ncParticleSystem.m_AttachPrefab != null)
			{
				Texture[] array5 = PreloadPrefab(ncParticleSystem.m_AttachPrefab, parentPrefabList, true);
				if (array5 == null)
				{
					ncParticleSystem.m_AttachPrefab = null;
				}
				else
				{
					list.AddRange(array5);
				}
			}
		}
		NcSpriteTexture[] componentsInChildren4 = tarObj.GetComponentsInChildren<NcSpriteTexture>(true);
		NcSpriteTexture[] array6 = componentsInChildren4;
		foreach (NcSpriteTexture ncSpriteTexture in array6)
		{
			if (ncSpriteTexture.m_NcSpriteFactoryPrefab != null)
			{
				Texture[] array7 = PreloadPrefab(ncSpriteTexture.m_NcSpriteFactoryPrefab, parentPrefabList, false);
				if (array7 != null)
				{
					list.AddRange(array7);
				}
			}
		}
		NcParticleSpiral[] componentsInChildren5 = tarObj.GetComponentsInChildren<NcParticleSpiral>(true);
		NcParticleSpiral[] array8 = componentsInChildren5;
		foreach (NcParticleSpiral ncParticleSpiral in array8)
		{
			if (ncParticleSpiral.m_ParticlePrefab != null)
			{
				Texture[] array9 = PreloadPrefab(ncParticleSpiral.m_ParticlePrefab, parentPrefabList, false);
				if (array9 != null)
				{
					list.AddRange(array9);
				}
			}
		}
		NcParticleEmit[] componentsInChildren6 = tarObj.GetComponentsInChildren<NcParticleEmit>(true);
		NcParticleEmit[] array10 = componentsInChildren6;
		foreach (NcParticleEmit ncParticleEmit in array10)
		{
			if (ncParticleEmit.m_ParticlePrefab != null)
			{
				Texture[] array11 = PreloadPrefab(ncParticleEmit.m_ParticlePrefab, parentPrefabList, false);
				if (array11 != null)
				{
					list.AddRange(array11);
				}
			}
		}
		NcAttachSound[] componentsInChildren7 = tarObj.GetComponentsInChildren<NcAttachSound>(true);
		NcAttachSound[] array12 = componentsInChildren7;
		foreach (NcAttachSound ncAttachSound in array12)
		{
			if (ncAttachSound.m_AudioClip != null)
			{
			}
		}
		NcSpriteFactory[] componentsInChildren8 = tarObj.GetComponentsInChildren<NcSpriteFactory>(true);
		NcSpriteFactory[] array13 = componentsInChildren8;
		foreach (NcSpriteFactory ncSpriteFactory in array13)
		{
			if (ncSpriteFactory.m_SpriteList == null)
			{
				continue;
			}
			for (int num4 = 0; num4 < ncSpriteFactory.m_SpriteList.Count; num4++)
			{
				if (ncSpriteFactory.m_SpriteList[num4].m_EffectPrefab != null)
				{
					Texture[] array14 = PreloadPrefab(ncSpriteFactory.m_SpriteList[num4].m_EffectPrefab, parentPrefabList, true);
					if (array14 == null)
					{
						ncSpriteFactory.m_SpriteList[num4].m_EffectPrefab = null;
					}
					else
					{
						list.AddRange(array14);
					}
					if (!(ncSpriteFactory.m_SpriteList[num4].m_AudioClip != null))
					{
					}
				}
			}
		}
		return list.ToArray();
	}

	private static Texture[] PreloadPrefab(GameObject tarObj, List<GameObject> parentPrefabList, bool bCheckDup)
	{
		if (parentPrefabList.Contains(tarObj))
		{
			if (bCheckDup)
			{
				string text = string.Empty;
				for (int i = 0; i < parentPrefabList.Count; i++)
				{
					text = text + parentPrefabList[i].name + "/";
				}
				return null;
			}
			return null;
		}
		parentPrefabList.Add(tarObj);
		Texture[] result = PreloadTexture(tarObj, parentPrefabList);
		parentPrefabList.Remove(tarObj);
		return result;
	}

	protected void AddRuntimeMaterial(Material addMaterial)
	{
		if (m_RuntimeMaterials == null)
		{
			m_RuntimeMaterials = new List<Material>();
		}
		if (!m_RuntimeMaterials.Contains(addMaterial))
		{
			m_RuntimeMaterials.Add(addMaterial);
		}
	}

	public static void AdjustSpeedRuntime(GameObject target, float fSpeedRate)
	{
		NcEffectBehaviour[] componentsInChildren = target.GetComponentsInChildren<NcEffectBehaviour>(true);
		NcEffectBehaviour[] array = componentsInChildren;
		foreach (NcEffectBehaviour ncEffectBehaviour in array)
		{
			ncEffectBehaviour.OnUpdateEffectSpeed(fSpeedRate, true);
		}
	}

	public static string GetMaterialColorName(Material mat)
	{
		string[] array = new string[3] { "_Color", "_TintColor", "_EmisColor" };
		if (mat != null)
		{
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (mat.HasProperty(text))
				{
					return text;
				}
			}
		}
		return null;
	}

	protected void DisableEmit()
	{
		NcParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<NcParticleSystem>(true);
		NcParticleSystem[] array = componentsInChildren;
		foreach (NcParticleSystem ncParticleSystem in array)
		{
			if (ncParticleSystem != null)
			{
				ncParticleSystem.SetDisableEmit();
			}
		}
		NcAttachPrefab[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<NcAttachPrefab>(true);
		NcAttachPrefab[] array2 = componentsInChildren2;
		foreach (NcAttachPrefab ncAttachPrefab in array2)
		{
			if (ncAttachPrefab != null)
			{
				ncAttachPrefab.enabled = false;
			}
		}
		ParticleSystem[] componentsInChildren3 = base.gameObject.GetComponentsInChildren<ParticleSystem>(true);
		ParticleSystem[] array3 = componentsInChildren3;
		foreach (ParticleSystem particleSystem in array3)
		{
			if (particleSystem != null)
			{
				particleSystem.enableEmission = false;
			}
		}
		ParticleEmitter[] componentsInChildren4 = base.gameObject.GetComponentsInChildren<ParticleEmitter>(true);
		ParticleEmitter[] array4 = componentsInChildren4;
		foreach (ParticleEmitter particleEmitter in array4)
		{
			if (particleEmitter != null)
			{
				particleEmitter.emit = false;
			}
		}
	}

	public static bool IsSafe()
	{
		return !m_bShuttingDown;
	}

	protected GameObject CreateEditorGameObject(GameObject srcGameObj)
	{
		return srcGameObj;
	}

	public GameObject CreateGameObject(string name)
	{
		if (!IsSafe())
		{
			return null;
		}
		return CreateEditorGameObject(new GameObject(name));
	}

	public GameObject CreateGameObject(GameObject original)
	{
		if (!IsSafe())
		{
			return null;
		}
		return CreateEditorGameObject(Object.Instantiate(original));
	}

	public GameObject CreateGameObject(GameObject prefabObj, Vector3 position, Quaternion rotation)
	{
		if (!IsSafe())
		{
			return null;
		}
		return CreateEditorGameObject((GameObject)Object.Instantiate(prefabObj, position, rotation));
	}

	public GameObject CreateGameObject(GameObject parentObj, GameObject prefabObj)
	{
		if (!IsSafe())
		{
			return null;
		}
		GameObject gameObject = CreateGameObject(prefabObj);
		if (parentObj != null)
		{
			ChangeParent(parentObj.transform, gameObject.transform, true, null);
		}
		return gameObject;
	}

	public GameObject CreateGameObject(GameObject parentObj, Transform parentTrans, GameObject prefabObj)
	{
		if (!IsSafe())
		{
			return null;
		}
		GameObject gameObject = CreateGameObject(prefabObj);
		if (parentObj != null)
		{
			ChangeParent(parentObj.transform, gameObject.transform, true, parentTrans);
		}
		return gameObject;
	}

	protected void ChangeParent(Transform newParent, Transform child, bool bKeepingLocalTransform, Transform addTransform)
	{
		NcTransformTool ncTransformTool = null;
		if (bKeepingLocalTransform)
		{
			ncTransformTool = new NcTransformTool(child.transform);
			if (addTransform != null)
			{
				ncTransformTool.AddTransform(addTransform);
			}
		}
		child.parent = newParent;
		if (bKeepingLocalTransform)
		{
			ncTransformTool.CopyToLocalTransform(child.transform);
		}
		if (bKeepingLocalTransform)
		{
			NcBillboard[] componentsInChildren = child.GetComponentsInChildren<NcBillboard>();
			NcBillboard[] array = componentsInChildren;
			foreach (NcBillboard ncBillboard in array)
			{
				ncBillboard.UpdateBillboard();
			}
		}
	}

	protected void UpdateMeshColors(Color color)
	{
		if (m_MeshFilter == null)
		{
			m_MeshFilter = (MeshFilter)base.gameObject.GetComponent(typeof(MeshFilter));
		}
		if (!(m_MeshFilter == null) && !(m_MeshFilter.sharedMesh == null) && !(m_MeshFilter.mesh == null))
		{
			Color[] array = new Color[m_MeshFilter.mesh.vertexCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = color;
			}
			m_MeshFilter.mesh.colors = array;
		}
	}

	protected virtual void OnDestroy()
	{
		if (m_RuntimeMaterials == null)
		{
			return;
		}
		foreach (Material runtimeMaterial in m_RuntimeMaterials)
		{
			Object.Destroy(runtimeMaterial);
		}
		m_RuntimeMaterials = null;
	}

	public void OnApplicationQuit()
	{
		m_bShuttingDown = true;
	}

	public virtual void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
	}

	public virtual void OnUpdateToolData()
	{
	}
}
