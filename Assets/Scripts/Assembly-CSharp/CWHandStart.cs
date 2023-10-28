using UnityEngine;

public class CWHandStart : MonoBehaviour
{
	private CWPlayerHandsController handController;

	private void Start()
	{
	}

	private void OnEnable()
	{
		if (handController == null)
		{
			handController = CWPlayerHandsController.GetInstance();
		}
		handController.HandStart();
	}

	private void Update()
	{
	}
}
