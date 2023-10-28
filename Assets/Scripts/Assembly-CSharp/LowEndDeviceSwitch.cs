using UnityEngine;

public class LowEndDeviceSwitch : MonoBehaviour
{
	private void Update()
	{
		if (SLOTGame.IsLowEndDevice())
		{
			if (base.gameObject.GetComponent<Renderer>().enabled && CWBattleSequenceController.GetInstance().camAlignFlag)
			{
				base.gameObject.GetComponent<Renderer>().enabled = false;
			}
			else if (!base.gameObject.GetComponent<Renderer>().enabled && !CWBattleSequenceController.GetInstance().camAlignFlag)
			{
				base.gameObject.GetComponent<Renderer>().enabled = true;
			}
		}
	}
}
