using UnityEngine;

public class CWCardTutorial : MonoBehaviour
{
	public GameObject attack;

	public GameObject defense;

	public GameObject magicCost;

	public GameObject landscapeType;

	private float startTime;

	private int bannerIndex;

	private GameObject[] banners;

	private bool done;

	private CWTutorialsPopup popup;

	private CWHandCardZoom zoomCard;

	private static float bannerDisplayTime = 1f;

	public void Show(bool show)
	{
		base.gameObject.SetActive(show);
		if (show)
		{
			if ((bool)attack)
			{
				attack.SetActive(false);
			}
			if ((bool)defense)
			{
				defense.SetActive(false);
			}
			if ((bool)magicCost)
			{
				magicCost.SetActive(false);
			}
			if ((bool)landscapeType)
			{
				landscapeType.SetActive(false);
			}
			startTime = Time.realtimeSinceStartup;
			bannerIndex = 0;
			banners = new GameObject[4] { attack, defense, magicCost, landscapeType };
			ShowBanner(bannerIndex);
			ShowZoomCard();
			UICamera.useInputEnabler = true;
		}
		else
		{
			HideZoomCard();
		}
	}

	public void SetPopup(GameObject p)
	{
		if (p != null)
		{
			popup = p.GetComponentInChildren(typeof(CWTutorialsPopup)) as CWTutorialsPopup;
		}
		if (popup != null)
		{
			CWTutorialNext cWTutorialNext = base.gameObject.AddComponent(typeof(CWTutorialNext)) as CWTutorialNext;
			if (cWTutorialNext != null)
			{
				cWTutorialNext.target = popup.gameObject;
				cWTutorialNext.functionName = "OnClick";
				cWTutorialNext.useOnPress = false;
			}
			SLOTUI.RemoveInputEnabler(popup.gameObject);
		}
	}

	private void Update()
	{
		float num = Time.realtimeSinceStartup - startTime;
		if (!done && num >= bannerDisplayTime)
		{
			bannerIndex++;
			if (!ShowBanner(bannerIndex))
			{
				done = true;
			}
			else
			{
				startTime = Time.realtimeSinceStartup;
			}
			if (done)
			{
				SLOTUI.AddInputEnabler(base.gameObject);
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			UIInputEnabler uIInputEnabler = base.gameObject.GetComponent(typeof(UIInputEnabler)) as UIInputEnabler;
			if (!UICamera.useInputEnabler || (uIInputEnabler != null && uIInputEnabler.inputEnabled))
			{
				Clicked();
			}
		}
	}

	private bool ShowBanner(int idx)
	{
		if (idx >= 0 && idx < banners.Length && banners[idx] != null)
		{
			banners[idx].SetActive(true);
			return true;
		}
		return false;
	}

	private void Clicked()
	{
		if (done)
		{
			base.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			Show(false);
		}
	}

	private void ShowZoomCard()
	{
		CWPlayerHandsController instance = CWPlayerHandsController.GetInstance();
		for (int i = 0; i < 7; i++)
		{
			if (instance.playerHands[i] != null && instance.playerHands[i].gameObject.activeInHierarchy)
			{
				CWTBTapToBringForward component = instance.playerHands[i].GetComponent<CWTBTapToBringForward>();
				if (component != null && component.card != null && component.card.Form != null && component.card.Form.Type == CardType.Creature)
				{
					component.Select();
					zoomCard = component.PlayZoomTween(true);
					break;
				}
			}
		}
	}

	private void HideZoomCard()
	{
		if (zoomCard != null)
		{
			zoomCard.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			zoomCard = null;
		}
	}
}
