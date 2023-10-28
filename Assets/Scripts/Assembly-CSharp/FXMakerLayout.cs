using UnityEngine;

public class FXMakerLayout : NgLayout
{
	public enum WINDOWID
	{
		NONE = 0,
		TOP_LEFT = 10,
		TOP_CENTER = 11,
		TOP_RIGHT = 12,
		EFFECT_LIST = 13,
		EFFECT_HIERARCHY = 14,
		EFFECT_CONTROLS = 15,
		TOOLIP_CURSOR = 16,
		MODAL_MSG = 17,
		MENUPOPUP = 18,
		SPRITEPOPUP = 19,
		POPUP = 100,
		RESOURCE_START = 200,
		HINTRECT = 300
	}

	public enum MODAL_TYPE
	{
		MODAL_NONE,
		MODAL_MSG,
		MODAL_OK,
		MODAL_YESNO,
		MODAL_OKCANCEL
	}

	public enum MODALRETURN_TYPE
	{
		MODALRETURN_SHOW,
		MODALRETURN_OK,
		MODALRETURN_CANCEL
	}

	public const string m_CurrentVersion = "v1.3.2";

	public const int m_nMaxResourceListCount = 100;

	public const int m_nMaxPrefabListCount = 500;

	public const int m_nMaxTextureListCount = 500;

	public const int m_nMaxMaterialListCount = 1000;

	public const float m_fScreenShotEffectZoomRate = 1f;

	public const float m_fScreenShotBackZoomRate = 0.6f;

	public const float m_fScrollButtonAspect = 0.55f;

	public const float m_fReloadPreviewTime = 0.5f;

	public const int m_nThumbCaptureSize = 512;

	public const int m_nThumbImageSize = 128;

	protected static float m_fFixedWindowWidth = -1f;

	protected static float m_fTopMenuHeight = -1f;

	protected static bool m_bLastStateTopMenuMini = false;

	public static bool m_bDevelopState = false;

	public static bool m_bDevelopPrefs = false;

	public static Rect m_rectOuterMargin = new Rect(2f, 2f, 0f, 0f);

	public static Rect m_rectInnerMargin = new Rect(7f, 19f, 7f, 4f);

	public static int m_nSidewindowWidthCount = 2;

	public static float m_fButtonMargin = 3f;

	public static float m_fScrollButtonHeight = 70f;

	public static bool m_bMinimizeTopMenu = false;

	public static bool m_bMinimizeAll = false;

	public static float m_fMinimizeClickWidth = 60f;

	public static float m_fMinimizeClickHeight = 20f;

	public static float m_fOriActionToolbarHeight = 126f;

	public static float m_fActionToolbarHeight = m_fOriActionToolbarHeight;

	public static float m_MinimizeHeight = 43f;

	public static float m_fToolMessageHeight = 50f;

	public static float m_fTooltipHeight = 60f;

	public static float m_fModalMessageWidth = 500f;

	public static float m_fModalMessageHeight = 200f;

	public static Color m_ColorButtonHover = new Color(0.7f, 1f, 0.9f, 1f);

	public static Color m_ColorButtonActive = new Color(1f, 1f, 0.6f, 1f);

	public static Color m_ColorButtonMatNormal = new Color(0.5f, 0.7f, 0.7f, 1f);

	public static Color m_ColorButtonUnityEngine = new Color(0.7f, 0.7f, 0.7f, 1f);

	public static Color m_ColorDropFocused = new Color(0.2f, 1f, 0.6f, 0.8f);

	public static Color m_ColorHelpBox = new Color(1f, 0.1f, 0.1f, 1f);

	protected static float m_fArrowIntervalStartTime = 0.2f;

	protected static float m_fArrowIntervalRepeatTime = 0.1f;

	protected static float m_fKeyLastTime;

	public static float GetFixedWindowWidth()
	{
		return 115f;
	}

	public static float GetTopMenuHeight()
	{
		return (!m_bMinimizeAll && !m_bMinimizeTopMenu) ? 92f : m_MinimizeHeight;
	}

	public static int GetWindowId(WINDOWID id)
	{
		return (int)id;
	}

	public static Rect GetChildTopRect(Rect rectParent, int topMargin, int nHeight)
	{
		return new Rect(m_rectInnerMargin.x, (float)topMargin + m_rectInnerMargin.y, rectParent.width - m_rectInnerMargin.x - m_rectInnerMargin.width, nHeight);
	}

	public static Rect GetChildBottomRect(Rect rectParent, int nHeight)
	{
		return new Rect(m_rectInnerMargin.x, rectParent.height - (float)nHeight - m_rectInnerMargin.height, rectParent.width - m_rectInnerMargin.x - m_rectInnerMargin.width, nHeight);
	}

	public static Rect GetChildVerticalRect(Rect rectParent, int topMargin, int count, int pos, int sumCount)
	{
		return new Rect(m_rectInnerMargin.x, (float)topMargin + m_rectInnerMargin.y + (rectParent.height - (float)topMargin - m_rectInnerMargin.y - m_rectInnerMargin.height) / (float)count * (float)pos, rectParent.width - m_rectInnerMargin.x - m_rectInnerMargin.width, (rectParent.height - (float)topMargin - m_rectInnerMargin.y - m_rectInnerMargin.height) / (float)count * (float)sumCount - m_fButtonMargin);
	}

	public static Rect GetInnerVerticalRect(Rect rectBase, int count, int pos, int sumCount)
	{
		return new Rect(rectBase.x, rectBase.y + (rectBase.height + m_fButtonMargin) / (float)count * (float)pos, rectBase.width, (rectBase.height + m_fButtonMargin) / (float)count * (float)sumCount - m_fButtonMargin);
	}

	public static Rect GetChildHorizontalRect(Rect rectParent, int topMargin, int count, int pos, int sumCount)
	{
		return new Rect(m_rectInnerMargin.x + (rectParent.width - m_rectInnerMargin.x - m_rectInnerMargin.width) / (float)count * (float)pos, (float)topMargin + m_rectInnerMargin.y, (rectParent.width - m_rectInnerMargin.x - m_rectInnerMargin.width) / (float)count * (float)sumCount - m_fButtonMargin, rectParent.height - m_rectInnerMargin.y - m_rectInnerMargin.height);
	}

	public static Rect GetInnerHorizontalRect(Rect rectBase, int count, int pos, int sumCount)
	{
		return new Rect(rectBase.x + (rectBase.width + m_fButtonMargin) / (float)count * (float)pos, rectBase.y, (rectBase.width + m_fButtonMargin) / (float)count * (float)sumCount - m_fButtonMargin, rectBase.height);
	}

	public static Rect GetMenuChangeRect()
	{
		return new Rect(m_rectOuterMargin.x, m_rectOuterMargin.y, GetFixedWindowWidth(), GetTopMenuHeight());
	}

	public static Rect GetMenuToolbarRect()
	{
		return new Rect(GetMenuChangeRect().xMax + m_rectOuterMargin.x, m_rectOuterMargin.y, (float)Screen.width - GetMenuChangeRect().width - GetMenuTopRightRect().width - m_rectOuterMargin.x * 4f, GetTopMenuHeight());
	}

	public static Rect GetMenuTopRightRect()
	{
		return new Rect((float)Screen.width - GetFixedWindowWidth() - m_rectOuterMargin.x, m_rectOuterMargin.y, GetFixedWindowWidth(), GetTopMenuHeight());
	}

	public static Rect GetResListRect(int nIndex)
	{
		return new Rect(m_rectOuterMargin.x + (GetFixedWindowWidth() + m_rectOuterMargin.x) * (float)nIndex, GetMenuChangeRect().yMax + m_rectOuterMargin.y, GetFixedWindowWidth(), (float)Screen.height - GetMenuChangeRect().yMax - m_rectOuterMargin.y * 2f);
	}

	public static Rect GetEffectListRect()
	{
		return new Rect(m_rectOuterMargin.x, GetMenuChangeRect().yMax + m_rectOuterMargin.y, GetFixedWindowWidth() * (float)m_nSidewindowWidthCount + m_rectOuterMargin.x, (float)Screen.height - GetMenuChangeRect().yMax - m_rectOuterMargin.y * 2f);
	}

	public static Rect GetEffectHierarchyRect()
	{
		return new Rect((float)Screen.width - (GetFixedWindowWidth() + m_rectOuterMargin.x) * (float)m_nSidewindowWidthCount, GetMenuChangeRect().yMax + m_rectOuterMargin.y, GetFixedWindowWidth() * (float)m_nSidewindowWidthCount + m_rectOuterMargin.x, (float)Screen.height - GetMenuChangeRect().yMax - m_rectOuterMargin.y * 2f);
	}

	public static Rect GetActionToolbarRect()
	{
		return new Rect(m_rectOuterMargin.x * 3f + GetFixedWindowWidth() * (float)m_nSidewindowWidthCount, (float)Screen.height - m_fActionToolbarHeight - m_rectOuterMargin.y, (float)Screen.width - GetMenuChangeRect().width * 4f - m_rectOuterMargin.x * 6f, m_fActionToolbarHeight);
	}

	public static Rect GetToolMessageRect()
	{
		return new Rect(GetFixedWindowWidth() * 2.5f, (float)Screen.height - m_fActionToolbarHeight - m_rectOuterMargin.y - m_fToolMessageHeight - m_fTooltipHeight, (float)Screen.width - GetFixedWindowWidth() * (float)(m_nSidewindowWidthCount * 2 + 1), m_fToolMessageHeight);
	}

	public static Rect GetTooltipRect()
	{
		return new Rect(m_rectOuterMargin.x * 3f + GetFixedWindowWidth() * (float)m_nSidewindowWidthCount, (float)Screen.height - m_fActionToolbarHeight - m_rectOuterMargin.y - m_fTooltipHeight, (float)Screen.width - GetMenuChangeRect().width * 4f - m_rectOuterMargin.x * 6f, m_fTooltipHeight);
	}

	public static Rect GetCursorTooltipRect(Vector2 size)
	{
		return NgLayout.ClampWindow(new Rect(Input.mousePosition.x + 15f, (float)Screen.height - Input.mousePosition.y + 80f, size.x, size.y));
	}

	public static Rect GetModalMessageRect()
	{
		return new Rect(((float)Screen.width - m_fModalMessageWidth) / 2f, ((float)Screen.height - m_fModalMessageHeight - m_fModalMessageHeight / 8f) / 2f, m_fModalMessageWidth, m_fModalMessageHeight);
	}

	public static Rect GetMenuGizmoRect()
	{
		return new Rect(m_rectOuterMargin.x * 3f + GetFixedWindowWidth() * (float)m_nSidewindowWidthCount, GetTopMenuHeight() + m_rectOuterMargin.y, 490f, 26f);
	}

	public static Rect GetClientRect()
	{
		return new Rect(m_rectOuterMargin.x * 3f + GetFixedWindowWidth() * (float)m_nSidewindowWidthCount, GetTopMenuHeight() + m_rectOuterMargin.y, (float)Screen.width - (m_rectOuterMargin.x * 3f + GetFixedWindowWidth() * (float)m_nSidewindowWidthCount) * 2f, (float)Screen.height - m_fActionToolbarHeight - m_rectOuterMargin.y * 3f - GetTopMenuHeight());
	}

	public static Rect GetScrollViewRect(int nWidth, int nButtonCount, int nColumn)
	{
		return new Rect(0f, 0f, nWidth - 2, m_fScrollButtonHeight * (float)(nButtonCount / nColumn + ((0 < nButtonCount % nColumn) ? 1 : 0)) + 25f);
	}

	public static Rect GetScrollGridRect(int nWidth, int nButtonCount, int nColumn)
	{
		return new Rect(0f, 0f, nWidth - 2, m_fScrollButtonHeight * (float)(nButtonCount / nColumn + ((0 < nButtonCount % nColumn) ? 1 : 0)));
	}

	public static Rect GetAspectScrollViewRect(int nWidth, float fAspect, int nButtonCount, int nColumn, bool bImageOnly)
	{
		return new Rect(0f, 0f, nWidth - 4, ((float)((nWidth - 4) / nColumn) * fAspect + (float)((!bImageOnly) ? 10 : 0)) * (float)(nButtonCount / nColumn + ((0 < nButtonCount % nColumn) ? 1 : 0)) + 25f);
	}

	public static Rect GetAspectScrollGridRect(int nWidth, float fAspect, int nButtonCount, int nColumn, bool bImageOnly)
	{
		return new Rect(0f, 0f, nWidth - 4, ((float)((nWidth - 4) / nColumn) * fAspect + (float)((!bImageOnly) ? 10 : 0)) * (float)(nButtonCount / nColumn + ((0 < nButtonCount % nColumn) ? 1 : 0)));
	}

	public static KeyCode GetVaildInputKey(KeyCode key, bool bPress)
	{
		if (bPress || m_fKeyLastTime + m_fArrowIntervalRepeatTime * Time.timeScale < Time.time)
		{
			m_fKeyLastTime = ((!bPress) ? Time.time : (Time.time + m_fArrowIntervalStartTime));
			return key;
		}
		return KeyCode.None;
	}

	public static int GetGridHoverIndex(Rect windowRect, Rect listRect, Rect gridRect, int nCount, int nColumn, GUIStyle style)
	{
		int num = ((style != null) ? style.margin.left : 0);
		int num2 = nCount / nColumn + ((0 < nCount % nColumn) ? 1 : 0);
		float num3 = gridRect.width / (float)nColumn;
		float num4 = gridRect.height / (float)num2;
		Vector2 point = NgLayout.GetGUIMousePosition() - new Vector2(windowRect.x, windowRect.y);
		if (!listRect.Contains(point))
		{
			return -1;
		}
		for (int i = 0; i < nCount; i++)
		{
			if (new Rect(listRect.x + num3 * (float)(i % nColumn) + (float)num, listRect.y + num4 * (float)(i / nColumn) + (float)num, num3 - (float)(num * 2), num4 - (float)(num * 2)).Contains(point))
			{
				return i;
			}
		}
		return -1;
	}

	public static int TooltipToolbar(Rect windowRect, Rect gridRect, int nGridIndex, GUIContent[] cons)
	{
		return TooltipToolbar(windowRect, gridRect, nGridIndex, cons, null);
	}

	public static int TooltipToolbar(Rect windowRect, Rect gridRect, int nGridIndex, GUIContent[] cons, GUIStyle style)
	{
		int result = GUI.Toolbar(gridRect, nGridIndex, cons, style);
		int gridHoverIndex = GetGridHoverIndex(windowRect, gridRect, gridRect, cons.Length, cons.Length, null);
		if (0 <= gridHoverIndex)
		{
			GUI.tooltip = cons[gridHoverIndex].tooltip;
		}
		return result;
	}

	public static int TooltipSelectionGrid(Rect windowRect, Rect listRect, int nGridIndex, GUIContent[] cons, int nColumCount)
	{
		return TooltipSelectionGrid(windowRect, listRect, listRect, nGridIndex, cons, nColumCount, null);
	}

	public static int TooltipSelectionGrid(Rect windowRect, Rect listRect, int nGridIndex, GUIContent[] cons, int nColumCount, GUIStyle style)
	{
		return TooltipSelectionGrid(windowRect, listRect, listRect, nGridIndex, cons, nColumCount, null);
	}

	public static int TooltipSelectionGrid(Rect windowRect, Rect listRect, Rect gridRect, int nGridIndex, GUIContent[] cons, int nColumCount)
	{
		return TooltipSelectionGrid(windowRect, listRect, gridRect, nGridIndex, cons, nColumCount, null);
	}

	public static int TooltipSelectionGrid(Rect windowRect, Rect listRect, Rect gridRect, int nGridIndex, GUIContent[] cons, int nColumCount, GUIStyle style)
	{
		int result = GUI.SelectionGrid(gridRect, nGridIndex, cons, nColumCount, style);
		int gridHoverIndex = GetGridHoverIndex(windowRect, listRect, gridRect, cons.Length, nColumCount, null);
		if (0 <= gridHoverIndex)
		{
			GUI.tooltip = cons[gridHoverIndex].tooltip;
		}
		return result;
	}

	public static void ModalWindow(Rect rect, GUI.WindowFunction func, string title)
	{
		GUI.Window(GUIUtility.GetControlID(FocusType.Passive), rect, delegate(int id)
		{
			GUI.depth = 0;
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			if (GUIUtility.hotControl < controlID)
			{
				setHotControl(0);
			}
			func(id);
			int controlID2 = GUIUtility.GetControlID(FocusType.Passive);
			if (GUIUtility.hotControl < controlID || (GUIUtility.hotControl > controlID2 && controlID2 != -1))
			{
				setHotControl(-1);
			}
			GUI.FocusWindow(id);
			GUI.BringWindowToFront(id);
		}, title);
	}

	private static void setHotControl(int id)
	{
		if (new Rect(0f, 0f, Screen.width, Screen.height).Contains(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)))
		{
			GUIUtility.hotControl = id;
		}
	}
}
