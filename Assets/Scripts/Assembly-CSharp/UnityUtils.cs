using UnityEngine;

public static class UnityUtils
{
	private static readonly char[] GO_NAME_SEPARATORS = new char[1] { '/' };

	public static GameObject InstantiatePrefab(GameObject prefab, GameObject parent = null)
	{
		GameObject gameObject = Object.Instantiate(prefab);
		if (parent == null)
		{
			return gameObject;
		}
		Transform component = gameObject.GetComponent<Transform>();
		Transform component2 = prefab.GetComponent<Transform>();
		component.SetParent(parent.GetComponent<Transform>());
		component.localPosition = component2.localPosition;
		component.localScale = component2.localScale;
		component.localRotation = component2.localRotation;
		RectTransform rectTransform = component as RectTransform;
		if (rectTransform != null)
		{
			RectTransform rectTransform2 = component2 as RectTransform;
			rectTransform.offsetMin = rectTransform2.offsetMin;
			rectTransform.offsetMax = rectTransform2.offsetMax;
		}
		return gameObject;
	}

	public static T InstantiatePrefab<T>(T prefabComponent, GameObject parent = null) where T : Component
	{
		GameObject gameObject = InstantiatePrefab(prefabComponent.gameObject, parent);
		return gameObject.GetComponent<T>();
	}

	public static Transform FindChildRecursive(GameObject root, string namePath)
	{
		if (root == null || namePath.Length <= 0)
		{
			return null;
		}
		string[] array = namePath.Split(GO_NAME_SEPARATORS);
		Transform[] componentsInChildren = root.transform.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (!(transform.name != array[0]))
			{
				if (array.Length <= 1)
				{
					return transform;
				}
				Transform transform2 = transform;
				int num = 1;
				while (transform2 != null && num < array.Length)
				{
					transform2 = transform2.Find(array[num]);
					num++;
				}
				if (transform2 != null)
				{
					return transform2;
				}
			}
		}
		return null;
	}

	public static T FindChildRecursive<T>(GameObject root, string namePath) where T : Component
	{
		Transform transform = FindChildRecursive(root, namePath);
		return (!(transform == null)) ? transform.GetComponent<T>() : ((T)null);
	}
}
