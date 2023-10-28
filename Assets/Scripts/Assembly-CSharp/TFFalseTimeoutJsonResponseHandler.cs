using System;
using System.Collections.Generic;
using System.Net;

public class TFFalseTimeoutJsonResponseHandler
{
	public delegate void RequestExecutor();

	private const int RETRIES_MAX = 2;

	private const long FALSETIMEOUT_DELAYTICKS_MAX = 100000000L;

	private RequestExecutor reqExecutor;

	private TFServer.JsonResponseHandler cb;

	private TFServer.JsonResponseHandler retryHandler;

	private long lastExecutionTick;

	private int retries;

	public TFFalseTimeoutJsonResponseHandler(TFServer.JsonResponseHandler inCb)
	{
		cb = inCb;
	}

	public void ExecuteRequest(RequestExecutor inReqExecutor)
	{
		reqExecutor = inReqExecutor;
		lastExecutionTick = DateTime.Now.Ticks;
		reqExecutor();
	}

	public TFServer.JsonResponseHandler ToHandler()
	{
		if (retryHandler == null)
		{
			retryHandler = delegate(Dictionary<string, object> result, HttpStatusCode status)
			{
				if (!TryFalseTimeoutRecover(status))
				{
					cb(result, status);
				}
			};
		}
		return retryHandler;
	}

	private bool TryFalseTimeoutRecover(HttpStatusCode status)
	{
		if (status != HttpStatusCode.ServiceUnavailable)
		{
			return false;
		}
		long num = DateTime.Now.Ticks - lastExecutionTick;
		if (num > 100000000)
		{
			return false;
		}
		if (retries >= 2)
		{
			return false;
		}
		retries++;
		TFUtils.WarnLog("Detected false timeout. Retrying (count = " + retries + ")");
		ExecuteRequest(reqExecutor);
		return true;
	}
}
