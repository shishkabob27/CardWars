using System.Collections.Generic;
using System.Net;

public class TFServer
{
	public delegate void JsonResponseHandler(Dictionary<string, object> dict,HttpStatusCode status);

	public TFServer(CookieContainer cookies, int maxConnections)
	{
	}

}
