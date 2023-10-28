using UnityEngine;

public class EmitParticles : MonoBehaviour
{
	public ParticleEmitter emitter;

	public Transform left;

	public Transform right;

	public Transform up;

	public Transform down;

	public void Emit()
	{
		emitter.Emit();
	}

	public void Emit(Vector3 dir)
	{
		Emit(Quaternion.LookRotation(dir));
	}

	public void Emit(Quaternion rot)
	{
		emitter.transform.rotation = rot;
		Emit();
	}

	public void EmitLeft()
	{
		Emit(left.rotation);
	}

	public void EmitRight()
	{
		Emit(right.rotation);
	}

	public void EmitUp()
	{
		Emit(up.rotation);
	}

	public void EmitDown()
	{
		Emit(down.rotation);
	}
}
