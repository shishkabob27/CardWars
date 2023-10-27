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
}
