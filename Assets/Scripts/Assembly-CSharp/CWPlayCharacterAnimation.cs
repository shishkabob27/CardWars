using UnityEngine;

public class CWPlayCharacterAnimation : MonoBehaviour
{
	public int player;
	public CharAnimType animType;
	public WrapMode wrapMode;
	public float animationDelay;
	public CharAnimType nextAnimType;
	public WrapMode nextWrapMode;
	public bool clampForever;
}
