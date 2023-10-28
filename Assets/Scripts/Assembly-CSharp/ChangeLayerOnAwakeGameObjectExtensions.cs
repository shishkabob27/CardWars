using UnityEngine;

public static class ChangeLayerOnAwakeGameObjectExtensions
{
	public static void SetLayerRecursively(this GameObject gameObject, string layerName)
	{
		gameObject.SetLayerRecursively(LayerMask.NameToLayer(layerName));
	}

	public static void SetLayerRecursively(this GameObject gameObject, int layer)
	{
		gameObject.layer = layer;
		foreach (Transform item in gameObject.transform)
		{
			item.gameObject.SetLayerRecursively(layer);
		}
	}
}
