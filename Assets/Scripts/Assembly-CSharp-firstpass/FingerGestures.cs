using UnityEngine;

public class FingerGestures : MonoBehaviour
{
	public bool makePersistent;
	public bool detectUnityRemote;
	public FGInputProvider mouseInputProviderPrefab;
	public FGInputProvider touchInputProviderPrefab;
	public bool adjustPixelScaleForRetinaDisplay;
	public float retinaPixelScale;
}
