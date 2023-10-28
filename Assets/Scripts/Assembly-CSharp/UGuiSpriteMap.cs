using System;
using System.Collections.Generic;
using UnityEngine;

public class UGuiSpriteMap : ScriptableObject
{
	[Serializable]
	public class StringSpriteTable : ScriptableDictionary<string, Sprite>
	{
	}

	public StringSpriteTable spriteMap = new StringSpriteTable();

	public Sprite GetSprite(string spriteName)
	{
		try
		{
			return spriteMap.Dic[spriteName];
		}
		catch (KeyNotFoundException)
		{
		}
		return null;
	}
}
