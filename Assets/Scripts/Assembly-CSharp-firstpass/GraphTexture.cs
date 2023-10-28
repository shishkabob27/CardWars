using System.Collections.Generic;
using UnityEngine;

public sealed class GraphTexture
{
	private Texture2D blankTexture;

	public Rect limits = new Rect(0f, 0f, 100f, 100f);

	public Vector2 offset = Vector2.zero;

	private Vector2 plotScale = Vector2.one;

	private Color penColor = Color.green;

	private int i;

	private bool offScale;

	public Texture2D texture { get; private set; }

	public GraphTexture(Vector2 size, Color bgColor, Color graphColor)
	{
		blankTexture = new Texture2D((int)size.x, (int)size.y, TextureFormat.RGBA32, false);
		texture = new Texture2D((int)size.x, (int)size.y, TextureFormat.RGBA32, false);
		Color[] array = new Color[(int)(size.x * size.y)];
		for (i = 0; i < array.Length; i++)
		{
			array[i] = bgColor;
		}
		penColor = graphColor;
		blankTexture.SetPixels(array);
		blankTexture.Apply();
		texture.SetPixels(array);
		texture.Apply();
	}

	public void Draw(List<float> data)
	{
		texture.SetPixels(blankTexture.GetPixels());
		plotScale = new Vector2((float)texture.width / limits.width, (float)texture.height / limits.height);
		for (i = 0; i < data.Count; i++)
		{
			PlotPoint(new Vector2(i, data[i]), texture);
		}
		texture.Apply();
	}

	private void PlotPoint(Vector2 point, Texture2D tex)
	{
		offScale = false;
		point += offset;
		point.x *= plotScale.x;
		point.y *= plotScale.y;
		if (!(point.x > (float)tex.width) && !(point.x < 0f))
		{
			if (point.y > (float)tex.height)
			{
				offScale = true;
				point.y = tex.height - 1;
			}
			else if (point.y < 0f)
			{
				return;
			}
			tex.SetPixel(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y), (!offScale) ? penColor : Color.red);
		}
	}

	private void Circle(Texture2D tex, int cx, int cy, int r, Color col)
	{
		int num = 0;
		int num2 = r;
		int num3 = -r;
		float num4 = Mathf.Ceil((float)r / Mathf.Sqrt(2f));
		for (num = 0; (float)num <= num4; num++)
		{
			tex.SetPixel(cx + num, cy + num2, col);
			tex.SetPixel(cx + num, cy - num2, col);
			tex.SetPixel(cx - num, cy + num2, col);
			tex.SetPixel(cx - num, cy - num2, col);
			tex.SetPixel(cx + num2, cy + num, col);
			tex.SetPixel(cx - num2, cy + num, col);
			tex.SetPixel(cx + num2, cy - num, col);
			tex.SetPixel(cx - num2, cy - num, col);
			num3 += 2 * num + 1;
			if (num3 > 0)
			{
				num3 += 2 - 2 * num2--;
			}
		}
	}
}
