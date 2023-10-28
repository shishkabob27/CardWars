using System.Collections;
using UnityEngine;

public class UVScroll : MonoBehaviour
{
	public float Xspeed = 0.8f;

	public float Yspeed = 0.8f;

	public Vector2 CycleRangeSeconds;

	private UITexture texture;

	private float RepeatTimer;

	private void OnEnable()
	{
		if (texture == null)
		{
			texture = GetComponent<UITexture>();
		}
		texture.alpha = 0f;
	}

	private void Update()
	{
		if (RepeatTimer <= 0f)
		{
			StartCoroutine(Glimer());
		}
		else
		{
			RepeatTimer -= Time.deltaTime;
		}
	}

	private IEnumerator Glimer()
	{
		Rect currentUV = texture.uvRect;
		currentUV.x = 0f;
		currentUV.y = 0.5f;
		while (currentUV.x <= 1f)
		{
			currentUV.x += Time.deltaTime * Xspeed;
			currentUV.y -= Time.deltaTime * Yspeed;
			texture.uvRect = currentUV;
			if (currentUV.x < 0.5f)
			{
				texture.alpha = currentUV.x * 2f;
			}
			else
			{
				texture.alpha = (1f - currentUV.x) * 2f;
			}
			Mathf.Clamp(texture.alpha, 0f, 1f);
			yield return null;
		}
		currentUV.x = 0f;
		currentUV.y = 0.5f;
		texture.uvRect = currentUV;
		texture.alpha = 0f;
		RepeatTimer = Random.Range(CycleRangeSeconds.x, CycleRangeSeconds.y);
	}
}
