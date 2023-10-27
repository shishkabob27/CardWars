using UnityEngine;

public class CWPlayerHandsController : MonoBehaviour
{
	public BoxCollider[] laneColliders;
	public BoxCollider[] oppLaneColliders;
	public GameObject[] playerHands;
	public GameObject replaceWindow;
	public GameObject tweenZoom;
	public GameObject spellParticleEffect;
	public AudioClip errorSound;
	public AudioClip cardMoveSound;
	public AudioClip cardZoomSound;
	public AudioClip cardUnzoomSound;
	public AudioClip cardPickSound;
	public AudioClip cardDragSound;
	public AudioClip[] cardPlaceSounds;
	public AudioClip spellSound;
	public AudioClip cardDrawSound;
	public bool spinStart;
	public GameObject replaceTweenTarget;
	public int lane;
	public string cardName;
	public GameObject endOfSpinningCardFX;
	public Vector3 endOfSpinningCardFXOffset;
}
