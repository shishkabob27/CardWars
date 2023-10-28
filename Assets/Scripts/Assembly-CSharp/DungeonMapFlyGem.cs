using UnityEngine;

public class DungeonMapFlyGem : CWQuestMapFlyGem
{
	public GameObject Heart;

	private bool mResumeFlag;

	public override void SetResumeFlag(bool resume)
	{
		mResumeFlag = resume;
	}

	public bool GetResumeFlag()
	{
		return mResumeFlag;
	}

	public override void ComputeStartAndDest()
	{
		CWUpdatePlayerStats instance = CWUpdatePlayerStats.GetInstance();
		if (instance != null)
		{
			Transform transform = ((earningType != EarningType.HEART) ? instance.gemSprite.gameObject.transform : instance.heartSprite.gameObject.transform);
			dest.transform.position = transform.position;
			Transform transform2 = ((!(Heart != null)) ? start : Heart.transform);
			start.transform.position = transform2.position;
		}
	}

	protected override GameObject GetSpawnObj(Transform start)
	{
		GameObject spawnObj = base.GetSpawnObj(start);
		spawnObj.SetActive(true);
		return spawnObj;
	}
}
