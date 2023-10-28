using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Edge Detection (Geometry)")]
[ExecuteInEditMode]
public class EdgeDetectEffectNormals : PostEffectsBase
{
	public EdgeDetectMode mode;

	public float sensitivityDepth;

	public float sensitivityNormals;

	public float edgesOnly;

	public Color edgesOnlyBgColor;

	public Shader edgeDetectShader;

	private Material edgeDetectMaterial;

	public EdgeDetectEffectNormals()
	{
		mode = EdgeDetectMode.Thin;
		sensitivityDepth = 1f;
		sensitivityNormals = 1f;
		edgesOnlyBgColor = Color.white;
	}

	public virtual void OnDisable()
	{
		if ((bool)edgeDetectMaterial)
		{
			UnityEngine.Object.DestroyImmediate(edgeDetectMaterial);
		}
	}

	public override bool CheckResources()
	{
		CheckSupport(true);
		edgeDetectMaterial = CheckShaderAndCreateMaterial(edgeDetectShader, edgeDetectMaterial);
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	[ImageEffectOpaque]
	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		Vector2 vector = new Vector2(sensitivityDepth, sensitivityNormals);
		source.filterMode = FilterMode.Point;
		edgeDetectMaterial.SetVector("sensitivity", new Vector4(vector.x, vector.y, 1f, vector.y));
		edgeDetectMaterial.SetFloat("_BgFade", edgesOnly);
		Vector4 vector2 = edgesOnlyBgColor;
		edgeDetectMaterial.SetVector("_BgColor", vector2);
		if (mode == EdgeDetectMode.Thin)
		{
			Graphics.Blit(source, destination, edgeDetectMaterial, 0);
		}
		else
		{
			Graphics.Blit(source, destination, edgeDetectMaterial, 1);
		}
	}

	public override void Main()
	{
	}
}
