using UnityEngine;
using System.Collections.Generic;

public class PointCloudGestureTemplate : ScriptableObject
{
	[SerializeField]
	private List<int> strokeIds;
	[SerializeField]
	private List<Vector2> positions;
	[SerializeField]
	private int strokeCount;
	[SerializeField]
	private Vector2 size;
}
