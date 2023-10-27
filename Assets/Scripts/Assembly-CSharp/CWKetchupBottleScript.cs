using UnityEngine;
using System;

public class CWKetchupBottleScript : MonoBehaviour
{
	[Serializable]
	public class Spinner
	{
		public Transform spinner;
		public Transform shadow;
	}

	public UILabel whoGoesFirstLabel;
	public Spinner[] normalSpinners;
	public Spinner[] fcSpinners;
	public GameObject battleReadyButton;
	public bool Spin;
	public float SpinSpeed;
	public GameObject fingerCollider;
	public Camera GameCamera;
	public GameObject spinButton;
	public GameObject tweenController;
	public GameObject bottleSpinLabel;
	public int clickCount;
}
