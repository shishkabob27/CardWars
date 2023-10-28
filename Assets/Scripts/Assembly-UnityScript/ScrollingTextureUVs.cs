using System;
using UnityEngine;

[Serializable]
public class ScrollingTextureUVs : MonoBehaviour
{
	public float animationRate;

	public float animationRate_y;

	public Vector2 offset;

	public int direction;

	public int direction_y;

	public ScrollingTextureUVs()
	{
		animationRate = 1f;
		direction = 1;
		direction_y = 1;
	}

	public virtual void Start()
	{
		offset = Vector2.zero;
	}

	public virtual void LateUpdate()
	{
		Vector2 vector = new Vector2((float)direction * animationRate, (float)direction_y * animationRate_y);
		offset += vector * Time.deltaTime;
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
	}

	public virtual void Main()
	{
	}
}
