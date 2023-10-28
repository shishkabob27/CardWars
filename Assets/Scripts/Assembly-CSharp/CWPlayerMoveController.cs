using UnityEngine;

public class CWPlayerMoveController : MonoBehaviour
{
	private static CWPlayerMoveController g_mover;

	private void Awake()
	{
		g_mover = this;
	}

	public static CWPlayerMoveController GetInstance()
	{
		return g_mover;
	}
}
