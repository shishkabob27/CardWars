using UnityEngine;

public class BattleButton : MonoBehaviour
{
	public UILabel magicPointsLabel;

	public UITweener buttonTweener;

	public GameObject onClickTarget;

	private CWP1SetupBranch setupbranch;

	private void Start()
	{
		setupbranch = base.gameObject.GetComponent<CWP1SetupBranch>();
	}

	private void Update()
	{
		UpdateTweener();
	}

	private void UpdateTweener()
	{
		if (buttonTweener != null)
		{
			bool flag = GameState.Instance.HasLegalMove(PlayerType.User);
			if (!flag && !buttonTweener.enabled)
			{
				buttonTweener.enabled = true;
				buttonTweener.Play(true);
			}
			else if (flag)
			{
				buttonTweener.Reset();
				buttonTweener.enabled = false;
			}
		}
	}

	private void OnClick()
	{
		if (CanClick())
		{
			if (onClickTarget != null)
			{
				onClickTarget.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			if (setupbranch != null)
			{
				setupbranch.Execute();
			}
		}
	}

	private bool CanClick()
	{
		CWPlayerHandsController instance = CWPlayerHandsController.GetInstance();
		return instance != null && instance.CanPlay();
	}
}
