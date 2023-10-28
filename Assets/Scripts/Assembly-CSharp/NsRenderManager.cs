using System.Collections.Generic;
using UnityEngine;

public class NsRenderManager : MonoBehaviour
{
	public List<Component> m_RenderEventCalls;

	private void Awake()
	{
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Start()
	{
	}

	private void OnPreRender()
	{
		if (m_RenderEventCalls == null)
		{
			return;
		}
		int num = m_RenderEventCalls.Count - 1;
		while (0 <= num)
		{
			if (m_RenderEventCalls[num] == null)
			{
				m_RenderEventCalls.RemoveAt(num);
			}
			else
			{
				m_RenderEventCalls[num].SendMessage("OnPreRender");
			}
			num--;
		}
	}

	private void OnRenderObject()
	{
	}

	private void OnPostRender()
	{
		if (m_RenderEventCalls == null)
		{
			return;
		}
		foreach (Component renderEventCall in m_RenderEventCalls)
		{
			if (renderEventCall != null)
			{
				renderEventCall.SendMessage("OnPostRender");
			}
		}
	}

	public void AddRenderEventCall(Component tarCom)
	{
		if (m_RenderEventCalls == null)
		{
			m_RenderEventCalls = new List<Component>();
		}
		if (!m_RenderEventCalls.Contains(tarCom))
		{
			m_RenderEventCalls.Add(tarCom);
		}
	}

	public void RemoveRenderEventCall(Component tarCom)
	{
		if (m_RenderEventCalls == null)
		{
			m_RenderEventCalls = new List<Component>();
		}
		if (m_RenderEventCalls.Contains(tarCom))
		{
			m_RenderEventCalls.Remove(tarCom);
		}
	}
}
