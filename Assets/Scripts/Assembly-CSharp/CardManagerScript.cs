using UnityEngine;
using System.Collections.Generic;

public class CardManagerScript : MonoBehaviour
{
	public bool CardSelected;
	public int CardsDiscarded;
	public bool Discarding;
	public bool GnomeSnot;
	public bool UpdateNow;
	public List<int> ChosenCardIDs;
	public List<int> ChosenCardTypes;
	public GameObject[] CardObjects;
}
