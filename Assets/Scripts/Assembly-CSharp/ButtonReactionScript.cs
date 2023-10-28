using UnityEngine;

public class ButtonReactionScript : MonoBehaviour
{
	public UISprite sprite;

	public MeshRenderer mesh;

	public bool doNotScale;

	private Color originalColor;

	private Vector3 originalSize;

	private Vector3 reactionSize;

	private float scaleTime = 0.05f;

	private UITweener.Method tweenMethod;

	private void Start()
	{
		originalSize = base.transform.localScale;
		reactionSize = new Vector3(originalSize.x * 0.8f, originalSize.y * 0.8f, originalSize.z);
		if (sprite == null)
		{
			UISprite[] componentsInChildren = GetComponentsInChildren<UISprite>();
			if (componentsInChildren.Length != 0)
			{
				sprite = componentsInChildren[0];
			}
			originalColor = Color.white;
		}
		else
		{
			originalColor = sprite.color;
		}
		if (mesh != null && mesh.material.HasProperty("_Color"))
		{
			originalColor = mesh.material.color;
		}
	}

	private void OnPress(bool isDown)
	{
		if (!base.enabled)
		{
			return;
		}
		if (isDown)
		{
			Color color = new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f, 1f);
			ColorSpritesAndMats(color);
			if (!doNotScale)
			{
				if (mesh == null)
				{
					TweenScale tweenScale = TweenScale.Begin(base.transform.gameObject, scaleTime, reactionSize);
					tweenScale.method = tweenMethod;
				}
				else if (sprite != null)
				{
					TweenScale tweenScale2 = TweenScale.Begin(sprite.gameObject, scaleTime, reactionSize);
					tweenScale2.method = tweenMethod;
				}
			}
		}
		else
		{
			TweenScale tweenScale3 = TweenScale.Begin(base.transform.gameObject, scaleTime, originalSize);
			tweenScale3.method = tweenMethod;
			ColorSpritesAndMats(originalColor);
		}
	}

	private void ColorSpritesAndMats(Color color)
	{
		if (sprite != null)
		{
			sprite.color = color;
		}
		MeshRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.material.color = color;
		}
	}
}
