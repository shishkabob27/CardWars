public class SLOTGame : BusyIconController
{
	public enum AssetBundleType
	{
		DownloadFromServer = 0,
		Local = 1,
		Disabled = 2,
	}

	public AssetBundleType assetBundleType;
	public string errorPopupPrefab;
}
