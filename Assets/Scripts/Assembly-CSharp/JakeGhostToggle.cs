using UnityEngine;

public class JakeGhostToggle : MonoBehaviour
{
	private const string CHARACTER_ORIGINAL = "Jake";

	private const string CHARACTER_REPLACEMENT = "GhostJake";

	public GameObject normal;

	public GameObject ghost;

	private bool isGhostLast;

	private void Start()
	{
		RefreshToggleState(isGhostLast);
	}

	private void Update()
	{
		if (SessionManager.GetInstance().IsReady())
		{
			bool flag = LeaderManager.Instance.AlreadyOwned("Leader_JakeGhost");
			if (flag != isGhostLast)
			{
				isGhostLast = flag;
				RefreshToggleState(flag);
			}
		}
	}

	private void RefreshToggleState(bool isGhost)
	{
		if (normal != null)
		{
			normal.SetActive(!isGhost);
		}
		if (ghost != null)
		{
			ghost.SetActive(isGhost);
		}
	}
}
