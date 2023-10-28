using UnityEngine;

[ExecuteInEditMode]
public class UISlicedSprite : UISprite
{
	public override Type type
	{
		get
		{
			return Type.Sliced;
		}
	}
}
