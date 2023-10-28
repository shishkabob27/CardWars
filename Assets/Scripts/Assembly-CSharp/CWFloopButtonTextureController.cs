using UnityEngine;

public class CWFloopButtonTextureController : MonoBehaviour
{
	public Texture greenTexture;

	public Texture redTexture;

	public GameObject FloopObj;

	public void SetButtonColor(FloopButtonColor color)
	{
		switch (color)
		{
		case FloopButtonColor.Green:
			FloopObj.GetComponent<Renderer>().material.mainTexture = greenTexture;
			break;
		case FloopButtonColor.Red:
			FloopObj.GetComponent<Renderer>().material.mainTexture = redTexture;
			break;
		}
	}
}
