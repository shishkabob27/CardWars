using System;

public class NcTickTimerTool
{
	protected int m_nStartTickCount;

	protected int m_nCheckTickCount;

	public NcTickTimerTool()
	{
		StartTickCount();
	}

	public static NcTickTimerTool GetTickTimer()
	{
		return new NcTickTimerTool();
	}

	public void StartTickCount()
	{
		m_nStartTickCount = Environment.TickCount;
		m_nCheckTickCount = m_nStartTickCount;
	}

	public int GetStartedTickCount()
	{
		return Environment.TickCount - m_nStartTickCount;
	}

	public int GetElapsedTickCount()
	{
		int result = Environment.TickCount - m_nCheckTickCount;
		m_nCheckTickCount = Environment.TickCount;
		return result;
	}

	public void LogElapsedTickCount()
	{
	}
}
