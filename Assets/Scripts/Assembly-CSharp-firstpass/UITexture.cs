using UnityEngine;

public class UITexture : UIWidget
{
	[SerializeField]
	private Rect mRect;
	[SerializeField]
	private Shader mShader;
	[SerializeField]
	private Texture mTexture;
	[SerializeField]
	private Material mMat;
}
