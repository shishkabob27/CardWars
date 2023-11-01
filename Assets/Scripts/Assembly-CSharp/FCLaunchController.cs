using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(QuestLaunchHelper))]
public class FCLaunchController : MonoBehaviour
{
	public GameObject RootUGUI;

	public GameObject UpsellPrefab;

	private GameObject FCUpsell;

	public GameObject enterMapEvents;

	public Animation battleMapAnimation;

	protected bool activateAnimationPlaying;

	public string animationIdle = "Idle";

	public string animationActivate = "FionaCakeIcon_Activate";

	public string loadingScreenTextureName = string.Empty;

	private void Start()
	{
		if (null == RootUGUI)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("UGUIMenu");
			if (null != gameObject)
			{
				RootUGUI = gameObject;
			}
		}
	}

	public void OnClick()
	{
		UICamera.useInputEnabler = true;
		activateAnimationPlaying = true;
		battleMapAnimation.CrossFade(animationActivate);
	}

	private void Update()
	{
		if (activateAnimationPlaying && battleMapAnimation != null && !battleMapAnimation.isPlaying)
		{
			UICamera.useInputEnabler = false;
			activateAnimationPlaying = false;
			StartCoroutine(LaunchFionnaAndCakeMapScene());
			battleMapAnimation.Play(animationIdle);
		}
	}

	public void LaunchFCDemo()
	{
		QuestData questData = QuestManager.Instance.GetQuestsByType("fc_demo").FirstOrDefault();
		if (questData != null)
		{
			PlayerInfoScript.GetInstance().SetHasStartedFCDemo();
			Singleton<AnalyticsManager>.Instance.LogFCDemoStart();
			GlobalFlags.Instance.InMPMode = false;
			if (!string.IsNullOrEmpty(loadingScreenTextureName))
			{
				questData.LoadingScreenTextureName = loadingScreenTextureName;
			}
			GetComponent<QuestLaunchHelper>().LaunchQuest(questData.QuestID, PlayerInfoScript.GetInstance().GetSelectedDeckCopy(), null, new FCDemoBattleResolver());
		}
	}

	private IEnumerator LaunchFionnaAndCakeMapScene()
	{
		TFUtils.DebugLog("Launching Fionna & Cake scene", GetType().ToString());
		FCMapController.Activate(false);
		if (enterMapEvents != null)
		{
			enterMapEvents.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		yield return null;
	}

	private void DisplayPurchaseScreen()
	{
		TFUtils.DebugLog("Displaying the purchase screen", GetType().ToString());
		if (null != FCUpsell)
		{
			Object.Destroy(FCUpsell);
		}
		FCUpsell = UnityUtils.InstantiatePrefab(UpsellPrefab, RootUGUI);
		if (null != FCUpsell)
		{
			FCUpsellController component = FCUpsell.GetComponent<FCUpsellController>();
			if (null != component)
			{
				component.SetFCButton(base.gameObject);
				component.ShowPanel();
			}
		}
	}

	private void OnDestroy()
	{
		if (null != FCUpsell)
		{
			Object.Destroy(FCUpsell);
		}
	}
}
