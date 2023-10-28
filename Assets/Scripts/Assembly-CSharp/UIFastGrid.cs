using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFastGrid : MonoBehaviour
{
	public enum Arrangement
	{
		Horizontal,
		Vertical
	}

	public delegate void FillItemCallback(GameObject item, object data);

	public delegate GameObject PrefabCallback(object data);

	public Arrangement arrangement;

	public int maxPerLine;

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	private FillItemCallback fillItem;

	private int numItems;

	private List<object> itemData;

	private PrefabCallback getPrefab;

	private UIPanel parentPanel;

	private UIDraggablePanel dragPanel;

	private SLOTUIAnchor uiAnchor;

	private bool isInitialized;

	private bool needsUpdate;

	private Vector4 prevClipRange;

	private Dictionary<int, GameObject> itemsShowing = new Dictionary<int, GameObject>();

	private void OnEnable()
	{
		parentPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
		dragPanel = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
		uiAnchor = dragPanel.gameObject.GetComponent<SLOTUIAnchor>();
		isInitialized = false;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		itemsShowing.Clear();
		foreach (Transform item in base.gameObject.transform)
		{
			PoolManager.ReleaseLater(item.gameObject, 0.1f);
		}
	}

	private void CalculatePosition(int num, ref float x, ref float y)
	{
		x = 0f;
		y = 0f;
		if (arrangement == Arrangement.Horizontal)
		{
			if (maxPerLine == 0)
			{
				x = (float)num * cellWidth;
				y = 0f;
			}
			else
			{
				x = (float)(num % maxPerLine) * cellWidth;
				y = (float)(num / maxPerLine) * (0f - cellHeight);
			}
		}
		else if (maxPerLine == 0)
		{
			x = 0f;
			y = (float)num * (0f - cellHeight);
		}
		else
		{
			x = (float)(num / maxPerLine) * cellWidth;
			y = (float)(num % maxPerLine) * (0f - cellHeight);
		}
	}

	private int CalculateIndex(float x, float y)
	{
		if (arrangement == Arrangement.Horizontal)
		{
			if (maxPerLine == 0)
			{
				return Mathf.RoundToInt(x / cellWidth);
			}
			int num = Mathf.RoundToInt(y / (0f - cellHeight));
			return Mathf.RoundToInt(x / cellWidth) + num * maxPerLine;
		}
		if (maxPerLine == 0)
		{
			return Mathf.RoundToInt(y / (0f - cellHeight));
		}
		int num2 = Mathf.RoundToInt(x / cellWidth);
		return Mathf.RoundToInt(y / (0f - cellHeight)) + num2 * maxPerLine;
	}

	private void PrepareItem(GameObject itemObj, int num)
	{
		SQUtils.SetLayer(itemObj, base.gameObject.layer);
		float x = 0f;
		float y = 0f;
		CalculatePosition(num, ref x, ref y);
		itemObj.transform.localPosition = new Vector3(x, y, 0f);
		itemObj.GetComponent<Collider>().enabled = true;
		UIPanel component = itemObj.GetComponent<UIPanel>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		UIDragPanelContents component2 = itemObj.GetComponent<UIDragPanelContents>();
		if (component2 != null)
		{
			component2.draggablePanel = dragPanel;
		}
	}

	public void ResetState()
	{
		StopAllCoroutines();
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, GameObject> item in itemsShowing)
		{
			list.Add(item.Key);
		}
		foreach (int item2 in list)
		{
			PoolManager.Release(itemsShowing[item2]);
			itemsShowing.Remove(item2);
		}
		foreach (Transform item3 in base.gameObject.transform)
		{
			Object.Destroy(item3.gameObject);
		}
	}

	private void CreateDummyObjects(int maxNum)
	{
		UIPlaceHolder uIPlaceHolder = NGUITools.AddChild<UIPlaceHolder>(base.gameObject);
		uIPlaceHolder.transform.localPosition = new Vector3(0f - cellWidth / 2f, cellHeight / 2f, 0f);
		float num = 0f;
		float num2 = 0f;
		if (arrangement == Arrangement.Horizontal)
		{
			if (maxPerLine == 0)
			{
				num = (float)maxNum * cellWidth;
				num2 = cellHeight;
			}
			else
			{
				num = (float)maxPerLine * cellWidth;
				num2 = (float)(maxNum / maxPerLine + 1) * (0f - cellHeight);
			}
		}
		else if (maxPerLine == 0)
		{
			num = cellWidth;
			num2 = (float)maxNum * (0f - cellHeight);
		}
		else
		{
			num = (float)(maxNum / maxPerLine + 1) * cellWidth;
			num2 = (float)maxPerLine * (0f - cellHeight);
		}
		num -= cellWidth / 2f;
		num2 += cellHeight / 2f;
		UIPlaceHolder uIPlaceHolder2 = NGUITools.AddChild<UIPlaceHolder>(base.gameObject);
		uIPlaceHolder2.transform.localPosition = new Vector3(num, num2, 0f);
	}

	private void Update()
	{
		if (isInitialized)
		{
			Vector4 clipRange = parentPanel.clipRange;
			Vector4 vector = prevClipRange;
			if (needsUpdate || !Mathf.Approximately(clipRange.w, vector.w) || !Mathf.Approximately(clipRange.x, vector.x) || !Mathf.Approximately(clipRange.y, vector.y) || !Mathf.Approximately(clipRange.z, vector.z))
			{
				prevClipRange = parentPanel.clipRange;
				StopAllCoroutines();
				StartCoroutine(FillGrid());
			}
		}
	}

	public void Initialize(List<object> items, PrefabCallback prefabCB, FillItemCallback fillCB)
	{
		ResetState();
		itemData = items;
		numItems = itemData.Count;
		fillItem = fillCB;
		getPrefab = prefabCB;
		CreateDummyObjects(numItems);
		if (dragPanel != null)
		{
			dragPanel.ResetPosition();
			dragPanel.UpdateScrollbars(true);
		}
		needsUpdate = true;
		isInitialized = true;
	}

	public IEnumerator FillGrid()
	{
		if (!isInitialized || parentPanel == null)
		{
			yield break;
		}
		needsUpdate = false;
		Vector4 clipRange = parentPanel.clipRange;
		Vector2 size = new Vector2(clipRange.z, clipRange.w);
		if (uiAnchor != null)
		{
			if (uiAnchor.syncClippingWidth)
			{
				size.x = (size.x - uiAnchor.syncClippingWidthOffset) / uiAnchor.syncClippingWidthScale;
			}
			if (uiAnchor.syncClippingHeight)
			{
				size.y = (size.y - uiAnchor.syncClippingHeightOffset) / uiAnchor.syncClippingHeightScale;
			}
		}
		Camera mCam = NGUITools.FindCameraForLayer(base.gameObject.layer);
		if (size.x == 0f)
		{
			size.x = ((!(mCam == null)) ? mCam.pixelWidth : Screen.width);
		}
		if (size.y == 0f)
		{
			size.y = ((!(mCam == null)) ? mCam.pixelHeight : Screen.height);
		}
		size *= 0.5f;
		size.x += cellWidth;
		size.y += cellHeight;
		Vector2 mMin = new Vector2(clipRange.x - size.x, clipRange.y - size.y);
		Vector2 mMax = new Vector2(clipRange.x + size.x, clipRange.y + size.y);
		HashSet<int> visibleSet = new HashSet<int>();
		for (int j = 0; j < numItems; j++)
		{
			float x = 0f;
			float y = 0f;
			CalculatePosition(j, ref x, ref y);
			if (x > mMin.x && y > mMin.y && x < mMax.x && y < mMax.y)
			{
				visibleSet.Add(j);
			}
		}
		List<int> keyList = new List<int>();
		foreach (KeyValuePair<int, GameObject> entry in itemsShowing)
		{
			if (!visibleSet.Contains(entry.Key))
			{
				keyList.Add(entry.Key);
			}
		}
		foreach (int k in keyList)
		{
			PoolManager.Release(itemsShowing[k]);
			itemsShowing.Remove(k);
		}
		keyList.Clear();
		foreach (int i in visibleSet)
		{
			if (!itemsShowing.ContainsKey(i))
			{
				GameObject itemObj = PoolManager.FetchChild(prefab: getPrefab(itemData[i]), parent: base.gameObject);
				PrepareItem(itemObj, i);
				if (fillItem != null)
				{
					fillItem(itemObj, itemData[i]);
				}
				itemsShowing.Add(i, itemObj);
				yield return null;
			}
		}
	}
}
