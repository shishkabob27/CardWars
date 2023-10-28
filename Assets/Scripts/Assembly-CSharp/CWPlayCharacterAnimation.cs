using System.Collections;
using UnityEngine;

public class CWPlayCharacterAnimation : MonoBehaviour
{
	public int player;

	public CharAnimType animType;

	public WrapMode wrapMode;

	public float animationDelay;

	public CharAnimType nextAnimType = CharAnimType.CardIdle;

	public WrapMode nextWrapMode;

	public bool clampForever;

	private CWCharacterAnimController charAnimController;

	private void Start()
	{
		charAnimController = CWCharacterAnimController.GetInstance();
	}

	private void OnClick()
	{
		StartCoroutine(playAnim(animationDelay));
	}

	private IEnumerator playAnim(float delay)
	{
		yield return new WaitForSeconds(delay);
		charAnimController.playAnim(player, animType, wrapMode, nextAnimType, nextWrapMode, clampForever);
	}

	private void Update()
	{
	}
}
