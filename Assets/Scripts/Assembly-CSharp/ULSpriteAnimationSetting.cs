using System;
using System.Collections.Generic;

[Serializable]
public class ULSpriteAnimationSetting
{
	public enum LoopMode
	{
		None,
		Loop
	}

	public string animName;

	public string resourceName;

	public string texture;

	public float cellTop;

	public float cellLeft;

	public float cellWidth;

	public float cellHeight;

	public int cellStartColumn;

	public int cellColumns;

	public int cellCount;

	public int framesPerSecond;

	public float timingTotal;

	public List<float> timingList;

	public LoopMode loopMode;

	public bool flipH;

	public bool flipV;
}
