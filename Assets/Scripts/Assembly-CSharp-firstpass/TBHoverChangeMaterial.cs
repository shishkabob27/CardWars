using UnityEngine;

public class TBHoverChangeMaterial : MonoBehaviour
{
	public Material hoverMaterial;

	private Material normalMaterial;

	private void Start()
	{
		normalMaterial = GetComponent<Renderer>().sharedMaterial;
	}

	private void OnFingerHover(FingerHoverEvent e)
	{
		if (e.Phase == FingerHoverPhase.Enter)
		{
			GetComponent<Renderer>().sharedMaterial = hoverMaterial;
		}
		else
		{
			GetComponent<Renderer>().sharedMaterial = normalMaterial;
		}
	}
}
