using UnityEngine;

public class MovingDweebCardScript : MonoBehaviour
{
	public DweebCupManagerScript DweebCupManager;

	public GameObject Splash;

	public UITexture CardFrame;

	public UITexture CardArt;

	public UILabel CardName;

	public UILabel CardDesc;

	public Transform AboveCup;

	public Transform Cup;

	public AudioClip SplashSound;

	public AudioClip CardMove;

	public Vector3 StartPosition;

	public Vector3 TouchStart;

	public Vector3 TouchCurrent;

	public Vector3 TouchLast;

	public bool JustReleased;

	public bool FollowFinger;

	public bool FingerDown;

	public bool Scrolling;

	public bool EnterCup;

	public bool FloatUp;

	public bool Grow;

	public bool Bob;

	public bool Up;

	public float ParentStart;

	public float TouchDelta;

	public float Rotation;

	public float Float;

	public CardItem CardData;

	private PanelManager pManager;

	public Camera uiCamera;

	private void Start()
	{
		pManager = PanelManager.GetInstance();
		uiCamera = pManager.uiCamera;
		DweebCupManager = GameObject.Find("F_2_DweebCupManager").GetComponent<DweebCupManagerScript>();
		AboveCup = GameObject.Find("AboveCupTransform").transform;
		Cup = GameObject.Find("P1CupTransform").transform;
		StartPosition = base.transform.localPosition;
		UpdateCard();
		base.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
	}

	private void OnPress(bool isPressed)
	{
		if (isPressed && !DweebCupManager.Ante && !DweebCupManager.CardGrown)
		{
			ParentStart = base.transform.parent.transform.localPosition.x;
			TouchStart = Input.mousePosition;
			DweebCupManager.MoveRight = false;
			DweebCupManager.MoveLeft = false;
			DweebCupManager.Momentum = 0f;
			DweebCupManager.FingerDown = true;
			FingerDown = true;
			JustReleased = false;
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), CardMove);
		}
	}

	private void Update()
	{
		if (!Bob)
		{
			if (Grow)
			{
				if ((double)base.transform.localScale.x > 2.9 && Input.GetMouseButtonUp(0))
				{
					DweebCupManager.CardGrown = false;
					Grow = false;
				}
			}
			else if (!DweebCupManager.CardGrown)
			{
				if (FingerDown)
				{
					TouchCurrent = Input.mousePosition;
					TouchDelta = -2f * (TouchStart.x - TouchCurrent.x);
					if (!FollowFinger && (TouchCurrent.x - TouchStart.x > 10f || TouchCurrent.x - TouchStart.x < -10f))
					{
						Scrolling = true;
					}
					if (Scrolling)
					{
						base.transform.parent.transform.localPosition = new Vector3(ParentStart + TouchDelta, base.transform.parent.transform.localPosition.y, 0f);
					}
					else if (TouchCurrent.y - TouchStart.y > 10f)
					{
						FollowFinger = true;
					}
				}
				if (FollowFinger)
				{
					CardArt.depth = 3;
					CardFrame.depth = 4;
					CardName.depth = 5;
					CardDesc.depth = 5;
					base.transform.position = uiCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9f));
					base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(2f, 2f, 2f), Time.deltaTime * 10f);
				}
				else if (!EnterCup)
				{
					CardArt.depth = 0;
					CardFrame.depth = 1;
					CardName.depth = 2;
					CardDesc.depth = 2;
					base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, StartPosition, Time.deltaTime * 10f);
					base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * 10f);
				}
				else if (FloatUp)
				{
					CardArt.depth = 3;
					CardFrame.depth = 4;
					CardName.depth = 5;
					CardDesc.depth = 5;
					base.transform.position = Vector3.Lerp(base.transform.position, AboveCup.position, Time.deltaTime * 10f);
					base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * 10f);
					if (base.transform.localPosition.y > 780f)
					{
						Object.Instantiate(Splash, Cup.position, Quaternion.identity);
						SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), SplashSound);
						FloatUp = false;
					}
				}
				else
				{
					CardArt.depth = 0;
					CardFrame.depth = 1;
					CardName.depth = 2;
					CardDesc.depth = 2;
					Rotation = Mathf.Lerp(Rotation, 15f, Time.deltaTime * 10f);
					base.transform.eulerAngles = new Vector3(0f, 0f, Rotation);
					base.transform.position = Vector3.Lerp(base.transform.position, Cup.position, Time.deltaTime * 10f);
					base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(2f, 2f, 2f), Time.deltaTime * 10f);
					if ((double)base.transform.position.y < (double)Cup.position.y + 0.01)
					{
						base.transform.localScale = new Vector3(2f, 2f, 2f);
						base.transform.position = Cup.position;
						base.transform.parent = Cup.transform.parent;
						Bob = true;
					}
				}
			}
		}
		else
		{
			if (Up)
			{
				Float += Time.deltaTime * 0.01f;
				if (Float > 0.01f)
				{
					Float = 0.01f;
					Up = false;
				}
			}
			else
			{
				Float -= Time.deltaTime * 0.01f;
				if (Float < -0.01f)
				{
					Float = -0.01f;
					Up = true;
				}
			}
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y + Float, base.transform.localPosition.z);
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (FingerDown)
			{
				JustReleased = true;
				if (TouchCurrent.x < TouchLast.x - 10f)
				{
					DweebCupManager.Momentum = Mathf.Abs(TouchCurrent.x - TouchLast.x);
					DweebCupManager.MoveLeft = true;
				}
				else if (TouchCurrent.x > TouchLast.x + 10f)
				{
					DweebCupManager.Momentum = TouchCurrent.x - TouchLast.x;
					DweebCupManager.MoveRight = true;
				}
			}
			if (FollowFinger && (double)Input.mousePosition.y > (double)Screen.height * 0.5 && (double)Input.mousePosition.x > (double)Screen.width * 0.333 && (double)Input.mousePosition.x < (double)Screen.width * 0.667)
			{
				DweebCupManager.P1_Ante = base.gameObject;
				DweebCupManager.PlayerAnte = CardData;
				DweebCupManager.Ante = true;
				EnterCup = true;
				FloatUp = true;
			}
			DweebCupManager.FingerDown = false;
			FollowFinger = false;
			FingerDown = false;
			Scrolling = false;
		}
		TouchLast = Input.mousePosition;
	}

	private void UpdateCard()
	{
		CardArt.mainTexture = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Dweeb/" + CardData.Form.SpriteName) as Texture;
		CardName.text = CardData.Form.Name;
		CardDesc.text = CardData.Description;
	}

	private void OnDoubleClick()
	{
		if (!DweebCupManager.Ante)
		{
			DweebCupManager.CardGrown = true;
			Grow = true;
		}
	}
}
