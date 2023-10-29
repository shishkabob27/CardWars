using System;
using System.Collections;
using UnityEngine;

public class SLOTResourceManager : SLOTGameSingleton<SLOTResourceManager>
{
	private class AsyncReq : SLOTResoureRequest
	{
		private Coroutine _asyncOp;

		public UnityEngine.Object asset { get; private set; }

		public bool isDone { get; private set; }

		public Coroutine asyncOp
		{
			get
			{
				return _asyncOp;
			}
			set
			{
				if (_asyncOp != null)
				{
					throw new AccessViolationException("Trying to set the async operation variable more than once!");
				}
				_asyncOp = value;
			}
		}

		public void SetComplete(UnityEngine.Object inAsset)
		{
			asset = inAsset;
			isDone = true;
		}
	}

	public bool useLocalResources;

	private AssetBundle assetBundle;

	private static string GetResourceName(string name)
	{
		if (SLOTGame.IsLowEndDevice())
		{
			int num = name.LastIndexOf("/");
			if (num >= 0)
			{
				string text = name.Substring(0, num);
				string text2 = name.Substring(num + 1);
				return text + "/low_" + text2;
			}
			return "low_" + name;
		}
		return name;
	}

	public bool SetAssetBundle(AssetBundle bundle)
	{
		if (bundle != null && !useLocalResources)
		{
			assetBundle = bundle;
			return true;
		}
		return false;
	}

	public UnityEngine.Object LoadResource(string path)
	{
		UnityEngine.Object @object;
		if (assetBundle != null && !useLocalResources)
		{
			@object = assetBundle.LoadAsset(GetResourceName(path));
			if (@object != null)
			{
				return @object;
			}
		}
		@object = Resources.Load(GetResourceName(path));
		if (@object != null)
		{
			return @object;
		}
		if (SLOTGame.IsLowEndDevice())
		{
			return Resources.Load(path);
		}
		return null;
	}

	public UnityEngine.Object LoadResource(string path, Type t)
	{
		UnityEngine.Object @object;
		if (assetBundle != null && !useLocalResources)
		{
			@object = assetBundle.LoadAsset(GetResourceName(path), t);
			if (@object != null)
			{
				return @object;
			}
		}
		@object = Resources.Load(GetResourceName(path), t);
		if (@object != null)
		{
			return @object;
		}
		if (SLOTGame.IsLowEndDevice())
		{
			return Resources.Load(path, t);
		}
		return null;
	}

	public SLOTResoureRequest LoadResourceAsync(string path)
	{
		AsyncReq asyncReq = new AsyncReq();
		asyncReq.asyncOp = StartCoroutine(CoroutineLoadResourceAsync(asyncReq, path));
		return asyncReq;
	}

	public SLOTResoureRequest LoadResourceAsync(string path, Type t)
	{
		AsyncReq asyncReq = new AsyncReq();
		asyncReq.asyncOp = StartCoroutine(CoroutineLoadResourceAsync(asyncReq, path, t));
		return asyncReq;
	}

	private void OnDestroy()
	{
		if (assetBundle != null)
		{
			assetBundle.Unload(true);
			assetBundle = null;
		}
	}

	private IEnumerator CoroutineLoadResourceAsync(AsyncReq asyncReq, string path, Type t = null)
	{
		if (assetBundle != null && !useLocalResources)
		{
			AssetBundleRequest req3 = assetBundle.LoadAssetAsync(GetResourceName(path), t);
			yield return req3;
			if (req3.asset != null)
			{
				asyncReq.SetComplete(req3.asset);
				yield break;
			}
		}
		ResourceRequest req2 = Resources.LoadAsync(GetResourceName(path), t);
		yield return req2;
		if (req2.asset != null)
		{
			asyncReq.SetComplete(req2.asset);
			yield break;
		}
		if (SLOTGame.IsLowEndDevice())
		{
			ResourceRequest req = Resources.LoadAsync(path, t);
			yield return req;
			if (req.asset != null)
			{
				asyncReq.SetComplete(req.asset);
				yield break;
			}
		}
		asyncReq.SetComplete(null);
	}
}
