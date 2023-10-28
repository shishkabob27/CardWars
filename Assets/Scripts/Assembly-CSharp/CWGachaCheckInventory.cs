using UnityEngine;

public class CWGachaCheckInventory : MonoBehaviour
{
	public UIButtonTween tooManyCards;

	private static CWGachaCheckInventory g_CWGachaCheckInventory;

	private void Awake()
	{
		if (g_CWGachaCheckInventory == null)
		{
			g_CWGachaCheckInventory = this;
		}
	}

	public static CWGachaCheckInventory GetInstance()
	{
		return g_CWGachaCheckInventory;
	}

	public void hasMaxCards()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance.DeckManager.CardCount() >= instance.MaxInventory)
		{
			tooManyCards.Play(true);
		}
	}

	private void OnClick()
	{
	}
}
