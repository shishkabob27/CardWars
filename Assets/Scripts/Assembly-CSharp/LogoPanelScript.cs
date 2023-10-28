using System.Collections;
using UnityEngine;

public class LogoPanelScript : MonoBehaviour
{
	public UITexture CNLogo;

	public UITexture D3Logo;

	public UITexture KFFLogo;

	public UITexture Loading;

	public UITexture Background;

	public TweenColor backGroundTween;

	public bool IsComplete;

	private float Alpha;

	private float Timer;

	private bool isReturn;

	private void Start()
	{
		if (CNLogo != null)
		{
			CNLogo.enabled = true;
			CNLogo.color = new Color(1f, 1f, 1f, 0f);
		}
		if (D3Logo != null)
		{
			D3Logo.enabled = true;
			D3Logo.color = new Color(1f, 1f, 1f, 0f);
		}
		if (KFFLogo != null)
		{
			KFFLogo.enabled = true;
			KFFLogo.color = new Color(1f, 1f, 1f, 0f);
		}
		if (Background != null)
		{
			Background.enabled = true;
			backGroundTween = Background.GetComponent<TweenColor>();
		}
		isReturn = GlobalFlags.Instance.ReturnToMainMenu || GlobalFlags.Instance.ReturnToBuildDeck;
		if (isReturn)
		{
			IsComplete = true;
			if (Loading != null)
			{
				Loading.enabled = true;
				Loading.color = new Color(1f, 1f, 1f, 1f);
			}
			if (CNLogo != null)
			{
				CNLogo.enabled = false;
			}
			if (D3Logo != null)
			{
				D3Logo.enabled = false;
			}
			if (KFFLogo != null)
			{
				KFFLogo.enabled = false;
			}
			if (Background != null)
			{
				Background.enabled = false;
			}
		}
		else
		{
			if (Loading != null)
			{
				Loading.enabled = false;
			}
			if (!AuthScreenController.AuthStarted)
			{
				PlayerInfoScript.GetInstance().Login();
			}
		}
	}

	private void CleanupAndDestroy()
	{
		CNLogo = null;
		D3Logo = null;
		KFFLogo = null;
		Background = null;
		Loading = null;
		Object.Destroy(base.gameObject);
		SLOTGame.GetInstance().StartCoroutine(DelayedResourceUnload());
	}

	private IEnumerator DelayedResourceUnload()
	{
		yield return 0;
		yield return 0;
		Resources.UnloadUnusedAssets();
	}

	public void Complete()
	{
		if ((bool)Loading)
		{
			Loading.enabled = false;
		}
		CleanupAndDestroy();
	}

	private void Update()
	{
		if (!isReturn)
		{
			Timer += Time.deltaTime;
			if (Timer < 2f)
			{
				Alpha += Time.deltaTime;
				CNLogo.color = new Color(1f, 1f, 1f, Mathf.Clamp(Alpha, 0f, 1f));
			}
			else if (Timer < 4f)
			{
				Alpha -= Time.deltaTime;
				CNLogo.color = new Color(1f, 1f, 1f, Mathf.Clamp(Alpha, 0f, 1f));
			}
			else if (Timer < 6f)
			{
				backGroundTween.Play(true);
				CNLogo.color = new Color(1f, 1f, 1f, 0f);
				Alpha += Time.deltaTime;
				D3Logo.color = new Color(1f, 1f, 1f, Mathf.Clamp(Alpha, 0f, 1f));
			}
			else if (Timer < 8f)
			{
				Alpha -= Time.deltaTime;
				D3Logo.color = new Color(1f, 1f, 1f, Mathf.Clamp(Alpha, 0f, 1f));
			}
			else if (Timer < 10f)
			{
				D3Logo.color = new Color(1f, 1f, 1f, 0f);
				Alpha += Time.deltaTime;
				KFFLogo.color = new Color(1f, 1f, 1f, Mathf.Clamp(Alpha, 0f, 1f));
			}
			else if (Timer < 12f)
			{
				Alpha -= Time.deltaTime;
				KFFLogo.color = new Color(1f, 1f, 1f, Mathf.Clamp(Alpha, 0f, 1f));
			}
			else
			{
				IsComplete = true;
				CleanupAndDestroy();
			}
		}
	}
}
