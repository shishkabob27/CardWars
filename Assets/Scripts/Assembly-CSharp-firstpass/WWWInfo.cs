using System;
using System.Collections.Generic;
using UnityEngine;

public class WWWInfo
{
	public delegate void RequestCallback(WWWInfo info, object obj, string err, object param);

	[NonSerialized]
	public WWW www;

	[NonSerialized]
	public RequestCallback callback;

	[NonSerialized]
	public object callbackParam;

	[NonSerialized]
	public string scriptNameAndParams;

	[NonSerialized]
	public WWWForm form;

	[NonSerialized]
	public bool queued;

	[NonSerialized]
	public bool active;

	[NonSerialized]
	public int version;

	[NonSerialized]
	public bool isHttps;

	[NonSerialized]
	public bool isHttpsDone;

	[NonSerialized]
	public string httpsResult;

	[NonSerialized]
	public Dictionary<string, string> postData;

	public WWWInfo()
	{
		version = -1;
	}
}
