using UnityEngine;

public abstract class FGInputProvider : MonoBehaviour
{
	public abstract int MaxSimultaneousFingers { get; }

	public abstract void GetInputState(int fingerIndex, out bool down, out Vector2 position);
}
