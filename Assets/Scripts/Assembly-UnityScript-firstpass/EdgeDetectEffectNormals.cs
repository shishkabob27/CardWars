using System;
using UnityEngine;

[Serializable]
public class EdgeDetectEffectNormals : PostEffectsBase
{
	public EdgeDetectMode mode;
	public float sensitivityDepth;
	public float sensitivityNormals;
	public float edgesOnly;
	public Color edgesOnlyBgColor;
	public Shader edgeDetectShader;
}
