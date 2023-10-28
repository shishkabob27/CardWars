using UnityEngine;

public class CWResultSwitchboard : MonoBehaviour
{
	public CWResultFillTable CardGrid;

	public bool Skip;

	private void OnEnable()
	{
	}

	private void Update()
	{
	}

	private void OnClick()
	{
		if (CardGrid != null)
		{
			CardGrid.ForceShowAllDrops();
		}
	}
}
