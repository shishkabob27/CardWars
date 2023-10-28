using UnityEngine;

public class LocalizedAsset : MonoBehaviour
{
	public Object localizeTarget;

	public void Awake()
	{
		LocalizeAsset(localizeTarget);
	}

	public void LocalizeAsset()
	{
		LocalizeAsset(localizeTarget);
	}

	public static void LocalizeAsset(Object target)
	{
		if (target == null)
		{
			return;
		}
		if (target.GetType() == typeof(GUITexture))
		{
			GUITexture gUITexture = (GUITexture)target;
			if (gUITexture.texture != null)
			{
				Texture texture = (Texture)Language.GetAsset(gUITexture.texture.name);
				if (texture != null)
				{
					gUITexture.texture = texture;
				}
			}
		}
		else if (target.GetType() == typeof(Material))
		{
			Material material = (Material)target;
			if (material.mainTexture != null)
			{
				Texture texture2 = (Texture)Language.GetAsset(material.mainTexture.name);
				if (texture2 != null)
				{
					material.mainTexture = texture2;
				}
			}
		}
		else
		{
			if (target.GetType() != typeof(MeshRenderer))
			{
				return;
			}
			MeshRenderer meshRenderer = (MeshRenderer)target;
			if (meshRenderer.material.mainTexture != null)
			{
				Texture texture3 = (Texture)Language.GetAsset(meshRenderer.material.mainTexture.name);
				if (texture3 != null)
				{
					meshRenderer.material.mainTexture = texture3;
				}
			}
		}
	}
}
