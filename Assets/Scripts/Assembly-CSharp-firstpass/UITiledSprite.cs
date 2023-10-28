using UnityEngine;

[ExecuteInEditMode]
public class UITiledSprite : UISlicedSprite
{
	public override Type type
	{
		get
		{
			return Type.Tiled;
		}
	}
}
