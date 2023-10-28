using System;
using System.Collections;
using UnityEngine;

public class NgObject
{
	public static void SetActive(GameObject target, bool bActive)
	{
		target.SetActive(bActive);
	}

	public static void SetActiveRecursively(GameObject target, bool bActive)
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

	public static bool IsActive(GameObject target)
	{
		return target.activeInHierarchy && target.activeSelf;
	}

	public static GameObject CreateGameObject(GameObject prefabObj)
	{
		return (GameObject)NcSafeTool.SafeInstantiate(prefabObj);
	}

	public static GameObject CreateGameObject(GameObject parent, string name)
	{
		return CreateGameObject(parent.transform, name);
	}

	public static GameObject CreateGameObject(MonoBehaviour parent, string name)
	{
		return CreateGameObject(parent.transform, name);
	}

	public static GameObject CreateGameObject(Transform parent, string name)
	{
		GameObject gameObject = new GameObject(name);
		if (parent != null)
		{
			NcTransformTool ncTransformTool = new NcTransformTool(gameObject.transform);
			gameObject.transform.parent = parent;
			ncTransformTool.CopyToLocalTransform(gameObject.transform);
		}
		return gameObject;
	}

	public static GameObject CreateGameObject(GameObject parent, GameObject prefabObj)
	{
		return CreateGameObject(parent.transform, prefabObj);
	}

	public static GameObject CreateGameObject(MonoBehaviour parent, GameObject prefabObj)
	{
		return CreateGameObject(parent.transform, prefabObj);
	}

	public static GameObject CreateGameObject(Transform parent, GameObject prefabObj)
	{
		GameObject gameObject = (GameObject)NcSafeTool.SafeInstantiate(prefabObj);
		if (parent != null)
		{
			NcTransformTool ncTransformTool = new NcTransformTool(gameObject.transform);
			gameObject.transform.parent = parent;
			ncTransformTool.CopyToLocalTransform(gameObject.transform);
		}
		return gameObject;
	}

	public static GameObject CreateGameObject(GameObject parent, GameObject prefabObj, Vector3 pos, Quaternion rot)
	{
		return CreateGameObject(parent.transform, prefabObj, pos, rot);
	}

	public static GameObject CreateGameObject(MonoBehaviour parent, GameObject prefabObj, Vector3 pos, Quaternion rot)
	{
		return CreateGameObject(parent.transform, prefabObj, pos, rot);
	}

	public static GameObject CreateGameObject(Transform parent, GameObject prefabObj, Vector3 pos, Quaternion rot)
	{
		if (!NcSafeTool.IsSafe())
		{
			return null;
		}
		GameObject gameObject = (GameObject)NcSafeTool.SafeInstantiate(prefabObj, pos, rot);
		if (parent != null)
		{
			NcTransformTool ncTransformTool = new NcTransformTool(gameObject.transform);
			gameObject.transform.parent = parent;
			ncTransformTool.CopyToLocalTransform(gameObject.transform);
		}
		return gameObject;
	}

	public static void HideAllChildObject(GameObject parent)
	{
		int num = parent.transform.GetChildCount() - 1;
		while (0 <= num)
		{
			if (num < parent.transform.GetChildCount())
			{
				IsActive(parent.transform.GetChild(num).gameObject);
			}
			num--;
		}
	}

	public static void RemoveAllChildObject(GameObject parent, bool bImmediate)
	{
		int num = parent.transform.GetChildCount() - 1;
		while (0 <= num)
		{
			if (num < parent.transform.GetChildCount())
			{
				Transform child = parent.transform.GetChild(num);
				if (bImmediate)
				{
					UnityEngine.Object.DestroyImmediate(child.gameObject);
				}
				else
				{
					UnityEngine.Object.Destroy(child.gameObject);
				}
			}
			num--;
		}
	}

	public static Component CreateComponent(Transform parent, Type type)
	{
		return CreateComponent(parent.gameObject, type);
	}

	public static Component CreateComponent(MonoBehaviour parent, Type type)
	{
		return CreateComponent(parent.gameObject, type);
	}

	public static Component CreateComponent(GameObject parent, Type type)
	{
		Component component = parent.GetComponent(type);
		if (component != null)
		{
			return component;
		}
		return parent.AddComponent(type);
	}

	public static Transform FindTransform(Transform rootTrans, string name)
	{
		Transform transform = rootTrans.Find(name);
		if ((bool)transform)
		{
			return transform;
		}
		foreach (Transform rootTran in rootTrans)
		{
			transform = FindTransform(rootTran, name);
			if ((bool)transform)
			{
				return transform;
			}
		}
		return null;
	}

	public static bool FindTransform(Transform rootTrans, Transform findTrans)
	{
		if (rootTrans == findTrans)
		{
			return true;
		}
		foreach (Transform rootTran in rootTrans)
		{
			if (FindTransform(rootTran, findTrans))
			{
				return true;
			}
		}
		return false;
	}

	public static Material ChangeMeshMaterial(Transform t, Material newMat)
	{
		MeshRenderer[] componentsInChildren = t.GetComponentsInChildren<MeshRenderer>(true);
		Material result = null;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			result = componentsInChildren[i].material;
			componentsInChildren[i].material = newMat;
		}
		return result;
	}

	public static void ChangeSkinnedMeshColor(Transform t, Color color)
	{
		SkinnedMeshRenderer[] componentsInChildren = t.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].material.color = color;
		}
	}

	public static void ChangeMeshColor(Transform t, Color color)
	{
		MeshRenderer[] componentsInChildren = t.GetComponentsInChildren<MeshRenderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].material.color = color;
		}
	}

	public static void ChangeSkinnedMeshAlpha(Transform t, float alpha)
	{
		SkinnedMeshRenderer[] componentsInChildren = t.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Color color = componentsInChildren[i].material.color;
			color.a = alpha;
			componentsInChildren[i].material.color = color;
		}
	}

	public static void ChangeMeshAlpha(Transform t, float alpha)
	{
		MeshRenderer[] componentsInChildren = t.GetComponentsInChildren<MeshRenderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Color color = componentsInChildren[i].material.color;
			color.a = alpha;
			componentsInChildren[i].material.color = color;
		}
	}

	public static Transform[] GetChilds(Transform parentObj)
	{
		Transform[] componentsInChildren = parentObj.GetComponentsInChildren<Transform>(true);
		Transform[] array = new Transform[componentsInChildren.Length - 1];
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			array[i - 1] = componentsInChildren[i];
		}
		return array;
	}

	public static SortedList GetChildsSortList(Transform parentObj, bool bSub, bool bOnlyActive)
	{
		SortedList sortedList = new SortedList();
		if (bSub)
		{
			Transform[] componentsInChildren = parentObj.GetComponentsInChildren<Transform>(bOnlyActive);
			for (int i = 1; i < componentsInChildren.Length; i++)
			{
				sortedList.Add(componentsInChildren[i].name, componentsInChildren[i]);
			}
		}
		else
		{
			for (int j = 0; j < parentObj.childCount; j++)
			{
				Transform child = parentObj.GetChild(j);
				sortedList.Add(child.name, child);
			}
		}
		return sortedList;
	}

	public static GameObject FindObjectWithTag(GameObject rootObj, string findTag)
	{
		if (rootObj == null)
		{
			return null;
		}
		if (rootObj.tag == findTag)
		{
			return rootObj;
		}
		for (int i = 0; i < rootObj.transform.GetChildCount(); i++)
		{
			GameObject gameObject = FindObjectWithTag(rootObj.transform.GetChild(i).gameObject, findTag);
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		return null;
	}

	public static GameObject FindObjectWithLayer(GameObject rootObj, int nFindLayer)
	{
		if (rootObj == null)
		{
			return null;
		}
		if (rootObj.layer == nFindLayer)
		{
			return rootObj;
		}
		for (int i = 0; i < rootObj.transform.GetChildCount(); i++)
		{
			GameObject gameObject = FindObjectWithLayer(rootObj.transform.GetChild(i).gameObject, nFindLayer);
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		return null;
	}

	public static void ChangeLayerWithChild(GameObject rootObj, int nLayer)
	{
		if (!(rootObj == null))
		{
			rootObj.layer = nLayer;
			for (int i = 0; i < rootObj.transform.GetChildCount(); i++)
			{
				ChangeLayerWithChild(rootObj.transform.GetChild(i).gameObject, nLayer);
			}
		}
	}

	public static void GetMeshInfo(GameObject selObj, bool bInChildren, out int nVertices, out int nTriangles, out int nMeshCount)
	{
		nVertices = 0;
		nTriangles = 0;
		nMeshCount = 0;
		if (selObj == null)
		{
			return;
		}
		Component[] array;
		Component[] array2;
		if (bInChildren)
		{
			array = selObj.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
			array2 = selObj.GetComponentsInChildren(typeof(MeshFilter));
		}
		else
		{
			array = selObj.GetComponents(typeof(SkinnedMeshRenderer));
			array2 = selObj.GetComponents(typeof(MeshFilter));
		}
		ArrayList arrayList = new ArrayList(array2.Length + array.Length);
		for (int i = 0; i < array2.Length; i++)
		{
			MeshFilter meshFilter = (MeshFilter)array2[i];
			arrayList.Add(meshFilter.sharedMesh);
		}
		for (int j = 0; j < array.Length; j++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)array[j];
			arrayList.Add(skinnedMeshRenderer.sharedMesh);
		}
		for (int k = 0; k < arrayList.Count; k++)
		{
			Mesh mesh = (Mesh)arrayList[k];
			if (mesh != null)
			{
				nVertices += mesh.vertexCount;
				nTriangles += mesh.triangles.Length / 3;
				nMeshCount++;
			}
		}
	}

	public static void GetMeshInfo(Mesh mesh, out int nVertices, out int nTriangles, out int nMeshCount)
	{
		nVertices = 0;
		nTriangles = 0;
		nMeshCount = 0;
		if (!(mesh == null) && mesh != null)
		{
			nVertices += mesh.vertexCount;
			nTriangles += mesh.triangles.Length / 3;
			nMeshCount++;
		}
	}
}
