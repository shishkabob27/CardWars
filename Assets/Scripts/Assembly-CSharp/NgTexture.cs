using System.Collections.Generic;
using UnityEngine;

public class NgTexture
{
	public static void UnloadTextures(GameObject rootObj)
	{
		if (rootObj == null)
		{
			return;
		}
		Renderer[] componentsInChildren = rootObj.GetComponentsInChildren<Renderer>(true);
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (renderer.material != null && renderer.material.mainTexture != null)
			{
				Resources.UnloadAsset(renderer.material.mainTexture);
			}
		}
	}

	public static Texture2D CopyTexture(Texture2D srcTex, Texture2D tarTex)
	{
		Color32[] pixels = srcTex.GetPixels32();
		tarTex.SetPixels32(pixels);
		tarTex.Apply(false);
		return tarTex;
	}

	public static Texture2D InverseTexture32(Texture2D srcTex, Texture2D tarTex)
	{
		Color32[] pixels = srcTex.GetPixels32();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i].a = (byte)(255 - pixels[i].a);
		}
		tarTex.SetPixels32(pixels);
		tarTex.Apply(false);
		return tarTex;
	}

	public static Texture2D CombineTexture(Texture2D baseTexture, Texture2D combineTexture)
	{
		Texture2D texture2D = new Texture2D(baseTexture.width, baseTexture.height, baseTexture.format, false);
		Color[] pixels = baseTexture.GetPixels();
		Color[] pixels2 = combineTexture.GetPixels();
		Color[] array = new Color[pixels.Length];
		int num = pixels.Length;
		for (int i = 0; i < num; i++)
		{
			array[i] = Color.Lerp(pixels[i], pixels2[i], pixels2[i].a);
		}
		texture2D.SetPixels(array);
		texture2D.Apply(false);
		return texture2D;
	}

	public static bool CompareTexture(Texture2D tex1, Texture2D tex2)
	{
		Color[] pixels = tex1.GetPixels();
		Color[] pixels2 = tex2.GetPixels();
		if (pixels.Length != pixels2.Length)
		{
			return false;
		}
		int num = pixels.Length;
		for (int i = 0; i < num; i++)
		{
			if (pixels[i] != pixels2[i])
			{
				return false;
			}
		}
		return true;
	}

	public static Texture2D FindTexture(List<Texture2D> findList, Texture2D findTex)
	{
		for (int i = 0; i < findList.Count; i++)
		{
			if (CompareTexture(findList[i], findTex))
			{
				return findList[i];
			}
		}
		return null;
	}

	public static int FindTextureIndex(List<Texture2D> findList, Texture2D findTex)
	{
		for (int i = 0; i < findList.Count; i++)
		{
			if (CompareTexture(findList[i], findTex))
			{
				return i;
			}
		}
		return -1;
	}

	public static Texture2D CopyTexture(Texture2D srcTex, Rect srcRect, Texture2D tarTex, Rect tarRect)
	{
		Color[] pixels = srcTex.GetPixels((int)srcRect.x, (int)srcRect.y, (int)srcRect.width, (int)srcRect.height);
		tarTex.SetPixels((int)tarRect.x, (int)tarRect.y, (int)tarRect.width, (int)tarRect.height, pixels);
		tarTex.Apply();
		return tarTex;
	}

	public static Texture2D CopyTextureHalf(Texture2D srcTexture, Texture2D tarHalfTexture)
	{
		if (srcTexture.width != tarHalfTexture.width * 2)
		{
		}
		if (srcTexture.height != tarHalfTexture.height * 2)
		{
		}
		Color[] pixels = srcTexture.GetPixels();
		Color[] array = new Color[pixels.Length / 4];
		int width = tarHalfTexture.width;
		int height = tarHalfTexture.height;
		int num = 0;
		int num2 = 2;
		int num3 = num2 * 2;
		for (int i = 0; i < height; i++)
		{
			int num4 = 0;
			while (num4 < width)
			{
				array[num] = Color.Lerp(Color.Lerp(pixels[i * width * num3 + num4 * num2], pixels[i * width * num3 + num4 * num2 + 1], 0.5f), Color.Lerp(pixels[i * width * num3 + width * num2 + num4 * num2], pixels[i * width * num3 + width * num2 + num4 * num2 + 1], 0.5f), 0.5f);
				num4++;
				num++;
			}
		}
		tarHalfTexture.SetPixels(array);
		tarHalfTexture.Apply(false);
		return tarHalfTexture;
	}

	public static Texture2D CopyTextureQuad(Texture2D srcTexture, Texture2D tarQuadTexture)
	{
		if (srcTexture.width != tarQuadTexture.width * 4)
		{
		}
		if (srcTexture.height != tarQuadTexture.height * 4)
		{
		}
		Color[] pixels = srcTexture.GetPixels();
		Color[] array = new Color[pixels.Length / 16];
		int width = tarQuadTexture.width;
		int height = tarQuadTexture.height;
		int num = 0;
		int num2 = 4;
		int num3 = num2 * 4;
		for (int i = 0; i < height; i++)
		{
			int num4 = 0;
			while (num4 < width)
			{
				array[num] = Color.Lerp(Color.Lerp(Color.Lerp(Color.Lerp(pixels[i * width * num3 + num4 * num2], pixels[i * width * num3 + num4 * num2 + 1], 0.5f), Color.Lerp(pixels[i * width * num3 + width * num2 + num4 * num2], pixels[i * width * num3 + width * num2 + num4 * num2 + 1], 0.5f), 0.5f), Color.Lerp(Color.Lerp(pixels[i * width * num3 + num4 * num2 + 2], pixels[i * width * num3 + num4 * num2 + 3], 0.5f), Color.Lerp(pixels[i * width * num3 + width * num2 + num4 * num2 + 2], pixels[i * width * num3 + width * num2 + num4 * num2 + 3], 0.5f), 0.5f), 0.5f), Color.Lerp(Color.Lerp(Color.Lerp(pixels[i * width * num3 + width * num2 * 2 + num4 * num2], pixels[i * width * num3 + width * num2 * 2 + num4 * num2 + 1], 0.5f), Color.Lerp(pixels[i * width * num3 + width * num2 * 3 + num4 * num2], pixels[i * width * num3 + width * num2 * 3 + num4 * num2 + 1], 0.5f), 0.5f), Color.Lerp(Color.Lerp(pixels[i * width * num3 + width * num2 * 2 + num4 * num2 + 2], pixels[i * width * num3 + width * num2 * 2 + num4 * num2 + 3], 0.5f), Color.Lerp(pixels[i * width * num3 + width * num2 * 3 + num4 * num2 + 2], pixels[i * width * num3 + width * num2 * 3 + num4 * num2 + 3], 0.5f), 0.5f), 0.5f), 0.5f);
				num4++;
				num++;
			}
		}
		tarQuadTexture.SetPixels(array);
		tarQuadTexture.Apply(false);
		return tarQuadTexture;
	}

	public static Texture2D CopyTexture(Texture2D srcTex, Texture2D tarTex, Rect drawRect)
	{
		Rect srcRect = new Rect(0f, 0f, srcTex.width, srcTex.height);
		if (drawRect.x < 0f)
		{
			srcRect.x -= drawRect.x;
			srcRect.width += drawRect.x;
			drawRect.width += drawRect.x;
			drawRect.x = 0f;
		}
		if (drawRect.y < 0f)
		{
			srcRect.y -= drawRect.y;
			srcRect.height += drawRect.y;
			drawRect.height += drawRect.y;
			drawRect.y = 0f;
		}
		if ((float)tarTex.width < drawRect.x + drawRect.width)
		{
			srcRect.width -= drawRect.x + drawRect.width - (float)tarTex.width;
			drawRect.width -= drawRect.x + drawRect.width - (float)tarTex.width;
		}
		if ((float)tarTex.height < drawRect.y + drawRect.height)
		{
			srcRect.height -= drawRect.y + drawRect.height - (float)tarTex.height;
			drawRect.height -= drawRect.y + drawRect.height - (float)tarTex.height;
		}
		return CopyTexture(srcTex, srcRect, tarTex, drawRect);
	}
}
