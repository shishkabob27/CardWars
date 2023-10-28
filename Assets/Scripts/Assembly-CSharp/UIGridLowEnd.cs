using UnityEngine;

public class UIGridLowEnd : MonoBehaviour
{
	private void OnEnable()
	{
	}

	private void ReduceMaxColumns()
	{
		UIGrid component = base.gameObject.GetComponent<UIGrid>();
		if ((bool)component && component.maxPerLine == 2)
		{
			component.maxPerLine = 1;
			return;
		}
		UIFastGrid component2 = base.gameObject.GetComponent<UIFastGrid>();
		if ((bool)component2 && component2.maxPerLine == 2)
		{
			component2.maxPerLine = 1;
		}
	}
}
