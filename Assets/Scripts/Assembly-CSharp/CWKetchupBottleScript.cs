using System;
using UnityEngine;

public class CWKetchupBottleScript : MonoBehaviour
{
	[Serializable]
	public class Spinner
	{
		public Transform spinner;

		public Transform shadow;
	}

	private const string FIONNACAKE_QUESTTYPE_PREFIX = "fc";

	private static int INVALID_DRAG_TOUCH_ID = -12345;

	private static float MIN_ANGULAR_VELOCITY = 0.7f;

	private static float FORCE_ANGULAR_VELOCITY = 2f;

	private static float ANGULAR_VELOCITY_SCALE = 10f;

	private static float ANGULAR_RANDOMNESS = 1f;

	private static float STOP_ANGULAR_VELOCITY = 0.1f;

	private static float SPIN_ANGLE_SCALE = 0.75f;

	private static float DRAG_DISTANCE = 5f;

	public UILabel whoGoesFirstLabel;

	public Spinner[] normalSpinners;

	public Spinner[] fcSpinners;

	public GameObject battleReadyButton;

	public bool Spin;

	public float SpinSpeed;

	public GameObject fingerCollider;

	public Camera GameCamera;

	public GameObject spinButton;

	public static DebugFlagsScript.ForceStartingPlayer forceStartingPlayer;

	private GameDataScript GameData;

	private GameState GameInstance;

	public GameObject tweenController;

	public GameObject bottleSpinLabel;

	private FingerGestures.Finger activeFinger;

	private Vector2 dragBeginPos;

	private bool dragging;

	private Plane tablePlane = new Plane(Vector3.up, Vector3.zero);

	private bool done;

	private DebugFlagsScript.ForceStartingPlayer savedForceStartingPlayer;

	private int dragTouchID = INVALID_DRAG_TOUCH_ID;

	private Vector2 dragStartPos;

	private Vector2 prevDragPos;

	private Spinner targetSpinner;

	public int clickCount;

	private void Awake()
	{
		Reset();
	}

	private void Start()
	{
		GetComponent<Rigidbody>().maxAngularVelocity = 200f;
		GameData = GameDataScript.GetInstance();
		GameInstance = GameState.Instance;
		if (fingerCollider != null)
		{
			fingerCollider.SetActive(false);
		}
		Spinner[] array = fcSpinners;
		foreach (Spinner spinnerDef in array)
		{
			SetSpinnerActive(spinnerDef, false);
		}
		Spinner[] array2 = normalSpinners;
		foreach (Spinner spinnerDef2 in array2)
		{
			SetSpinnerActive(spinnerDef2, false);
		}
		Spinner[] array3 = ((!GameInstance.ActiveQuest.QuestType.StartsWith("fc")) ? normalSpinners : fcSpinners);
		targetSpinner = array3[KFFRandom.GetRandomIndex(array3.Length)];
		SetSpinnerActive(targetSpinner, true);
	}

	private void SetSpinnerActive(Spinner spinnerDef, bool active)
	{
		spinnerDef.spinner.gameObject.SetActive(active);
		spinnerDef.shadow.gameObject.SetActive(active);
	}

	public void SetSpinFlag()
	{
		SpinSpeed = 50f;
		GetComponent<Rigidbody>().angularVelocity = new Vector3(GetComponent<Rigidbody>().angularVelocity.x, 150f + UnityEngine.Random.value * 50f * (float)((!(UnityEngine.Random.value > 0.5f)) ? 1 : (-1)), GetComponent<Rigidbody>().angularVelocity.z);
		Spin = true;
		base.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		savedForceStartingPlayer = forceStartingPlayer;
	}

	private bool DeterminePlayer(float angle)
	{
		angle = Mathf.Repeat(angle, 360f);
		if (angle >= 180f)
		{
			angle -= 360f;
		}
		if (angle >= 0f)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
		if (!Spin)
		{
			return;
		}
		if (bottleSpinLabel.activeSelf)
		{
			bottleSpinLabel.SetActive(false);
		}
		DebugFlagsScript.ForceStartingPlayer forceStartingPlayer = savedForceStartingPlayer;
		if (forceStartingPlayer == DebugFlagsScript.ForceStartingPlayer.None)
		{
			forceStartingPlayer = DebugFlagsScript.GetInstance().forceStartingPlayer;
		}
		if (forceStartingPlayer != 0 && Mathf.Abs(GetComponent<Rigidbody>().angularVelocity.y) <= FORCE_ANGULAR_VELOCITY)
		{
			bool flag = DeterminePlayer(base.transform.eulerAngles.y);
			switch (forceStartingPlayer)
			{
			case DebugFlagsScript.ForceStartingPlayer.Me:
				if (!flag)
				{
					GetComponent<Rigidbody>().angularVelocity = new Vector3(GetComponent<Rigidbody>().angularVelocity.x, FORCE_ANGULAR_VELOCITY * (float)((!(GetComponent<Rigidbody>().angularVelocity.y < 0f)) ? 1 : (-1)), GetComponent<Rigidbody>().angularVelocity.z);
				}
				break;
			case DebugFlagsScript.ForceStartingPlayer.Them:
				if (flag)
				{
					GetComponent<Rigidbody>().angularVelocity = new Vector3(GetComponent<Rigidbody>().angularVelocity.x, FORCE_ANGULAR_VELOCITY * (float)((!(GetComponent<Rigidbody>().angularVelocity.y < 0f)) ? 1 : (-1)), GetComponent<Rigidbody>().angularVelocity.z);
				}
				break;
			}
		}
		if (Mathf.Abs(GetComponent<Rigidbody>().angularVelocity.y) > STOP_ANGULAR_VELOCITY)
		{
			return;
		}
		GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
		bool flag2 = DeterminePlayer(base.transform.eulerAngles.y);
		bool flag3 = true;
		if (forceStartingPlayer != 0)
		{
			switch (forceStartingPlayer)
			{
			case DebugFlagsScript.ForceStartingPlayer.Me:
				if (!flag2)
				{
					flag3 = false;
				}
				break;
			case DebugFlagsScript.ForceStartingPlayer.Them:
				if (flag2)
				{
					flag3 = false;
				}
				break;
			}
		}
		if (flag3)
		{
			if (flag2)
			{
				GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.FinishedBottleSpin);
				SetupFirstPlayer(PlayerType.User);
			}
			else
			{
				GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				SetupFirstPlayer(PlayerType.Opponent);
			}
		}
		else
		{
			GetComponent<Rigidbody>().angularVelocity = new Vector3(GetComponent<Rigidbody>().angularVelocity.x, STOP_ANGULAR_VELOCITY * (float)((!(GetComponent<Rigidbody>().angularVelocity.y < 0f)) ? 1 : (-1)), GetComponent<Rigidbody>().angularVelocity.z);
		}
	}

	private void SetupFirstPlayer(PlayerType player)
	{
		if (GlobalFlags.Instance.InMPMode)
		{
			whoGoesFirstLabel.text = string.Format(KFFLocalization.Get("!!FORMAT_PLAYER_GOES_FIRST"), (player != PlayerType.User) ? PlayerInfoScript.GetInstance().MPOpponentName : PlayerInfoScript.GetInstance().MPPlayerName);
		}
		else
		{
			whoGoesFirstLabel.text = string.Format(KFFLocalization.Get("!!FORMAT_PLAYER_GOES_FIRST"), GameInstance.GetDeck(player).Leader.Form.Name);
		}
		tweenController.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		GameInstance.SetMagicPoints(player, ParametersManager.Instance.Starting_Magic_Points);
		GameData.FirstPlayer = (int)player + 1;
		CWiTweenCamTrigger component = battleReadyButton.GetComponent<CWiTweenCamTrigger>();
		component.tweenName = ((player != PlayerType.User) ? "ToP2Setup" : "ToP1Setup");
		CWSetPhase component2 = battleReadyButton.GetComponent<CWSetPhase>();
		component2.setPhase = ((player != PlayerType.User) ? BattlePhase.P2SetupBanner : BattlePhase.P1SetupBanner);
		Spin = false;
		done = true;
		CWTapDelegate component3 = GetComponent<CWTapDelegate>();
		if (component3 != null)
		{
			component3.disableFlag = false;
		}
	}

	private void LateUpdate()
	{
		Transform spinner = targetSpinner.spinner;
		Transform shadow = targetSpinner.shadow;
		shadow.position = new Vector3(spinner.position.x - 1f, spinner.position.y - 1f, spinner.position.z + 1f);
		shadow.eulerAngles = spinner.eulerAngles;
		shadow.localScale = spinner.localScale;
	}

	private void OnClick()
	{
		clickCount++;
	}

	private bool ShouldDisableInput(GameObject selection)
	{
		if (UICamera.useInputEnabler)
		{
			UIInputEnabler uIInputEnabler = ((!(selection == null)) ? selection.GetComponent<UIInputEnabler>() : null);
			if (uIInputEnabler == null || !uIInputEnabler.inputEnabled)
			{
				return true;
			}
		}
		return false;
	}

	private void OnFingerDown(FingerDownEvent e)
	{
		if (!Spin && !done && !ShouldDisableInput(base.gameObject) && !dragging && (activeFinger == null || e.Finger == activeFinger))
		{
			activeFinger = e.Finger;
			dragStartPos = e.Position;
		}
	}

	private void OnFingerUp(FingerUpEvent e)
	{
		if (!Spin && !done && !ShouldDisableInput(base.gameObject) && activeFinger == e.Finger)
		{
			activeFinger = null;
			dragging = false;
		}
	}

	private void OnFingerHover(FingerHoverEvent e)
	{
		if (Spin || done || ShouldDisableInput(base.gameObject) || e.Finger != activeFinger || dragging)
		{
			return;
		}
		switch (e.Phase)
		{
		case FingerHoverPhase.Enter:
			if (activeFinger == null || e.Finger == activeFinger)
			{
				activeFinger = e.Finger;
				dragStartPos = e.Position;
			}
			break;
		case FingerHoverPhase.Exit:
			if (activeFinger == e.Finger)
			{
				activeFinger = null;
				dragging = false;
			}
			break;
		}
	}

	private void OnFingerMove(FingerMotionEvent e)
	{
		if (!Spin && !done && !ShouldDisableInput(base.gameObject) && activeFinger != null && activeFinger == e.Finger)
		{
			if (dragging)
			{
				Vector2 vector = GameCamera.WorldToScreenPoint(targetSpinner.spinner.position);
				Vector2 vector2 = UICamera.lastTouchPosition - vector;
				Vector2 vector3 = prevDragPos - vector;
				float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
				float num2 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
				float num3 = num - num2;
				GetComponent<Rigidbody>().angularVelocity = new Vector3(GetComponent<Rigidbody>().angularVelocity.x, GetComponent<Rigidbody>().angularVelocity.y - num3 * SPIN_ANGLE_SCALE, GetComponent<Rigidbody>().angularVelocity.z);
				CheckSpin();
			}
			else if ((UICamera.lastTouchPosition - dragStartPos).sqrMagnitude > DRAG_DISTANCE * DRAG_DISTANCE)
			{
				dragging = true;
			}
			prevDragPos = UICamera.lastTouchPosition;
		}
	}

	public void OnPress(bool isDown)
	{
	}

	public void OnDrag(Vector2 delta)
	{
	}

	private void UpdateFingerCollider(Vector2 fingerPosition)
	{
		Vector3 worldPos;
		if (fingerCollider != null && raycast(fingerPosition, out worldPos))
		{
			worldPos.y = base.transform.position.y;
			fingerCollider.transform.position = worldPos;
		}
	}

	private bool raycast(Vector3 fingerPos, out Vector3 worldPos)
	{
		worldPos = Vector3.zero;
		Ray ray = GameCamera.ScreenPointToRay(fingerPos);
		float enter = 0f;
		if (!tablePlane.Raycast(ray, out enter))
		{
			return false;
		}
		worldPos = ray.GetPoint(enter);
		return true;
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!done && !Spin && fingerCollider != null && other != null && other.collider == fingerCollider.GetComponent<Collider>())
		{
			CheckSpin();
		}
	}

	private void CheckSpin()
	{
		if (Mathf.Abs(GetComponent<Rigidbody>().angularVelocity.y) > MIN_ANGULAR_VELOCITY)
		{
			GetComponent<Rigidbody>().angularVelocity = new Vector3(GetComponent<Rigidbody>().angularVelocity.x, (GetComponent<Rigidbody>().angularVelocity.y + UnityEngine.Random.value * ANGULAR_RANDOMNESS * (float)((GetComponent<Rigidbody>().angularVelocity.y > 0f) ? 1 : (-1))) * ANGULAR_VELOCITY_SCALE, GetComponent<Rigidbody>().angularVelocity.z);
			float y = GetComponent<Rigidbody>().angularVelocity.y;
			if (fingerCollider != null)
			{
				fingerCollider.SetActive(false);
			}
			if (spinButton != null)
			{
				spinButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				GetComponent<Rigidbody>().angularVelocity = new Vector3(GetComponent<Rigidbody>().angularVelocity.x, y, GetComponent<Rigidbody>().angularVelocity.z);
			}
			else
			{
				Spin = true;
				base.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				savedForceStartingPlayer = forceStartingPlayer;
			}
		}
	}

	private void OnDisable()
	{
		Reset();
	}

	private void OnDestroy()
	{
		Reset();
	}

	private void Reset()
	{
		forceStartingPlayer = DebugFlagsScript.ForceStartingPlayer.None;
	}
}
