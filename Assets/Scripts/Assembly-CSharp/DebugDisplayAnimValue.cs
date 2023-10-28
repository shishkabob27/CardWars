using UnityEngine;

public class DebugDisplayAnimValue : MonoBehaviour
{
	public int player;

	public UILabel animName;

	public UILabel currentTime;

	public UILabel animLength;

	public UILabel faceAnimName;

	public UILabel faceCurrentTime;

	private GameObject character;

	public UILabel characterName;

	public Animation anim;

	private CWCharacterAnimController animController;

	private bool initialized;

	private void Start()
	{
		animController = CWCharacterAnimController.GetInstance();
	}

	private void Refresh()
	{
		bool flag = false;
		foreach (AnimationState item in anim)
		{
			if (anim.IsPlaying(item.name))
			{
				SetDisplayValue(item);
				flag = true;
			}
		}
		if (!flag)
		{
			SetDisplayValue(null);
		}
	}

	private void SetDisplayValue(AnimationState ani)
	{
		animName.text = ((!(ani != null)) ? string.Empty : ani.name);
		float num = ((!(ani != null)) ? 0f : (Mathf.Round(ani.time * 100f) / 100f));
		currentTime.text = num.ToString();
		animLength.text = ((!(ani != null)) ? string.Empty : ani.length.ToString());
		characterName.text = character.name;
		faceAnimName.text = ((player != 0) ? animController.P2FaceAnim : animController.P1FaceAnim);
		faceCurrentTime.text = ((player != 0) ? animController.P2FaceTime.ToString() : animController.P1FaceTime.ToString());
	}

	private void Update()
	{
		SessionManager instance = SessionManager.GetInstance();
		if (instance.IsReady())
		{
			initialized = true;
		}
		if (initialized)
		{
			if (animController == null)
			{
				animController = CWCharacterAnimController.GetInstance();
			}
			character = animController.playerCharacters[player];
			anim = character.GetComponent<Animation>();
			if (character != null)
			{
				Refresh();
			}
		}
	}
}
