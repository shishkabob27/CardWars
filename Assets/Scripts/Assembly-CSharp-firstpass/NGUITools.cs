using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class NGUITools
{
	private static AudioListener mListener;

	private static bool mLoaded = false;

	private static float mGlobalVolume = 1f;

	private static Color mInvisible = new Color(0f, 0f, 0f, 0f);

	private static PropertyInfo mSystemCopyBuffer = null;

	public static float soundVolume
	{
		get
		{
			if (!mLoaded)
			{
				mLoaded = true;
				mGlobalVolume = PlayerPrefs.GetFloat("Sound", 1f);
			}
			return mGlobalVolume;
		}
		set
		{
			if (mGlobalVolume != value)
			{
				mLoaded = true;
				mGlobalVolume = value;
				PlayerPrefs.SetFloat("Sound", value);
			}
		}
	}

	public static bool fileAccess
	{
		get
		{
			//return Application.platform != RuntimePlatform.WindowsWebPlayer && Application.platform != RuntimePlatform.OSXWebPlayer;
			return true;
		}
	}

	public static string clipboard
	{
		get
		{
			PropertyInfo systemCopyBufferProperty = GetSystemCopyBufferProperty();
			return (systemCopyBufferProperty == null) ? null : ((string)systemCopyBufferProperty.GetValue(null, null));
		}
		set
		{
			PropertyInfo systemCopyBufferProperty = GetSystemCopyBufferProperty();
			if (systemCopyBufferProperty != null)
			{
				systemCopyBufferProperty.SetValue(null, value, null);
			}
		}
	}

	public static AudioSource PlaySound(AudioClip clip)
	{
		return PlaySound(clip, 1f, 1f);
	}

	public static AudioSource PlaySound(AudioClip clip, float volume)
	{
		return PlaySound(clip, volume, 1f);
	}

	public static AudioSource PlaySound(AudioClip clip, float volume, float pitch)
	{
		if (KFFSoundPlayer.GetPlayOneShotFunction() != null)
		{
			return KFFSoundPlayer.PlayOneShot(clip, volume, pitch);
		}
		volume *= soundVolume;
		if (clip != null && volume > 0.01f)
		{
			if (mListener == null)
			{
				mListener = UnityEngine.Object.FindObjectOfType(typeof(AudioListener)) as AudioListener;
				if (mListener == null)
				{
					Camera camera = Camera.main;
					if (camera == null)
					{
						camera = UnityEngine.Object.FindObjectOfType(typeof(Camera)) as Camera;
					}
					if (camera != null)
					{
						mListener = camera.gameObject.AddComponent<AudioListener>();
					}
				}
			}
			if (mListener != null && mListener.enabled && GetActive(mListener.gameObject))
			{
				AudioSource audioSource = mListener.GetComponent<AudioSource>();
				if (audioSource == null)
				{
					audioSource = mListener.gameObject.AddComponent<AudioSource>();
				}
				audioSource.pitch = pitch;
				audioSource.PlayOneShot(clip, volume);
				return audioSource;
			}
		}
		return null;
	}

	public static WWW OpenURL(string url)
	{
		WWW result = null;
		try
		{
			result = new WWW(url);
		}
		catch (Exception)
		{
		}
		return result;
	}

	public static WWW OpenURL(string url, WWWForm form)
	{
		if (form == null)
		{
			return OpenURL(url);
		}
		WWW result = null;
		try
		{
			result = new WWW(url, form);
		}
		catch (Exception)
		{
		}
		return result;
	}

	public static int RandomRange(int min, int max)
	{
		if (min == max)
		{
			return min;
		}
		return UnityEngine.Random.Range(min, max + 1);
	}

	public static string GetHierarchy(GameObject obj)
	{
		string text = obj.name;
		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			text = obj.name + "/" + text;
		}
		return "\"" + text + "\"";
	}

	public static Color ParseColor(string text, int offset)
	{
		int num = (NGUIMath.HexToDecimal(text[offset]) << 4) | NGUIMath.HexToDecimal(text[offset + 1]);
		int num2 = (NGUIMath.HexToDecimal(text[offset + 2]) << 4) | NGUIMath.HexToDecimal(text[offset + 3]);
		int num3 = (NGUIMath.HexToDecimal(text[offset + 4]) << 4) | NGUIMath.HexToDecimal(text[offset + 5]);
		float num4 = 0.003921569f;
		return new Color(num4 * (float)num, num4 * (float)num2, num4 * (float)num3);
	}

	public static string EncodeColor(Color c)
	{
		int num = 0xFFFFFF & (NGUIMath.ColorToInt(c) >> 8);
		return NGUIMath.DecimalToHex(num);
	}

	public static int ParseSymbol(string text, int index, List<Color> colors, bool premultiply)
	{
		int length = text.Length;
		if (index + 2 < length)
		{
			if (text[index + 1] == '-')
			{
				if (text[index + 2] == ']')
				{
					if (colors != null && colors.Count > 1)
					{
						colors.RemoveAt(colors.Count - 1);
					}
					return 3;
				}
			}
			else if (index + 7 < length && text[index + 7] == ']')
			{
				if (colors != null)
				{
					Color color = ParseColor(text, index + 1);
					if (EncodeColor(color) != text.Substring(index + 1, 6).ToUpper())
					{
						return 0;
					}
					color.a = colors[colors.Count - 1].a;
					if (premultiply && color.a != 1f)
					{
						color = Color.Lerp(mInvisible, color, color.a);
					}
					colors.Add(color);
				}
				return 8;
			}
		}
		return 0;
	}

	public static string StripSymbols(string text)
	{
		if (text != null)
		{
			int num = 0;
			int length = text.Length;
			while (num < length)
			{
				char c = text[num];
				if (c == '[')
				{
					int num2 = ParseSymbol(text, num, null, false);
					if (num2 > 0)
					{
						text = text.Remove(num, num2);
						length = text.Length;
						continue;
					}
				}
				num++;
			}
		}
		return text;
	}

	public static T[] FindActive<T>() where T : Component
	{
		return UnityEngine.Object.FindObjectsOfType(typeof(T)) as T[];
	}

	public static Camera FindCameraForLayer(int layer)
	{
		int num = 1 << layer;
		Camera[] array = FindActive<Camera>();
		int i = 0;
		for (int num2 = array.Length; i < num2; i++)
		{
			Camera camera = array[i];
			if ((camera.cullingMask & num) != 0)
			{
				return camera;
			}
		}
		return null;
	}

	public static BoxCollider AddWidgetCollider(GameObject go)
	{
		if (go != null)
		{
			Collider component = go.GetComponent<Collider>();
			BoxCollider boxCollider = component as BoxCollider;
			if (boxCollider == null)
			{
				if (component != null)
				{
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy(component);
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(component);
					}
				}
				boxCollider = go.AddComponent<BoxCollider>();
			}
			int num = CalculateNextDepth(go);
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(go.transform);
			boxCollider.isTrigger = true;
			boxCollider.center = bounds.center + Vector3.back * ((float)num * 0.25f);
			boxCollider.size = new Vector3(bounds.size.x, bounds.size.y, 0f);
			return boxCollider;
		}
		return null;
	}

	public static string GetName<T>() where T : Component
	{
		string text = typeof(T).ToString();
		if (text.StartsWith("UI"))
		{
			text = text.Substring(2);
		}
		else if (text.StartsWith("UnityEngine."))
		{
			text = text.Substring(12);
		}
		return text;
	}

	public static GameObject AddChild(GameObject parent)
	{
		GameObject gameObject = new GameObject();
		if (parent != null)
		{
			Transform transform = gameObject.transform;
			transform.parent = parent.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			gameObject.layer = parent.layer;
		}
		return gameObject;
	}

	public static GameObject AddChild(GameObject parent, GameObject prefab)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
		if (gameObject != null && parent != null)
		{
			Transform transform = gameObject.transform;
			transform.SetParent(parent.transform);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			gameObject.layer = parent.layer;
		}
		return gameObject;
	}

	public static int CalculateNextDepth(GameObject go)
	{
		int num = -1;
		UIWidget[] componentsInChildren = go.GetComponentsInChildren<UIWidget>();
		int i = 0;
		for (int num2 = componentsInChildren.Length; i < num2; i++)
		{
			num = Mathf.Max(num, componentsInChildren[i].depth);
		}
		return num + 1;
	}

	public static T AddChild<T>(GameObject parent) where T : Component
	{
		GameObject gameObject = AddChild(parent);
		gameObject.name = GetName<T>();
		return gameObject.AddComponent<T>();
	}

	public static T AddWidget<T>(GameObject go) where T : UIWidget
	{
		int depth = CalculateNextDepth(go);
		T result = AddChild<T>(go);
		result.depth = depth;
		Transform transform = result.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = new Vector3(100f, 100f, 1f);
		result.gameObject.layer = go.layer;
		return result;
	}

	public static UISprite AddSprite(GameObject go, UIAtlas atlas, string spriteName)
	{
		UIAtlas.Sprite sprite = ((!(atlas != null)) ? null : atlas.GetSprite(spriteName));
		UISprite uISprite = AddWidget<UISprite>(go);
		uISprite.type = ((sprite != null && !(sprite.inner == sprite.outer)) ? UISprite.Type.Sliced : UISprite.Type.Simple);
		uISprite.atlas = atlas;
		uISprite.spriteName = spriteName;
		return uISprite;
	}

	public static GameObject GetRoot(GameObject go)
	{
		Transform transform = go.transform;
		while (true)
		{
			Transform parent = transform.parent;
			if (parent == null)
			{
				break;
			}
			transform = parent;
		}
		return transform.gameObject;
	}

	public static T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null)
		{
			return (T)null;
		}
		object component = go.GetComponent<T>();
		if (component == null)
		{
			Transform parent = go.transform.parent;
			while (parent != null && component == null)
			{
				component = parent.gameObject.GetComponent<T>();
				parent = parent.parent;
			}
		}
		return (T)component;
	}

	public static void Destroy(UnityEngine.Object obj)
	{
		if (!(obj != null))
		{
			return;
		}
		if (Application.isPlaying)
		{
			if (obj is GameObject)
			{
				GameObject gameObject = obj as GameObject;
				gameObject.transform.parent = null;
			}
			UnityEngine.Object.Destroy(obj);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
	}

	public static void DestroyImmediate(UnityEngine.Object obj)
	{
		if (obj != null)
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			else
			{
				UnityEngine.Object.Destroy(obj);
			}
		}
	}

	public static void Broadcast(string funcName)
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			array[i].SendMessage(funcName, SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void Broadcast(string funcName, object param)
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			array[i].SendMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
		}
	}

	public static bool IsChild(Transform parent, Transform child)
	{
		if (parent == null || child == null)
		{
			return false;
		}
		while (child != null)
		{
			if (child == parent)
			{
				return true;
			}
			child = child.parent;
		}
		return false;
	}

	private static void Activate(Transform t)
	{
		SetActiveSelf(t.gameObject, true);
		int i = 0;
		for (int childCount = t.childCount; i < childCount; i++)
		{
			Transform child = t.GetChild(i);
			if (child.gameObject.activeSelf)
			{
				return;
			}
		}
		int j = 0;
		for (int childCount2 = t.childCount; j < childCount2; j++)
		{
			Transform child2 = t.GetChild(j);
			Activate(child2);
		}
	}

	private static void Deactivate(Transform t)
	{
		SetActiveSelf(t.gameObject, false);
	}

	public static void SetActive(GameObject go, bool state)
	{
		if (state)
		{
			Activate(go.transform);
		}
		else
		{
			Deactivate(go.transform);
		}
	}

	public static void SetActiveChildren(GameObject go, bool state)
	{
		Transform transform = go.transform;
		if (state)
		{
			int i = 0;
			for (int childCount = transform.childCount; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				Activate(child);
			}
		}
		else
		{
			int j = 0;
			for (int childCount2 = transform.childCount; j < childCount2; j++)
			{
				Transform child2 = transform.GetChild(j);
				Deactivate(child2);
			}
		}
	}

	public static bool GetActive(GameObject go)
	{
		return (bool)go && go.activeInHierarchy;
	}

	public static void SetActiveSelf(GameObject go, bool state)
	{
		go.SetActive(state);
	}

	public static void SetLayer(GameObject go, int layer)
	{
		go.layer = layer;
		Transform transform = go.transform;
		int i = 0;
		for (int childCount = transform.childCount; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			SetLayer(child.gameObject, layer);
		}
	}

	public static Vector3 Round(Vector3 v)
	{
		v.x = Mathf.Round(v.x);
		v.y = Mathf.Round(v.y);
		v.z = Mathf.Round(v.z);
		return v;
	}

	public static void MakePixelPerfect(Transform t)
	{
		UIWidget component = t.GetComponent<UIWidget>();
		if (component != null)
		{
			component.MakePixelPerfect();
			return;
		}
		t.localPosition = Round(t.localPosition);
		t.localScale = Round(t.localScale);
		int i = 0;
		for (int childCount = t.childCount; i < childCount; i++)
		{
			MakePixelPerfect(t.GetChild(i));
		}
	}

	public static bool Save(string fileName, byte[] bytes)
	{
		if (!fileAccess)
		{
			return false;
		}
		string path = Application.persistentDataPath + "/" + fileName;
		if (bytes == null)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			return true;
		}
		FileStream fileStream = null;
		try
		{
			fileStream = File.Create(path);
		}
		catch (Exception ex)
		{
			NGUIDebug.Log(ex.Message);
			return false;
		}
		fileStream.Write(bytes, 0, bytes.Length);
		fileStream.Close();
		return true;
	}

	public static byte[] Load(string fileName)
	{
		if (!fileAccess)
		{
			return null;
		}
		string path = Application.persistentDataPath + "/" + fileName;
		if (File.Exists(path))
		{
			return File.ReadAllBytes(path);
		}
		return null;
	}

	public static Color ApplyPMA(Color c)
	{
		if (c.a != 1f)
		{
			c.r *= c.a;
			c.g *= c.a;
			c.b *= c.a;
		}
		return c;
	}

	public static void MarkParentAsChanged(GameObject go)
	{
		UIWidget[] componentsInChildren = go.GetComponentsInChildren<UIWidget>();
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			componentsInChildren[i].ParentHasChanged();
		}
	}

	private static PropertyInfo GetSystemCopyBufferProperty()
	{
		if (mSystemCopyBuffer == null)
		{
			Type typeFromHandle = typeof(GUIUtility);
			mSystemCopyBuffer = typeFromHandle.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
		}
		return mSystemCopyBuffer;
	}
}
