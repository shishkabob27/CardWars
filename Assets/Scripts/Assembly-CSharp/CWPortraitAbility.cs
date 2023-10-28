using UnityEngine;

public class CWPortraitAbility : MonoBehaviour
{
	public int playerType;

	public GameObject abilityDisplayPanel;

	public GameObject confirmationPanel;

	public UISprite portrait;

	public BoxCollider col;

	private UIButtonTween buttonTween;

	private bool colActive;

	private void Start()
	{
		buttonTween = GetComponent<UIButtonTween>();
		if (buttonTween != null)
		{
			buttonTween.enabled = false;
		}
	}

	private void OnClick()
	{
		CWPlayerHandsController instance = CWPlayerHandsController.GetInstance();
		if (!(instance != null) || !instance.CanPlay())
		{
			return;
		}
		LeaderItem leader = GameState.Instance.GetLeader(playerType);
		int leaderCooldown = GameState.Instance.GetLeaderCooldown(playerType);
		UILabel[] componentsInChildren = abilityDisplayPanel.GetComponentsInChildren<UILabel>(true);
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			switch (uILabel.name)
			{
			case "Name_Label":
				uILabel.text = leader.Form.Name;
				break;
			case "Desc_Label":
				uILabel.text = leader.Description;
				break;
			case "Turn_Count_Label":
				uILabel.text = leaderCooldown.ToString();
				break;
			}
		}
		UISprite[] componentsInChildren2 = abilityDisplayPanel.GetComponentsInChildren<UISprite>(true);
		UISprite[] array2 = componentsInChildren2;
		foreach (UISprite uISprite in array2)
		{
			if (uISprite.name == "LeaderIcon")
			{
				uISprite.atlas = portrait.atlas;
				uISprite.spriteName = portrait.spriteName;
			}
		}
		if (playerType == (int)PlayerType.User && leader.CanPlay(PlayerType.User) && confirmationPanel != null)
		{
			TweenPosition component = confirmationPanel.GetComponent<TweenPosition>();
			confirmationPanel.SetActive(true);
			component.Play(true);
		}
		if (buttonTween != null)
		{
			buttonTween.Play(true);
		}
	}

	private void Update()
	{
	}
}
