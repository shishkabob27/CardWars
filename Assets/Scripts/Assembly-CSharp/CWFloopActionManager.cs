using UnityEngine;

public class CWFloopActionManager : MonoBehaviour
{
	public int player;
	public int lane;
	public GameObject numberDisplayObj;
	public Animation anim;
	public AudioClip floopSound;
	public GameObject floopActionCamTarget;
	public GameObject floopActionCamLookAtTarget;
	public UIButtonTween opponentFloopPanelTween;
	public GameObject[] heroTargetObjeccts;
	public GameObject[] magicTargetObjects;
	public GameObject[] costTargetObjects;
	public GameObject spawnFXCameraCenter;
	public float heroFxWaitTime1;
	public float heroFxWaitTime2;
	public GameObject floopFX;
	public float detailCameraOffsetX;
	public float detailCameraOffsetY;
	public float floopCameraOffsetX;
	public float floopCameraOffsetY;
	public float floopCameraTargetOffsetY;
	public GameObject floopButtonFXIdle;
	public GameObject floopButtonFXAction;
	public GameObject floopPanel;
	public GameObject spellPanel;
	public Transform[] PlayerNeutralPoints;
	public Transform[] OpponentNeutralPoints;
	public Camera BattleCamera;
	public Transform BattleCameraTarget;
	public CWFloopPrompt floopPrompt;
	public Transform[] PlayerStatusEffects;
	public Transform[] OpponentStatusEffects;
	public GameObject DeckTarget;
	public bool usingSpellCamera;
	public Camera SpellCamera;
	public Transform BannerEffect;
}
