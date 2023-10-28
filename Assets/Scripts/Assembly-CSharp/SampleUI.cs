using UnityEngine;

public class SampleUI : MonoBehaviour
{
	public GUISkin skin;

	public Color titleColor = Color.white;

	private GUIStyle titleStyle;

	private GUIStyle statusStyle;

	private GUIStyle helpStyle;

	private Rect topBarRect = new Rect(0f, -4f, 600f, 56f);

	private Rect backButtonRect = new Rect(5f, 2f, 80f, 46f);

	private Rect titleRect = new Rect(100f, 2f, 400f, 46f);

	private Rect helpButtonRect = new Rect(515f, 2f, 80f, 46f);

	private Rect statusTextRect = new Rect(30f, 336f, 540f, 60f);

	private Rect helpRect = new Rect(50f, 60f, 500f, 300f);

	private string statusText = string.Empty;

	public bool showStatusText = true;

	public string helpText = string.Empty;

	public bool showHelpButton = true;

	public bool showHelp;

	public static readonly float VirtualScreenWidth = 600f;

	public static readonly float VirtualScreenHeight = 400f;

	public string StatusText
	{
		get
		{
			return statusText;
		}
		set
		{
			statusText = value;
		}
	}

	private void Awake()
	{
		titleStyle = new GUIStyle(skin.label);
		titleStyle.alignment = TextAnchor.MiddleCenter;
		titleStyle.normal.textColor = titleColor;
		statusStyle = new GUIStyle(skin.label);
		statusStyle.alignment = TextAnchor.LowerCenter;
		helpStyle = new GUIStyle(skin.label);
		helpStyle.alignment = TextAnchor.UpperLeft;
		helpStyle.padding.left = 5;
		helpStyle.padding.right = 5;
	}

	public static void ApplyVirtualScreen()
	{
		GUI.matrix = Matrix4x4.Scale(new Vector3((float)Screen.width / VirtualScreenWidth, (float)Screen.height / VirtualScreenHeight, 1f));
	}

	protected virtual void OnGUI()
	{
		if (skin != null)
		{
			GUI.skin = skin;
		}
		ApplyVirtualScreen();
		GUI.Box(topBarRect, string.Empty);
		if (GUI.Button(backButtonRect, "Back"))
		{
			Application.LoadLevel(0);
		}
		GUI.Label(titleRect, "FingerGestures - " + base.name, titleStyle);
		if (showStatusText)
		{
			GUI.Label(statusTextRect, statusText, statusStyle);
		}
		if (helpText.Length > 0 && showHelpButton && !showHelp && GUI.Button(helpButtonRect, "Help"))
		{
			showHelp = true;
		}
		if (showHelp)
		{
			GUI.Box(helpRect, "Help");
			GUILayout.BeginArea(helpRect);
			GUILayout.BeginVertical();
			GUILayout.Space(25f);
			GUILayout.Label(helpText, helpStyle);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Close", GUILayout.Height(40f)))
			{
				showHelp = false;
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
}
