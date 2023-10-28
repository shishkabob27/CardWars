using System.Collections;
using System.Collections.Generic;

public class ULSpriteAnimModel : ULSpriteAnimModelInterface
{
	protected Hashtable animationHashtable;

	public ULSpriteAnimModel(ULSpriteAnimationSetting[] animationSettings)
	{
		animationHashtable = new Hashtable();
		foreach (ULSpriteAnimationSetting uLSpriteAnimationSetting in animationSettings)
		{
			animationHashtable.Add(uLSpriteAnimationSetting.animName, uLSpriteAnimationSetting);
		}
	}

	public ULSpriteAnimModel(Hashtable hashtable)
	{
		animationHashtable = hashtable;
	}

	public ULSpriteAnimModel()
	{
		animationHashtable = new Hashtable();
	}

	public void AddAnimationSetting(string key, ULSpriteAnimationSetting setting)
	{
		animationHashtable.Add(key, setting);
	}

	public string GetMaterialName(string animName)
	{
		ULSpriteAnimationSetting uLSpriteAnimationSetting = (ULSpriteAnimationSetting)animationHashtable[animName];
		if (uLSpriteAnimationSetting.texture != null)
		{
			return "Materials/" + uLSpriteAnimationSetting.resourceName;
		}
		return uLSpriteAnimationSetting.resourceName;
	}

	public string GetResourceName(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).resourceName;
	}

	public string GetTextureName(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).texture;
	}

	public bool HasAnimation(string animName)
	{
		if (animationHashtable.ContainsKey(animName))
		{
			return true;
		}
		return false;
	}

	public float CellTop(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).cellTop;
	}

	public float CellLeft(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).cellLeft;
	}

	public float CellWidth(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).cellWidth;
	}

	public float CellHeight(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).cellHeight;
	}

	public int CellStartColumn(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).cellStartColumn;
	}

	public int CellColumns(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).cellColumns;
	}

	public int CellCount(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).cellCount;
	}

	public int FramesPerSecond(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).framesPerSecond;
	}

	public float TimingTotal(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).timingTotal;
	}

	public List<float> TimingList(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).timingList;
	}

	public bool Loop(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).loopMode == ULSpriteAnimationSetting.LoopMode.Loop;
	}

	public bool FlipH(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).flipH;
	}

	public bool FlipV(string animName)
	{
		return ((ULSpriteAnimationSetting)animationHashtable[animName]).flipV;
	}
}
