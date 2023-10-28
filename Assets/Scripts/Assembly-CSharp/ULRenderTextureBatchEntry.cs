using System;
using UnityEngine;

[Serializable]
public class ULRenderTextureBatchEntry
{
	public GameObject subject;

	public ULRenderTexture target;

	public ULRenderTextureCameraRig.RelativeCamDelegate camDelegate;
}
