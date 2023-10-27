using UnityEngine;
using System.Collections.Generic;

public class CWCharacterAnimController : MonoBehaviour
{
	public List<GameObject> playerCharacters;
	public List<GameObject> holdHandBones;
	public List<GameObject> playHandBones;
	public List<CWPlayCardsController> playCardControllers;
	public float cardReleaseTimePlayCard;
	public float cardReleaseTimeRareCard;
	public string playerID;
	public string opponentID;
	public string P1FaceAnim;
	public string P2FaceAnim;
	public float P1FaceTime;
	public float P2FaceTime;
	public GameObject dweebCup;
}
