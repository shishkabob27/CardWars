using UnityEngine;

public class DweebSlideInScript : MonoBehaviour
{
	private BattlePhaseManager phaseMgr;

	private void Start()
	{
		phaseMgr = BattlePhaseManager.GetInstance();
		base.transform.localPosition = new Vector3(1110f, base.transform.localPosition.y, base.transform.localPosition.z);
	}

	private void Update()
	{
		if (phaseMgr.Phase == BattlePhase.P1Setup)
		{
			base.transform.localPosition = new Vector3(Mathf.Lerp(base.transform.localPosition.x, 600f, Time.deltaTime * 10f), base.transform.localPosition.y, base.transform.localPosition.z);
			if (base.transform.localPosition.x < 601f)
			{
				base.transform.localPosition = new Vector3(600f, base.transform.localPosition.y, base.transform.localPosition.z);
				Object.Destroy(this);
			}
		}
	}
}
