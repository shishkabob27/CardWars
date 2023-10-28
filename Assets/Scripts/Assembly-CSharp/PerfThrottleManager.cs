using System.Collections.Generic;
using UnityEngine;

public class PerfThrottleManager : MonoBehaviour
{
	public enum PerfEvents
	{
		NONE,
		BATTLE_SEQUENCE,
		BATTLE_LANE1,
		BATTLE_LANE2,
		BATTLE_LANE3,
		BATTLE_LANE4
	}

	private class EventData
	{
		public List<GameObject> objectList;

		public List<Renderer> renderableList;

		public List<UIPanel> uiPanelsList;

		public List<Camera> cameraList;

		public void Clear()
		{
			if (objectList != null)
			{
				objectList.Clear();
			}
			if (renderableList != null)
			{
				renderableList.Clear();
			}
			if (uiPanelsList != null)
			{
				uiPanelsList.Clear();
			}
			if (cameraList != null)
			{
				cameraList.Clear();
			}
		}
	}

	private static Dictionary<PerfEvents, EventData> allEventsData = new Dictionary<PerfEvents, EventData>();

	public static void HandlePerfThrottleEvent(PerfEvents perfEvent, bool enableThings)
	{
		if (!KFFLODManager.IsLowEndDevice())
		{
			return;
		}
		PerfThrottleParams[] array = null;
		EventData value;
		if (!enableThings)
		{
			array = Object.FindObjectsOfType<PerfThrottleParams>();
			if (array == null)
			{
				return;
			}
			if (!allEventsData.TryGetValue(perfEvent, out value))
			{
				value = new EventData();
				allEventsData.Add(perfEvent, value);
			}
			else
			{
				value.Clear();
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].perfEvent != perfEvent)
				{
					continue;
				}
				GameObject gameObject = array[i].gameObject;
				if (array[i].disableObject)
				{
					if (value.objectList == null)
					{
						value.objectList = new List<GameObject>();
					}
					value.objectList.Add(gameObject);
				}
				if (array[i].disableRenderers)
				{
					Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
					if (componentsInChildren != null && componentsInChildren.Length > 0)
					{
						if (value.renderableList == null)
						{
							value.renderableList = new List<Renderer>();
						}
						value.renderableList.AddRange(componentsInChildren);
					}
				}
				if (array[i].disableUIPanels)
				{
					UIPanel[] componentsInChildren2 = gameObject.GetComponentsInChildren<UIPanel>();
					if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
					{
						if (value.uiPanelsList == null)
						{
							value.uiPanelsList = new List<UIPanel>();
						}
						value.uiPanelsList.AddRange(componentsInChildren2);
					}
				}
				if (!array[i].disableCamera)
				{
					continue;
				}
				Camera component = gameObject.GetComponent<Camera>();
				if (component != null)
				{
					if (value.cameraList == null)
					{
						value.cameraList = new List<Camera>();
					}
					value.cameraList.Add(component);
				}
			}
			EnableObjectsAndComponents(value, false);
		}
		else if (allEventsData.TryGetValue(perfEvent, out value))
		{
			EnableObjectsAndComponents(value, true);
		}
	}

	private static void EnableObjectsAndComponents(EventData eventData, bool enable)
	{
		if (eventData.objectList != null)
		{
			foreach (GameObject @object in eventData.objectList)
			{
				@object.SetActive(enable);
			}
		}
		if (eventData.renderableList != null)
		{
			foreach (Renderer renderable in eventData.renderableList)
			{
				if ((bool)renderable)
				{
					renderable.enabled = enable;
				}
			}
		}
		if (eventData.uiPanelsList != null)
		{
			foreach (UIPanel uiPanels in eventData.uiPanelsList)
			{
				if ((bool)uiPanels)
				{
					uiPanels.enabled = enable;
				}
			}
		}
		if (eventData.cameraList == null)
		{
			return;
		}
		foreach (Camera camera in eventData.cameraList)
		{
			if ((bool)camera)
			{
				camera.enabled = enable;
			}
		}
	}
}
