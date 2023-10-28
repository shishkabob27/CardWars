using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLOTUI : SLOTGameSingleton<SLOTUI>
{
	private void Awake()
	{
		ConnectivityController.enableInputForTutorialCallback = EnableInputForTutorialCallback;
	}

	private void Start()
	{
		SLOTGame.GetInstance().ShowBusyIcon(false);
	}

	public static void GetComponentsOfType(GameObject obj)
	{
		GetComponentsOfType(obj, null);
	}

	public static Component[] GetComponentsOfType(GameObject obj, Type[] types)
	{
		List<Component> list = new List<Component>();
		if (types == null)
		{
			types = new Type[1] { typeof(Behaviour) };
		}
		Type[] array = types;
		foreach (Type type in array)
		{
			Component[] componentsInChildren = obj.GetComponentsInChildren(type);
			if (componentsInChildren == null)
			{
				continue;
			}
			Component[] array2 = componentsInChildren;
			foreach (Component component in array2)
			{
				Behaviour behaviour = component as Behaviour;
				if (behaviour != null)
				{
					list.Add(behaviour);
				}
				Renderer renderer = component as Renderer;
				if (renderer != null)
				{
					list.Add(renderer);
				}
			}
		}
		return list.ToArray();
	}

	public static void EnableAllComponents(GameObject obj, bool enable)
	{
		if (obj != null)
		{
			Component[] componentsInChildren = obj.GetComponentsInChildren<Component>();
			EnableAllComponents(componentsInChildren, enable);
		}
	}

	public static void EnableAllComponents(Component[] components, bool enable)
	{
		if (components == null)
		{
			return;
		}
		foreach (Component component in components)
		{
			Behaviour behaviour = component as Behaviour;
			if (behaviour != null)
			{
				behaviour.enabled = enable;
			}
			Renderer renderer = component as Renderer;
			if (renderer != null)
			{
				renderer.enabled = enable;
			}
			UIWidget uIWidget = component as UIWidget;
			if (uIWidget != null)
			{
				uIWidget.enabled = enable;
			}
			Collider collider = component as Collider;
			if (collider != null)
			{
				collider.enabled = enable;
			}
		}
	}

	public static UIInputEnabler AddInputEnabler(GameObject obj)
	{
		return AddInputEnabler(obj, false, false);
	}

	public static UIInputEnabler AddInputEnabler(GameObject obj, bool removeInputEnablerOnPress, bool removeInputEnablerOnClick)
	{
		if (obj != null)
		{
			UIInputEnabler uIInputEnabler = obj.GetComponent(typeof(UIInputEnabler)) as UIInputEnabler;
			if (uIInputEnabler == null)
			{
				uIInputEnabler = obj.AddComponent(typeof(UIInputEnabler)) as UIInputEnabler;
			}
			if (uIInputEnabler != null)
			{
				uIInputEnabler.enabled = false;
				uIInputEnabler.inputEnabled = false;
				uIInputEnabler.RemoveOnPress = removeInputEnablerOnPress;
				uIInputEnabler.RemoveOnClick = removeInputEnablerOnClick;
				SLOTGameSingleton<SLOTUI>.GetInstance().StartCoroutine(EnableInputEnabler(uIInputEnabler));
			}
			return uIInputEnabler;
		}
		return null;
	}

	public void AddInputEnabler(GameObject obj, bool removeInputEnablerOnPress, bool removeInputEnablerOnClick, float delay)
	{
		StartCoroutine(AddInputEnablerCoroutine(obj, removeInputEnablerOnPress, removeInputEnablerOnClick, delay));
	}

	private IEnumerator AddInputEnablerCoroutine(GameObject obj, bool removeInputEnablerOnPress, bool removeInputEnablerOnClick, float delay)
	{
		float startTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - startTime < delay)
		{
			yield return null;
		}
		AddInputEnabler(obj, removeInputEnablerOnPress, removeInputEnablerOnClick);
	}

	private static IEnumerator EnableInputEnabler(UIInputEnabler e)
	{
		yield return null;
		if (e != null)
		{
			e.enabled = true;
			e.inputEnabled = true;
		}
	}

	public static void RemoveInputEnabler(GameObject o)
	{
		if (!(o != null))
		{
			return;
		}
		UnityEngine.Object[] components = o.GetComponents(typeof(UIInputEnabler));
		UnityEngine.Object[] array = components;
		foreach (UnityEngine.Object @object in array)
		{
			UIInputEnabler uIInputEnabler = @object as UIInputEnabler;
			bool flag = true;
			if (uIInputEnabler != null)
			{
				if (uIInputEnabler.permanent)
				{
					flag = false;
				}
				else
				{
					uIInputEnabler.inputEnabled = false;
					uIInputEnabler.enabled = false;
				}
			}
			if (@object != null && flag)
			{
				UnityEngine.Object.DestroyImmediate(@object);
			}
		}
	}

	public static void RemoveInputEnablers()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(UIInputEnabler));
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			UIInputEnabler uIInputEnabler = @object as UIInputEnabler;
			bool flag = true;
			if (uIInputEnabler != null)
			{
				if (uIInputEnabler.permanent)
				{
					flag = false;
				}
				else
				{
					uIInputEnabler.inputEnabled = false;
					uIInputEnabler.enabled = false;
				}
			}
			if (@object != null && flag)
			{
				UnityEngine.Object.DestroyImmediate(@object);
			}
		}
		UICamera.useInputEnabler = false;
	}

	public static void EnableInputForTutorialCallback(GameObject obj)
	{
	}

	public static bool TouchBegan()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			if (Input.GetTouch(i).phase == TouchPhase.Began)
			{
				return true;
			}
		}
		return false;
	}

	public static bool MaskTest(Texture hit_test_mask, Vector2 p, Rect sr, float alphaThreshold = 0f)
	{
		if (hit_test_mask != null)
		{
			Texture2D texture2D = hit_test_mask as Texture2D;
			if (texture2D != null)
			{
				Vector2 vector = new Vector2(hit_test_mask.width, hit_test_mask.height);
				Rect rect = new Rect(0f, 0f, 1f, 1f);
				float num = (rect.xMin + (p.x - sr.xMin) / sr.width * rect.width) * vector.x;
				float num2 = (rect.yMin + (sr.height - (p.y - sr.yMin)) / sr.height * rect.height) * vector.y;
				Color color = new Color(0f, 0f, 0f, 0f);
				try
				{
					color = texture2D.GetPixel((int)num, (int)num2);
				}
				catch (Exception)
				{
				}
				return color.a > alphaThreshold;
			}
		}
		return false;
	}

	public static void EnableTweener(UITweener tw, bool enable)
	{
		if (tw != null)
		{
			UITweener.Style style = tw.style;
			tw.style = UITweener.Style.Once;
			tw.Play(true);
			tw.Reset();
			tw.style = style;
			tw.enabled = enable;
		}
	}
}
