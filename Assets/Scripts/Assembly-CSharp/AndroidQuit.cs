using UnityEngine;

public class AndroidQuit : MonoBehaviour
{
	private void Confirm()
	{
		base.gameObject.SetActive(false);
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void Dismiss()
	{
		base.gameObject.SetActive(false);
	}
}
