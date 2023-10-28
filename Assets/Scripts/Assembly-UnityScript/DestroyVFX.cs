using System;
using UnityEngine;

[Serializable]
public class DestroyVFX : MonoBehaviour
{
	public float destroyTimer;

	public DestroyVFX()
	{
		destroyTimer = 1f;
	}

	public virtual void Start()
	{
	}

	public virtual void Update()
	{
		UnityEngine.Object.Destroy(gameObject, destroyTimer);
	}

	public virtual void Main()
	{
	}
}
