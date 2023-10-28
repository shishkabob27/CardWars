using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NisLaunchHelper))]
public class CWBattleIntroController : MonoBehaviour
{
	private static CWBattleIntroController g_controller;

	public NisAsyncPlayer nisPlayerPrefab;

	public UIButtonPlayAnimation CameraAnim;

	public Animation CharacterP1;

	public Animation CharacterP2;

	public GameObject ContinueButton;

	public CWMenuCameraTarget CameraTarget;

	private bool initialized;

	private bool charsLoaded;

	public UILabel NameP1;

	public UILabel NameP2;

	public UISprite bgP1;

	public UISprite bgP2;

	public UILabel Arena;

	public BattleJukeboxScript jukebox;

	public GameObject Logos;

	public AnimationClip[] introCameraClips;

	public GameObject[] tweenIn;

	public GameObject[] tweenOut;

	public float[] tweenInDelay;

	private int count;

	private bool tweenStartedFlag;

	private bool animStarted;

	private float nameBGPadding = 100f;

	private void Start()
	{
		g_controller = this;
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		if (!(Logos != null) || activeQuest == null || string.IsNullOrEmpty(activeQuest.LoadingScreenTextureName))
		{
			return;
		}
		UITexture componentInChildren = Logos.GetComponentInChildren<UITexture>();
		if (componentInChildren != null)
		{
			Texture texture = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(activeQuest.LoadingScreenTextureName) as Texture;
			if (texture != null)
			{
				componentInChildren.mainTexture = texture;
			}
		}
	}

	public static CWBattleIntroController GetInstance()
	{
		return g_controller;
	}

	private void LaunchBattleIntro()
	{
		if (GlobalFlags.Instance.InMPMode)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_ARENA);
		}
		LeaderItem leader = GameState.Instance.GetLeader(PlayerType.User);
		LeaderItem leader2 = GameState.Instance.GetLeader(PlayerType.Opponent);
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		if (NameP1 != null && leader != null && bgP1 != null)
		{
			if (GlobalFlags.Instance.InMPMode)
			{
				NameP1.text = PlayerInfoScript.GetInstance().MPPlayerName;
			}
			else
			{
				NameP1.text = leader.Form.Name.ToUpper();
			}
			bgP1.transform.localScale = new Vector3(NameP1.relativeSize.x * NameP1.transform.localScale.x + nameBGPadding, bgP1.transform.localScale.y, bgP1.transform.localScale.z);
		}
		if (NameP2 != null && leader2 != null && bgP2 != null)
		{
			if (GlobalFlags.Instance.InMPMode)
			{
				NameP2.text = PlayerInfoScript.GetInstance().MPOpponentName;
			}
			else
			{
				NameP2.text = leader2.Form.Name.ToUpper();
			}
			bgP2.transform.localScale = new Vector3(NameP2.relativeSize.x * NameP2.transform.localScale.x + nameBGPadding, bgP2.transform.localScale.y, bgP2.transform.localScale.z);
		}
		if (Arena != null && activeQuest != null)
		{
			Arena.text = activeQuest.LevelName;
		}
		if (CameraTarget != null)
		{
			CameraTarget.followFlag = false;
		}
		StartCoroutine(CoroutineIntro());
	}

	private IEnumerator CoroutineIntro()
	{
		yield return StartCoroutine(TriggerPreBattleNis());
		yield return StartCoroutine(TriggerBattleIntro());
	}

	private IEnumerator TriggerPreBattleNis()
	{
		QuestData qData = GameState.Instance.ActiveQuest;
		if (qData.NisPreBattle != null && (qData.NisPlayAlways || PlayerInfoScript.GetInstance().GetQuestProgress(qData) <= 0))
		{
			bool nisComplete = false;
			NisLaunchHelper nisLauncher = GetComponent<NisLaunchHelper>();
			nisLauncher.OnceLoaded(OnIntroReady);
			nisLauncher.OnceComplete(delegate
			{
				nisComplete = true;
			});
			nisLauncher.LaunchNis(qData.NisPreBattle);
			while (!nisComplete)
			{
				yield return null;
			}
		}
	}

	private IEnumerator TriggerBattleIntro()
	{
		OnIntroReady();
		CWCharacterAnimController animCtrl = CWCharacterAnimController.GetInstance();
		animCtrl.playAnim(0, CharAnimType.IntroP1, WrapMode.Once, CharAnimType.CardIdle, WrapMode.Loop, true);
		animCtrl.playAnim(1, CharAnimType.IntroP2, WrapMode.Once, CharAnimType.CardIdle, WrapMode.Loop, true);
		if (jukebox != null)
		{
			jukebox.PlayBattleIntro();
		}
		Camera gameCamera = PanelManagerBattle.GetInstance().newCamera.GetComponent<Camera>();
		Animation anim = gameCamera.GetComponent<Animation>();
		AnimationClip[] array = introCameraClips;
		foreach (AnimationClip clip in array)
		{
			anim.Play(clip.name);
			tweenStartedFlag = false;
			while (anim.isPlaying)
			{
				yield return 0;
				if (!tweenStartedFlag)
				{
					StartCoroutine(TriggerBannerTween(count, anim[clip.name].length));
					tweenStartedFlag = true;
				}
			}
			count++;
		}
		yield return null;
		FinishIntro();
	}

	private IEnumerator TriggerBannerTween(int count, float clipLength)
	{
		float delay = tweenInDelay[count];
		yield return new WaitForSeconds(delay);
		tweenIn[count].SendMessage("OnClick");
		yield return new WaitForSeconds(clipLength - delay - 0.3f);
		tweenOut[count].SendMessage("OnClick");
	}

	public void FlagCharactersLoaded()
	{
		charsLoaded = true;
	}

	public void FinishIntro()
	{
		if (CameraTarget != null)
		{
			CameraTarget.followFlag = true;
		}
		if (ContinueButton != null)
		{
			ContinueButton.SendMessage("OnClick");
		}
		if (jukebox != null)
		{
			jukebox.Refresh();
		}
	}

	public void Update()
	{
		if (animStarted)
		{
			return;
		}
		if (initialized && charsLoaded)
		{
			LaunchBattleIntro();
			animStarted = true;
			return;
		}
		SessionManager instance = SessionManager.GetInstance();
		if (instance.IsReady())
		{
			initialized = true;
		}
	}

	private void OnIntroReady(NisLaunchHelper nisLauncher = null)
	{
		if (Logos != null)
		{
			Logos.SetActive(false);
			Object.Destroy(Logos);
			Logos = null;
		}
	}
}
