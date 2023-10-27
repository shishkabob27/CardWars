using UnityEngine;

public class SLOTUIAnchor : UIAnchor
{
	public Vector2 screenOffset;
	public float depthOffset;
	public bool useDepthOffset;
	public GameObject parentObject;
	public bool useParentObjectPosition;
	public bool useCameraRectIfNotUsingParentObjectPosition;
	public bool anchorEnabled;
	public bool syncWidth;
	public float syncWidthOffset;
	public float syncWidthScale;
	public bool syncHeight;
	public float syncHeightOffset;
	public float syncHeightScale;
	public bool syncClippingWidth;
	public float syncClippingWidthOffset;
	public float syncClippingWidthScale;
	public bool syncClippingHeight;
	public float syncClippingHeightOffset;
	public float syncClippingHeightScale;
	public bool syncColliderWidth;
	public float syncColliderWidthOffset;
	public float syncColliderWidthScale;
	public bool syncColliderHeight;
	public float syncColliderHeightOffset;
	public float syncColliderHeightScale;
}
