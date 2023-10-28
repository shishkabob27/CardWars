using UnityEngine;

public class CWScreenRaycaster : ScreenRaycaster
{
	protected override bool Raycast(Camera cam, Vector2 screenPos, out RaycastHit hit)
	{
		if (cam == null || cam.gameObject == null)
		{
			hit = default(RaycastHit);
			return false;
		}
		LayerMask ignoreLayerMask = IgnoreLayerMask;
		IgnoreLayerMask = ~(1 << cam.gameObject.layer);
		bool result = base.Raycast(cam, screenPos, out hit);
		IgnoreLayerMask = ignoreLayerMask;
		return result;
	}
}
