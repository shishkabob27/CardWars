using System.Collections.Generic;
using UnityEngine;

public class iTweenPath : MonoBehaviour
{
	public string pathName = string.Empty;

	public Color pathColor = Color.cyan;

	public List<Vector3> nodes = new List<Vector3>
	{
		Vector3.zero,
		Vector3.zero
	};

	public int nodeCount;

	public static Dictionary<string, iTweenPath> paths = new Dictionary<string, iTweenPath>();

	public bool initialized;

	public string initialName = string.Empty;

	private void OnEnable()
	{
		if (!paths.ContainsKey(pathName))
		{
			paths.Add(pathName.ToLower(), this);
		}
	}

	private void OnDisable()
	{
		paths.Remove(pathName.ToLower());
	}

	private void OnDrawGizmosSelected()
	{
		if (base.enabled && nodes.Count > 0)
		{
			iTween.DrawPath(nodes.ToArray(), pathColor);
		}
	}

	public static Vector3[] GetPath(string requestedName)
	{
		requestedName = requestedName.ToLower();
		if (paths.ContainsKey(requestedName))
		{
			return paths[requestedName].nodes.ToArray();
		}
		return null;
	}
}
