using UnityEngine;

public class STDErrorRetry : MonoBehaviour
{
	public STDErrorDialog.OnClickedDelegate callback;

	public void OnClick()
	{
		if (callback != null)
		{
			callback();
		}
	}
}
