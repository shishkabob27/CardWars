using System.Collections;
using System.Collections.Generic;
using Facebook;
using UnityEngine;

public class EditorFacebookAccessToken : MonoBehaviour
{
	private const float windowWidth = 592f;

	private float windowHeight = 200f;

	private string accessToken = string.Empty;

	private bool isLoggingIn;

	private static GUISkin fbSkin;

	private GUIStyle greyButton;

	private IEnumerator Start()
	{
		if (!(fbSkin != null))
		{
			string downloadUrl = IntegratedPluginCanvasLocation.FbSkinUrl;
			WWW www = new WWW(downloadUrl);
			yield return www;
			if (www.error != null)
			{
				FbDebug.Error("Could not find the Facebook Skin: " + www.error);
				yield break;
			}
			fbSkin = www.assetBundle.mainAsset as GUISkin;
			www.assetBundle.Unload(false);
		}
	}

	private void OnGUI()
	{
		float y = (float)(Screen.height / 2) - windowHeight / 2f;
		float x = (float)(Screen.width / 2) - 296f;
		if (fbSkin != null)
		{
			GUI.skin = fbSkin;
			greyButton = fbSkin.GetStyle("greyButton");
		}
		else
		{
			greyButton = GUI.skin.button;
		}
		GUI.ModalWindow(GetHashCode(), new Rect(x, y, 592f, windowHeight), OnGUIDialog, "Unity Editor Facebook Login");
	}

	private void OnGUIDialog(int windowId)
	{
		GUI.enabled = !isLoggingIn;
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.Space(10f);
		GUILayout.Label("User Access Token:");
		GUILayout.EndVertical();
		accessToken = GUILayout.TextField(accessToken, GUI.skin.textArea, GUILayout.MinWidth(400f));
		GUILayout.EndHorizontal();
		GUILayout.Space(20f);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Find Access Token"))
		{
			Application.OpenURL(string.Format("https://developers.facebook.com/tools/accesstoken/?app_id={0}", FB.AppId));
		}
		GUILayout.FlexibleSpace();
		GUIContent content = new GUIContent("Login");
		Rect rect = GUILayoutUtility.GetRect(content, GUI.skin.button);
		if (GUI.Button(rect, content))
		{
			EditorFacebook component = FBComponentFactory.GetComponent<EditorFacebook>();
			component.AccessToken = accessToken;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["batch"] = "[{\"method\":\"GET\", \"relative_url\":\"me?fields=id\"},{\"method\":\"GET\", \"relative_url\":\"app?fields=id\"}]";
			dictionary["method"] = "POST";
			dictionary["access_token"] = accessToken;
			FB.API("/", HttpMethod.GET, component.MockLoginCallback, dictionary);
			isLoggingIn = true;
		}
		GUI.enabled = true;
		GUIContent content2 = new GUIContent("Cancel");
		Rect rect2 = GUILayoutUtility.GetRect(content2, greyButton);
		if (GUI.Button(rect2, content2, greyButton))
		{
			FBComponentFactory.GetComponent<EditorFacebook>().MockCancelledLoginCallback();
			Object.Destroy(this);
		}
		GUILayout.EndHorizontal();
		if (Event.current.type == EventType.Repaint)
		{
			windowHeight = rect2.y + rect2.height + (float)GUI.skin.window.padding.bottom;
		}
	}
}
