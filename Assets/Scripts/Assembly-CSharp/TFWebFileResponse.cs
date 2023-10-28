using System.Collections.Generic;
using System.Net;
using MiniJSON;

public class TFWebFileResponse
{
	public HttpStatusCode StatusCode;

	public string Data;

	public string URI;

	public object UserData;

	public WebHeaderCollection headers;

	public bool NetworkDown;

	public Dictionary<string, object> GetAsJSONDict()
	{
		if (Data != null)
		{
			return (Dictionary<string, object>)Json.Deserialize(Data);
		}
		return null;
	}
}
