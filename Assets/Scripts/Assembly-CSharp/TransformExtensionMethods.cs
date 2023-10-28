using UnityEngine;

public static class TransformExtensionMethods
{
	public static Transform FindDescendant(this Transform transform, string descendantName)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child != null && child.name == descendantName)
			{
				return child;
			}
		}
		for (int j = 0; j < transform.childCount; j++)
		{
			Transform child2 = transform.GetChild(j);
			if (child2 != null)
			{
				Transform transform2 = child2.FindDescendant(descendantName);
				if (transform2 != null)
				{
					return transform2;
				}
			}
		}
		return null;
	}
}
