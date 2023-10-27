using UnityEngine;

public class FGMouseInputProvider : FGInputProvider
{
	public int maxButtons;
	public string pinchAxis;
	public float pinchAxisScale;
	public float pinchResetTimeDelay;
	public float initialPinchDistance;
	public string twistAxis;
	public float twistAxisScale;
	public KeyCode twistKey;
	public float twistResetTimeDelay;
	public KeyCode twistAndPinchKey;
}
