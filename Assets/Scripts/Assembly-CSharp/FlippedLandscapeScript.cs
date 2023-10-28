using UnityEngine;

public class FlippedLandscapeScript : MonoBehaviour
{
	public Collider MyCollider;

	private bool FadeIn;

	public bool Flash;

	public float Alpha;

	private Color color = new Color(1f, 1f, 1f);

	private LandscapeType faction;

	public AudioClip SelectionSound;

	public bool doNotStopHighlightFlag;

	public int Index { get; set; }

	public LandscapeType LandType
	{
		get
		{
			return faction;
		}
		set
		{
			faction = value;
			color = FactionManager.Instance.GetFactionData(faction).FactionColor;
		}
	}

	private void Start()
	{
		MyCollider.enabled = false;
	}

	private void Update()
	{
		if (!Flash)
		{
			return;
		}
		MyCollider.enabled = true;
		if (FadeIn)
		{
			Alpha += Time.deltaTime;
			if (Alpha > 1f)
			{
				FadeIn = false;
			}
		}
		else
		{
			Alpha -= Time.deltaTime;
			if (Alpha < 0f)
			{
				FadeIn = true;
			}
		}
		GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, Alpha);
	}

	private void OnClick()
	{
		if (Flash)
		{
			if (SelectionSound != null)
			{
				SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(SelectionSound);
			}
			GameState.Instance.SelectTarget(Index);
		}
	}

	public void TurnWhite()
	{
		MyCollider.enabled = false;
		FadeIn = false;
		Flash = false;
		Alpha = 1f;
	}

	public void StopHighlight()
	{
		MyCollider.enabled = false;
		FadeIn = false;
		Flash = false;
		Alpha = 0f;
		Color color = GetComponent<Renderer>().material.color;
		if (!doNotStopHighlightFlag)
		{
			GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 0f);
		}
	}

	private Faction GetFactionFromLandscape(LandscapeType land)
	{
		switch (land)
		{
		case LandscapeType.Corn:
			return Faction.Corn;
		case LandscapeType.Cotton:
			return Faction.Cotton;
		case LandscapeType.Sand:
			return Faction.Sand;
		case LandscapeType.Swamp:
			return Faction.Swamp;
		case LandscapeType.Plains:
			return Faction.Plains;
		default:
			return Faction.Universal;
		}
	}
}
