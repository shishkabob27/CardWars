using UnityEngine;

public class LandscapeScript : MonoBehaviour
{
	public GameObject Corn;

	public bool FadeIn;

	private float Timer;

	private Shader TransparentColored;

	private Shader TransparentDiffuse;

	private Shader UnlitTransparent;

	private Shader UnlitTexture;

	private void Start()
	{
		TransparentColored = Shader.Find("Unlit/Transparent Colored");
		TransparentDiffuse = Shader.Find("Transparent/Diffuse");
		UnlitTransparent = Shader.Find("Unlit/Transparent");
		UnlitTexture = Shader.Find("Unlit/Texture");
		if (Corn != null)
		{
			Corn.transform.localScale = new Vector3(1f, 1f, 0.01f);
		}
		if (GetComponent<Renderer>().materials.Length == 1)
		{
			GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);
			return;
		}
		GetComponent<Renderer>().materials[1].shader = TransparentDiffuse;
		GetComponent<Renderer>().materials[0].color = new Color(1f, 1f, 1f, 0f);
		GetComponent<Renderer>().materials[1].color = new Color(1f, 1f, 1f, 0f);
	}

	private void Update()
	{
		if (FadeIn)
		{
			if (Timer < 1f)
			{
				Timer += Time.deltaTime;
				if (Corn != null)
				{
					Corn.transform.localScale = new Vector3(1f, 1f, Timer);
					Corn.GetComponent<Renderer>().material.shader = TransparentColored;
					Corn.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, Timer);
				}
				if (GetComponent<Renderer>().materials.Length == 1)
				{
					GetComponent<Renderer>().material.shader = TransparentDiffuse;
					GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, Timer);
				}
				else
				{
					GetComponent<Renderer>().materials[0].shader = TransparentDiffuse;
					GetComponent<Renderer>().materials[1].shader = TransparentDiffuse;
					GetComponent<Renderer>().materials[0].color = new Color(1f, 1f, 1f, Timer);
					GetComponent<Renderer>().materials[1].color = new Color(1f, 1f, 1f, Timer);
				}
			}
			if (Timer > 1f)
			{
				Timer = 1f;
				if (Corn != null)
				{
					Corn.GetComponent<Renderer>().material.shader = UnlitTransparent;
					Corn.transform.localScale = new Vector3(1f, 1f, 1f);
				}
				if (GetComponent<Renderer>().materials.Length == 1)
				{
					GetComponent<Renderer>().material.shader = UnlitTexture;
					return;
				}
				GetComponent<Renderer>().materials[0].shader = UnlitTexture;
				GetComponent<Renderer>().materials[1].shader = UnlitTransparent;
			}
			return;
		}
		if (Timer > 0f)
		{
			Timer -= Time.deltaTime;
			if (Corn != null)
			{
				Corn.GetComponent<Renderer>().material.shader = TransparentColored;
				Corn.transform.localScale = new Vector3(1f, 1f, Mathf.Max(Timer, 0.01f));
			}
			if (GetComponent<Renderer>().materials.Length == 1)
			{
				GetComponent<Renderer>().material.shader = TransparentDiffuse;
				GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, Timer);
			}
			else
			{
				GetComponent<Renderer>().materials[0].shader = TransparentDiffuse;
				GetComponent<Renderer>().materials[1].shader = TransparentDiffuse;
				GetComponent<Renderer>().materials[0].color = new Color(1f, 1f, 1f, Timer);
				GetComponent<Renderer>().materials[1].color = new Color(1f, 1f, 1f, Timer);
			}
		}
		if (Timer < 0f)
		{
			Timer = 0f;
			if (Corn != null)
			{
				Corn.transform.localScale = new Vector3(1f, 1f, 0.01f);
			}
		}
	}
}
