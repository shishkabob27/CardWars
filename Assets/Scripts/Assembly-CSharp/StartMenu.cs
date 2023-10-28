using UnityEngine;

public class StartMenu : MonoBehaviour
{
	public GUIStyle titleStyle;

	public GUIStyle buttonStyle;

	public float buttonHeight = 80f;

	public Transform itemsTree;

	private Transform currentMenuRoot;

	private Rect screenRect = new Rect(0f, 0f, SampleUI.VirtualScreenWidth, SampleUI.VirtualScreenHeight);

	public float menuWidth = 450f;

	public float sideBorder = 30f;

	public Transform CurrentMenuRoot
	{
		get
		{
			return currentMenuRoot;
		}
		set
		{
			currentMenuRoot = value;
		}
	}

	private void Start()
	{
		CurrentMenuRoot = itemsTree;
	}

	private void OnGUI()
	{
		SampleUI.ApplyVirtualScreen();
		GUILayout.BeginArea(screenRect);
		GUILayout.BeginHorizontal();
		GUILayout.Space(sideBorder);
		if ((bool)CurrentMenuRoot)
		{
			GUILayout.BeginVertical();
			GUILayout.Space(15f);
			GUILayout.Label(CurrentMenuRoot.name, titleStyle);
			for (int i = 0; i < CurrentMenuRoot.childCount; i++)
			{
				Transform child = CurrentMenuRoot.GetChild(i);
				if (GUILayout.Button(child.name, GUILayout.Height(buttonHeight)))
				{
					MenuNode component = child.GetComponent<MenuNode>();
					if ((bool)component && component.sceneName != null && component.sceneName.Length > 0)
					{
						Application.LoadLevel(component.sceneName);
					}
					else if (child.childCount > 0)
					{
						CurrentMenuRoot = child;
					}
				}
				GUILayout.Space(5f);
			}
			GUILayout.FlexibleSpace();
			if (CurrentMenuRoot != itemsTree && (bool)CurrentMenuRoot.parent)
			{
				if (GUILayout.Button("<< BACK <<", GUILayout.Height(buttonHeight)))
				{
					CurrentMenuRoot = CurrentMenuRoot.parent;
				}
				GUILayout.Space(15f);
			}
			GUILayout.EndVertical();
		}
		GUILayout.Space(sideBorder);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
