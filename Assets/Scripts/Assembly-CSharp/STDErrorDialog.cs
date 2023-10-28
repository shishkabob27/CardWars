using UnityEngine;

public class STDErrorDialog : MonoBehaviour
{
	public delegate void OnClickedDelegate();

	public GameObject errorText;

	public GameObject showErrorObj;

	public GameObject hideErrorObj;

	private static STDErrorDialog g_stdErrorDialog;

	public static STDErrorDialog GetInstance()
	{
		return g_stdErrorDialog;
	}

	private void Awake()
	{
		if (g_stdErrorDialog == null)
		{
			g_stdErrorDialog = this;
		}
	}

	public void ShowError(string error, OnClickedDelegate callback)
	{
		if (!(showErrorObj == null))
		{
			if (errorText != null)
			{
				SQUtils.SetLabel(errorText, error);
			}
			STDErrorRetry[] componentsInChildren = GetComponentsInChildren<STDErrorRetry>(true);
			STDErrorRetry[] array = componentsInChildren;
			foreach (STDErrorRetry sTDErrorRetry in array)
			{
				sTDErrorRetry.callback = callback;
			}
			UIButtonTween component = showErrorObj.GetComponent<UIButtonTween>();
			NGUITools.SetActive(base.gameObject, true);
			component.Play(true);
		}
	}
}
