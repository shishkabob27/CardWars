public class NcRepeatTimerTool : NcTimerTool
{
	protected float m_fUpdateTime;

	protected float m_fIntervalTime;

	protected int m_nRepeatCount;

	protected int m_nCallCount;

	protected object m_ArgObject;

	public bool UpdateTimer()
	{
		if (!m_bEnable)
		{
			return false;
		}
		bool flag = m_fUpdateTime <= GetTime();
		if (flag)
		{
			m_fUpdateTime += m_fIntervalTime;
			m_nCallCount++;
			if (m_nRepeatCount == 0 || (m_nRepeatCount != 0 && m_nRepeatCount <= m_nCallCount))
			{
				m_bEnable = false;
			}
		}
		return flag;
	}

	public void ResetUpdateTime()
	{
		m_fUpdateTime = GetTime() + m_fIntervalTime;
	}

	public int GetCallCount()
	{
		return m_nCallCount;
	}

	public object GetArgObject()
	{
		return m_ArgObject;
	}

	public float GetElapsedRate()
	{
		if (m_fUpdateTime == 0f)
		{
			return 0f;
		}
		return GetTime() / m_fUpdateTime;
	}

	public void SetTimer(float fStartTime)
	{
		SetRelTimer(fStartTime - NcTimerTool.GetEngineTime());
	}

	public void SetTimer(float fStartTime, float fRepeatTime)
	{
		SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRepeatTime);
	}

	public void SetTimer(float fStartTime, float fRepeatTime, int nRepeatCount)
	{
		SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRepeatTime, nRepeatCount);
	}

	public void SetTimer(float fStartTime, object arg)
	{
		SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), arg);
	}

	public void SetTimer(float fStartTime, float fRepeatTime, object arg)
	{
		SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRepeatTime, arg);
	}

	public void SetTimer(float fStartTime, float fRepeatTime, int nRepeatCount, object arg)
	{
		SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRepeatTime, nRepeatCount, arg);
	}

	public void SetRelTimer(float fStartRelTime)
	{
		Start();
		m_nCallCount = 0;
		m_fUpdateTime = fStartRelTime;
		m_fIntervalTime = 0f;
		m_nRepeatCount = 0;
	}

	public void SetRelTimer(float fStartRelTime, float fRepeatTime)
	{
		Start();
		m_nCallCount = 0;
		m_fUpdateTime = fStartRelTime;
		m_fIntervalTime = fRepeatTime;
		m_nRepeatCount = 0;
	}

	public void SetRelTimer(float fStartRelTime, float fRepeatTime, int nRepeatCount)
	{
		Start();
		m_nCallCount = 0;
		m_fUpdateTime = fStartRelTime;
		m_fIntervalTime = fRepeatTime;
		m_nRepeatCount = nRepeatCount;
	}

	public void SetRelTimer(float fStartRelTime, object arg)
	{
		Start();
		m_nCallCount = 0;
		m_fUpdateTime = fStartRelTime;
		m_fIntervalTime = 0f;
		m_nRepeatCount = 0;
		m_ArgObject = arg;
	}

	public void SetRelTimer(float fStartRelTime, float fRepeatTime, object arg)
	{
		Start();
		m_nCallCount = 0;
		m_fUpdateTime = fStartRelTime;
		m_fIntervalTime = fRepeatTime;
		m_nRepeatCount = 0;
		m_ArgObject = arg;
	}

	public void SetRelTimer(float fStartRelTime, float fRepeatTime, int nRepeatCount, object arg)
	{
		Start();
		m_nCallCount = 0;
		m_fUpdateTime = fStartRelTime;
		m_fIntervalTime = fRepeatTime;
		m_nRepeatCount = nRepeatCount;
		m_ArgObject = arg;
	}
}
