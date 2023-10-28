using System.Collections;
using UnityEngine;

public class CWFuseCraftSequencer : MonoBehaviour
{
	public AudioClip craftStartSound;

	public AudioClip craftSound;

	public TweenAlpha blackPanelTween;

	public TweenAlpha fuseAnimPanel;

	public GameObject[] recipeCards;

	public Transform[] recipeCardsStartPos;

	public Transform craftAnimationTr;

	public float recipeFlyTime;

	public AudioClip recipeFlySound;

	public float burstFXStartDelay = 0.5f;

	public GameObject burstFX;

	public float whitePanelStartDelay = 1f;

	public TweenAlpha whitePanelTween;

	public AudioClip whiteFXSound;

	public GameObject cardBefore;

	public GameObject[] vfxRarity;

	public GameObject normalChestFX;

	public GameObject premiumChestFX;

	public GameObject[] bannerObjects;

	public AudioClip[] raritySounds;

	public GameObject resultCard;

	public GameObject resultCardObj;

	public float[] fxTimes;

	public float[] cardTimes;

	public GameObject tapAnywherePanel;

	public GameObject flyingCard;

	public float flyingTime;

	public Transform destTr;

	public AudioClip earnSound;

	public Transform bannerParentTr;

	public Transform effectParentTr;

	private PanelManagerDeck panelMgrDeck;

	private CWUpdatePlayerStats playerStats;

	private SLOTAudioManager audioMgr;

	private static CWFuseCraftSequencer g_craftSqcr;

	private bool _keyPressed;

	private void Awake()
	{
		g_craftSqcr = this;
	}

	public static CWFuseCraftSequencer GetInstance()
	{
		return g_craftSqcr;
	}

	private void Start()
	{
		panelMgrDeck = PanelManagerDeck.GetInstance();
		playerStats = CWUpdatePlayerStats.GetInstance();
		audioMgr = SLOTGameSingleton<SLOTAudioManager>.GetInstance();
		if (playerStats != null)
		{
			destTr.parent = playerStats.inventoryLabel.transform;
			destTr.localPosition = Vector3.zero;
		}
	}

	public void TriggerCraftSequence(CardForm form, RecipeData recipe)
	{
		if (playerStats != null)
		{
			playerStats.holdUpdateFlag = true;
		}
		StartCoroutine(CraftSequencer(form, recipe));
	}

	private IEnumerator CraftSequencer(CardForm form, RecipeData recipe)
	{
		blackPanelTween.gameObject.SetActive(true);
		blackPanelTween.Play(true);
		blackPanelTween.Reset();
		yield return new WaitForSeconds(1f);
		craftAnimationTr.gameObject.SetActive(true);
		for (int i = 0; i < recipe.ingredients.Count; i++)
		{
			GameObject obj = recipeCards[i];
			obj.SetActive(true);
			CardForm recipeCardForm = recipe.ingredients[i].Form;
			panelMgrDeck.FillCardInfo(obj, recipeCardForm);
			obj.transform.position = recipeCardsStartPos[i].position;
			obj.transform.localScale = recipeCardsStartPos[i].localScale;
		}
		fuseAnimPanel.gameObject.SetActive(true);
		fuseAnimPanel.Play(true);
		fuseAnimPanel.Reset();
		yield return new WaitForSeconds(0.5f);
		audioMgr.PlayOneShot(craftSound);
		for (int j = 0; j < recipe.ingredients.Count; j++)
		{
			yield return new WaitForSeconds(0.2f);
			GameObject obj2 = recipeCards[j];
			iTween.MoveTo(obj2, iTween.Hash("position", craftAnimationTr, "time", recipeFlyTime, "name", "cardFly"));
			Vector3 sc = new Vector3(0.1f, 0.1f, 0.1f);
			iTween.ScaleTo(obj2, iTween.Hash("scale", sc, "time", recipeFlyTime, "name", "cardScale"));
			audioMgr.PlayOneShot(recipeFlySound);
			yield return new WaitForSeconds(recipeFlyTime - 0.2f);
			iTween.StopByName(obj2, "cardFly");
			iTween.StopByName(obj2, "cardScale");
			obj2.SetActive(false);
		}
		yield return new WaitForSeconds(burstFXStartDelay);
		audioMgr.PlayOneShot(whiteFXSound);
		if (burstFX != null)
		{
			GetSpawnObject(burstFX, effectParentTr);
		}
		yield return new WaitForSeconds(whitePanelStartDelay);
		whitePanelTween.gameObject.SetActive(true);
		whitePanelTween.Play(true);
		craftAnimationTr.gameObject.SetActive(false);
		yield return new WaitForSeconds(1f);
		cardBefore.SetActive(true);
		whitePanelTween.Play(false);
		yield return new WaitForSeconds(1f);
		iTween.RotateTo(cardBefore, iTween.Hash("y", 1080, "time", 3f));
		yield return StartCoroutine(PlayBanner(form));
	}

	private IEnumerator PlayBanner(CardForm form)
	{
		GameObject vfx = vfxRarity[form.Rarity - 1];
		int tempIndex = 1;
		if (vfx.name.EndsWith("Low"))
		{
			tempIndex = 0;
		}
		else if (vfx.name.EndsWith("Med"))
		{
			tempIndex = 1;
		}
		else if (vfx.name.EndsWith("High"))
		{
			tempIndex = 2;
		}
		float cardTime = cardTimes[tempIndex];
		float fxTime = fxTimes[tempIndex];
		audioMgr.PlayOneShot(raritySounds[form.Rarity - 1]);
		GameObject prefabObj2 = ((form.Rarity < 3) ? normalChestFX : premiumChestFX);
		GetSpawnObject(bannerObjects[form.Rarity - 1], bannerParentTr);
		yield return new WaitForSeconds(fxTime);
		prefabObj2 = vfxRarity[form.Rarity - 1];
		GameObject flashFx = GetSpawnObject(prefabObj2, effectParentTr);
		flashFx.transform.localScale = new Vector3(100f, 100f, 100f);
		Transform[] children = flashFx.GetComponentsInChildren<Transform>(true);
		Transform[] array = children;
		foreach (Transform tr in array)
		{
			tr.gameObject.layer = effectParentTr.gameObject.layer;
		}
		yield return new WaitForSeconds(cardTime);
		resultCardObj = GetSpawnObject(resultCard, bannerParentTr);
		resultCardObj.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
		TweenPosition tw = resultCardObj.GetComponent<TweenPosition>();
		tw.from = new Vector3(0f, -90f, 5f);
		tw.to = new Vector3(0f, -110f, 5f);
		cardBefore.SetActive(false);
		panelMgrDeck.FillCardInfo(resultCardObj, form);
		yield return new WaitForSeconds(2f);
		tapAnywherePanel.SetActive(true);
		yield return StartCoroutine(WaitForKeyPress());
	}

	private IEnumerator WaitForKeyPress()
	{
		while (!_keyPressed)
		{
			if (Input.GetMouseButtonDown(0))
			{
				yield return StartCoroutine(FinishCrafting());
				break;
			}
			yield return 0;
		}
	}

	private IEnumerator FinishCrafting()
	{
		tapAnywherePanel.SetActive(false);
		blackPanelTween.Play(false);
		blackPanelTween.Reset();
		GameObject spawnObj = GetSpawnObject(flyingCard, bannerParentTr);
		iTween.MoveTo(spawnObj, iTween.Hash("position", destTr, "time", flyingTime));
		Vector3 sc = new Vector3(0.1f, 0.1f, 0.1f);
		iTween.ScaleTo(spawnObj, iTween.Hash("scale", sc, "time", flyingTime));
		TweenAlpha resultTween = resultCardObj.GetComponent<TweenAlpha>();
		resultTween.Play(true);
		yield return new WaitForSeconds(flyingTime);
		Object.DestroyImmediate(resultCardObj);
		if (playerStats != null)
		{
			playerStats.holdUpdateFlag = false;
		}
		iTweenEvent tweenEvent = iTweenEvent.GetEvent(destTr.gameObject, "PunchScale");
		if (tweenEvent != null)
		{
			tweenEvent.Play();
		}
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(earnSound);
		bool tutorialTriggered = false;
		if (TutorialMonitor.Instance != null)
		{
			tutorialTriggered = TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.CardCraftingSuccess);
		}
		if (!tutorialTriggered)
		{
			UICamera.useInputEnabler = false;
		}
		CWFuseCraft.isCrafting = false;
	}

	private GameObject GetSpawnObject(GameObject prefab, Transform parentTr)
	{
		GameObject gameObject = null;
		gameObject = SLOTGame.InstantiateFX(prefab, parentTr.position, parentTr.rotation) as GameObject;
		gameObject.transform.parent = parentTr;
		Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			transform.gameObject.layer = parentTr.gameObject.layer;
		}
		return gameObject;
	}

	private void Update()
	{
	}
}
