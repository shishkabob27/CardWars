using System;
using System.Collections.Generic;

[Serializable]
public class BMGlyph
{
	public int index;

	public int x;

	public int y;

	public int width;

	public int height;

	public int offsetX;

	public int offsetY;

	public int advance;

	public int channel;

	public List<int> kerning;

	public int GetKerning(int previousChar)
	{
		if (kerning != null)
		{
			int i = 0;
			for (int count = kerning.Count; i < count; i += 2)
			{
				if (kerning[i] == previousChar)
				{
					return kerning[i + 1];
				}
			}
		}
		return 0;
	}

	public void SetKerning(int previousChar, int amount)
	{
		if (kerning == null)
		{
			kerning = new List<int>();
		}
		for (int i = 0; i < kerning.Count; i += 2)
		{
			if (kerning[i] == previousChar)
			{
				kerning[i + 1] = amount;
				return;
			}
		}
		kerning.Add(previousChar);
		kerning.Add(amount);
	}

	public void Trim(int xMin, int yMin, int xMax, int yMax)
	{
		int num = x + width;
		int num2 = y + height;
		if (x < xMin)
		{
			int num3 = xMin - x;
			x += num3;
			width -= num3;
			offsetX += num3;
		}
		if (y < yMin)
		{
			int num4 = yMin - y;
			y += num4;
			height -= num4;
			offsetY += num4;
		}
		if (num > xMax)
		{
			width -= num - xMax;
		}
		if (num2 > yMax)
		{
			height -= num2 - yMax;
		}
	}
}
