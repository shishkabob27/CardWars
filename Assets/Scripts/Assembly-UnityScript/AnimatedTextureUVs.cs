using System;
using UnityEngine;

[Serializable]
public class AnimatedTextureUVs : MonoBehaviour
{
	public int uvAnimationTileX;

	public int uvAnimationTileY;

	public float framesPerSecond;

	public bool loop;

	public bool finished;

	private int numFrames;

	public AnimatedTextureUVs()
	{
		uvAnimationTileX = 3;
		uvAnimationTileY = 3;
		framesPerSecond = 10f;
	}

	public virtual void Start()
	{
		numFrames = uvAnimationTileX * uvAnimationTileY;
	}

	public virtual void Update()
	{
		if (!finished)
		{
			int num = (int)(Time.time * framesPerSecond);
			int num2 = num;
			num %= numFrames;
			Vector2 scale = new Vector2(1f / (float)uvAnimationTileX, 1f / (float)uvAnimationTileY);
			int num3 = num % uvAnimationTileX;
			int num4 = num / uvAnimationTileX;
			Vector2 offset = new Vector2((float)num3 * scale.x, 1f - scale.y - (float)num4 * scale.y);
			GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
			GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
			if (!loop && num2 == numFrames - 1)
			{
				finished = true;
			}
		}
	}

	public virtual void Main()
	{
	}
}
