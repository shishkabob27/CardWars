using UnityEngine;

public class CWTombstoneScript : MonoBehaviour
{
	public GameObject tombStone;

	private GameObject tombStoneObj;

	public float delay = 3f;

	private PanelManagerBattle panelMgrBattle;

	private void Start()
	{
		panelMgrBattle = PanelManagerBattle.GetInstance();
	}

	public void SpawnTombStone()
	{
		//Object @object = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Props/" + tombStone.name);
        GameObject original = Resources.Load("Props/" + tombStone.name, typeof(GameObject)) as GameObject;
        if (original != null)
		{
			tombStoneObj = Instantiate(original, base.transform.position, base.transform.rotation) as GameObject;
			tombStoneObj.transform.parent = base.transform;
			if (tombStone.name == "GoldBag")
			{
				tombStoneObj.transform.localRotation = Quaternion.Euler(0f, -50f, 0f);
			}
		}
		if (tombStone.name.Contains("Loot"))
		{
			Transform parent = tombStoneObj.transform.parent;
			parent.GetComponent<Collider>().enabled = false;
			Collider[] componentsInChildren = tombStoneObj.GetComponentsInChildren<Collider>();
			Collider[] array = componentsInChildren;
			foreach (Collider collider in array)
			{
				collider.enabled = true;
			}
			tombStoneObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
		}
	}

	private void FlyingCard()
	{
        //Object @object = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Props/" + panelMgrBattle.flyingCard.name);
        GameObject original = Resources.Load("Props/" + panelMgrBattle.flyingCard.name, typeof(GameObject)) as GameObject;
        if (original != null)
		{
			GameObject gameObject = Instantiate(original) as GameObject;
			gameObject.transform.parent = panelMgrBattle.flyingCardDestination.transform.parent;
		}
	}
}
