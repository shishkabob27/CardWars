using UnityEngine;

public class VersionNumber : MonoBehaviour
{
	private UILabel mVersion;

	private void Start()
	{
		mVersion = base.gameObject.GetComponent<UILabel>();
		if ((bool)mVersion)
		{
			//mVersion.text = "Version: " + KFFCSUtils.GetManifestKeyString("CFBundleVersion");
			mVersion.text = "Version: " + Application.version;
        }
	}
}
