using UnityEngine;

public class NgLayout
{
	protected static Color m_GuiOldColor;

	protected static bool m_GuiOldEnable;

	public static Rect GetZeroRect()
	{
		return new Rect(0f, 0f, 0f, 0f);
	}

	public static Rect GetSumRect(Rect rect1, Rect rect2)
	{
		return GetOffsetRect(rect1, Mathf.Min(0f, rect2.xMin - rect1.xMin), Mathf.Min(0f, rect2.yMin - rect1.yMin), Mathf.Max(0f, rect2.xMax - rect1.xMax), Mathf.Max(0f, rect2.yMax - rect1.yMax));
	}

	public static Rect GetOffsetRect(Rect rect, float left, float top)
	{
		return new Rect(rect.x + left, rect.y + top, rect.width, rect.height);
	}

	public static Rect GetOffsetRect(Rect rect, float left, float top, float right, float bottom)
	{
		return new Rect(rect.x + left, rect.y + top, rect.width - left + right, rect.height - top + bottom);
	}

	public static Rect GetOffsetRect(Rect rect, float fOffset)
	{
		return new Rect(rect.x - fOffset, rect.y - fOffset, rect.width + fOffset * 2f, rect.height + fOffset * 2f);
	}

	public static Rect GetVOffsetRect(Rect rect, float fOffset)
	{
		return new Rect(rect.x, rect.y - fOffset, rect.width, rect.height + fOffset * 2f);
	}

	public static Rect GetHOffsetRect(Rect rect, float fOffset)
	{
		return new Rect(rect.x - fOffset, rect.y, rect.width + fOffset * 2f, rect.height);
	}

	public static Rect GetOffsetRateRect(Rect rect, float fOffsetRate)
	{
		return new Rect(rect.x - Mathf.Abs(rect.x) * fOffsetRate, rect.y - Mathf.Abs(rect.y) * fOffsetRate, rect.width + Mathf.Abs(rect.x) * fOffsetRate * 2f, rect.height + Mathf.Abs(rect.y) * fOffsetRate * 2f);
	}

	public static Rect GetZeroStartRect(Rect rect)
	{
		return new Rect(0f, 0f, rect.width, rect.height);
	}

	public static Rect GetLeftRect(Rect rect, float width)
	{
		return new Rect(rect.x, rect.y, width, rect.height);
	}

	public static Rect GetRightRect(Rect rect, float width)
	{
		return new Rect(rect.x + rect.width - width, rect.y, width, rect.height);
	}

	public static Rect GetInnerTopRect(Rect rectBase, int topMargin, int nHeight)
	{
		return new Rect(rectBase.x, (float)topMargin + rectBase.y, rectBase.width, nHeight);
	}

	public static Rect GetInnerBottomRect(Rect rectBase, int nHeight)
	{
		return new Rect(rectBase.x, rectBase.y + rectBase.height - (float)nHeight, rectBase.width, nHeight);
	}

	public static Vector2 ClampPoint(Rect rect, Vector2 point)
	{
		if (point.x < rect.xMin)
		{
			point.x = rect.xMin;
		}
		if (point.y < rect.yMin)
		{
			point.y = rect.yMin;
		}
		if (rect.xMax < point.x)
		{
			point.x = rect.xMax;
		}
		if (rect.yMax < point.y)
		{
			point.y = rect.yMax;
		}
		return point;
	}

	public static Vector3 ClampPoint(Rect rect, Vector3 point)
	{
		if (point.x < rect.xMin)
		{
			point.x = rect.xMin;
		}
		if (point.y < rect.yMin)
		{
			point.y = rect.yMin;
		}
		if (rect.xMax < point.x)
		{
			point.x = rect.xMax;
		}
		if (rect.yMax < point.y)
		{
			point.y = rect.yMax;
		}
		return point;
	}

	public static Rect ClampWindow(Rect popupRect)
	{
		if (popupRect.y < 0f)
		{
			popupRect.y = 0f;
		}
		if ((float)Screen.width < popupRect.xMax)
		{
			popupRect.x -= popupRect.xMax - (float)Screen.width;
		}
		if ((float)Screen.height < popupRect.yMax)
		{
			popupRect.y -= popupRect.yMax - (float)Screen.height;
		}
		return popupRect;
	}

	public static bool GUIToggle(Rect pos, bool bToggle, GUIContent content, bool bEnable)
	{
		bool enabled = GUI.enabled;
		if (!bEnable)
		{
			GUI.enabled = false;
		}
		bToggle = GUI.Toggle(pos, bToggle, content);
		GUI.enabled = enabled;
		return bToggle;
	}

	public static bool GUIButton(Rect pos, string name, bool bEnable)
	{
		bool enabled = GUI.enabled;
		if (!bEnable)
		{
			GUI.enabled = false;
		}
		bool result = GUI.Button(pos, name);
		GUI.enabled = enabled;
		return result;
	}

	public static bool GUIButton(Rect pos, GUIContent content, bool bEnable)
	{
		bool enabled = GUI.enabled;
		if (!bEnable)
		{
			GUI.enabled = false;
		}
		bool result = GUI.Button(pos, content);
		GUI.enabled = enabled;
		return result;
	}

	public static bool GUIButton(Rect pos, GUIContent content, GUIStyle style, bool bEnable)
	{
		bool enabled = GUI.enabled;
		if (!bEnable)
		{
			GUI.enabled = false;
		}
		bool result = GUI.Button(pos, content, style);
		GUI.enabled = enabled;
		return result;
	}

	public static string GUITextField(Rect pos, string name, bool bEnable)
	{
		bool enabled = GUI.enabled;
		if (!bEnable)
		{
			GUI.enabled = false;
		}
		string result = GUI.TextField(pos, name);
		GUI.enabled = enabled;
		return result;
	}

	public static bool GUIEnableBackup(bool newEnable)
	{
		m_GuiOldEnable = GUI.enabled;
		GUI.enabled = newEnable;
		return m_GuiOldEnable;
	}

	public static void GUIEnableRestore()
	{
		GUI.enabled = m_GuiOldEnable;
	}

	public static Color GUIColorBackup(Color newColor)
	{
		m_GuiOldColor = GUI.color;
		GUI.color = newColor;
		return m_GuiOldColor;
	}

	public static void GUIColorRestore()
	{
		GUI.color = m_GuiOldColor;
	}

	public static Vector2 GetGUIMousePosition()
	{
		return new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
	}

	public static float GetWorldPerScreenPixel(Vector3 worldPoint)
	{
		Camera main = Camera.main;
		if (main == null)
		{
			return 0f;
		}
		float distanceToPoint = new Plane(main.transform.forward, main.transform.position).GetDistanceToPoint(worldPoint);
		float num = 100f;
		return Vector3.Distance(main.ScreenToWorldPoint(new Vector3(Screen.width / 2, (float)(Screen.height / 2) - num / 2f, distanceToPoint)), main.ScreenToWorldPoint(new Vector3(Screen.width / 2, (float)(Screen.height / 2) + num / 2f, distanceToPoint))) / num;
	}

	public static Vector3 GetScreenToWorld(Vector3 targetWorld, Vector2 screenPos)
	{
		Camera main = Camera.main;
		if (main == null)
		{
			return Vector3.zero;
		}
		float distanceToPoint = new Plane(main.transform.forward, main.transform.position).GetDistanceToPoint(targetWorld);
		return main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distanceToPoint));
	}

	public static Vector3 GetWorldToScreen(Vector3 targetWorld)
	{
		Camera main = Camera.main;
		if (main == null)
		{
			return Vector3.zero;
		}
		return main.WorldToScreenPoint(targetWorld);
	}
}
