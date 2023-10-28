using UnityEngine;

public class ULRenderTexture
{
	private RenderTexture texture;

	private Material material;

	public Material RMaterial
	{
		get
		{
			return material;
		}
	}

	public RenderTexture RTexture
	{
		get
		{
			return texture;
		}
	}

	public ULRenderTexture(int squareSize, string materialName, string shaderIdentifier)
	{
		texture = new RenderTexture(squareSize, squareSize, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.anisoLevel = 0;
		texture.filterMode = FilterMode.Bilinear;
		Shader shader = Shader.Find(shaderIdentifier);
		material = new Material(shader);
		material.name = materialName;
		material.mainTexture = texture;
	}
}
