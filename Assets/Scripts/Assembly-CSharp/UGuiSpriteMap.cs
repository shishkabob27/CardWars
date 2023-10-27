using UnityEngine;
using System;

public class UGuiSpriteMap : ScriptableObject
{
	[Serializable]
	public class StringSpriteTable : ScriptableDictionary<string, Sprite>
	{
	}

	public StringSpriteTable spriteMap;
}
