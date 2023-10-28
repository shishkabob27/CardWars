using UnityEngine;

public class FBSettings : ScriptableObject
{
	private const string facebookSettingsAssetName = "FacebookSettings";

	private const string facebookSettingsPath = "Facebook/Resources";

	private const string facebookSettingsAssetExtension = ".asset";

	private static FBSettings instance;

	[SerializeField]
	private int selectedAppIndex;

	[SerializeField]
	private string[] appIds = new string[1] { "0" };

	[SerializeField]
	private string[] appLabels = new string[1] { "App Name" };

	[SerializeField]
	private bool cookie = true;

	[SerializeField]
	private bool logging = true;

	[SerializeField]
	private bool status = true;

	[SerializeField]
	private bool xfbml;

	[SerializeField]
	private bool frictionlessRequests = true;

	[SerializeField]
	private string iosURLSuffix = string.Empty;

	private static FBSettings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load("FacebookSettings") as FBSettings;
				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<FBSettings>();
				}
			}
			return instance;
		}
	}

	public int SelectedAppIndex
	{
		get
		{
			return selectedAppIndex;
		}
	}

	public string[] AppIds
	{
		get
		{
			return appIds;
		}
		set
		{
			if (appIds != value)
			{
				appIds = value;
				DirtyEditor();
			}
		}
	}

	public string[] AppLabels
	{
		get
		{
			return appLabels;
		}
		set
		{
			if (appLabels != value)
			{
				appLabels = value;
				DirtyEditor();
			}
		}
	}

	public static string[] AllAppIds
	{
		get
		{
			return Instance.AppIds;
		}
	}

	public static string AppId
	{
		get
		{
			return Instance.AppIds[Instance.SelectedAppIndex];
		}
	}

	public static bool IsValidAppId
	{
		get
		{
			return AppId != null && AppId.Length > 0 && !AppId.Equals("0");
		}
	}

	public static bool Cookie
	{
		get
		{
			return Instance.cookie;
		}
		set
		{
			if (Instance.cookie != value)
			{
				Instance.cookie = value;
				DirtyEditor();
			}
		}
	}

	public static bool Logging
	{
		get
		{
			return Instance.logging;
		}
		set
		{
			if (Instance.logging != value)
			{
				Instance.logging = value;
				DirtyEditor();
			}
		}
	}

	public static bool Status
	{
		get
		{
			return Instance.status;
		}
		set
		{
			if (Instance.status != value)
			{
				Instance.status = value;
				DirtyEditor();
			}
		}
	}

	public static bool Xfbml
	{
		get
		{
			return Instance.xfbml;
		}
		set
		{
			if (Instance.xfbml != value)
			{
				Instance.xfbml = value;
				DirtyEditor();
			}
		}
	}

	public static string IosURLSuffix
	{
		get
		{
			return Instance.iosURLSuffix;
		}
		set
		{
			if (Instance.iosURLSuffix != value)
			{
				Instance.iosURLSuffix = value;
				DirtyEditor();
			}
		}
	}

	public static string ChannelUrl
	{
		get
		{
			return "/channel.html";
		}
	}

	public static bool FrictionlessRequests
	{
		get
		{
			return Instance.frictionlessRequests;
		}
		set
		{
			if (Instance.frictionlessRequests != value)
			{
				Instance.frictionlessRequests = value;
				DirtyEditor();
			}
		}
	}

	public void SetAppIndex(int index)
	{
		if (selectedAppIndex != index)
		{
			selectedAppIndex = index;
			DirtyEditor();
		}
	}

	public void SetAppId(int index, string value)
	{
		if (appIds[index] != value)
		{
			appIds[index] = value;
			DirtyEditor();
		}
	}

	public void SetAppLabel(int index, string value)
	{
		if (appLabels[index] != value)
		{
			AppLabels[index] = value;
			DirtyEditor();
		}
	}

	private static void DirtyEditor()
	{
	}
}
