using UnityEngine;

public class LodUiTexture : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	public UITexture texture;

	[HideInInspector]
	[SerializeField]
	public int cutoff3by2 = 640;

	[HideInInspector]
	[SerializeField]
	public int cutoff4by3 = 2048;

	[SerializeField]
	[HideInInspector]
	public int cutoff16by9 = 1136;

	[HideInInspector]
	[SerializeField]
	protected string texture3by2Normal;

	[HideInInspector]
	[SerializeField]
	protected string texture3by2Large;

	[SerializeField]
	[HideInInspector]
	protected string texture4by3Normal;

	[SerializeField]
	[HideInInspector]
	protected string texture4by3Large;

	[SerializeField]
	[HideInInspector]
	protected string texture16by9Normal;

	[SerializeField]
	[HideInInspector]
	protected string texture16by9Large;

	private static string RESOURCE_STRING = "Assets/Resources";

	public Texture Texture3by2Normal
	{
		get
		{
			return TextureForString(texture3by2Normal);
		}
		set
		{
			texture3by2Normal = ResourceStringFromTexture(value);
		}
	}

	public Texture Texture3by2Large
	{
		get
		{
			return TextureForString(texture3by2Normal);
		}
		set
		{
			texture3by2Normal = ResourceStringFromTexture(value);
		}
	}

	public Texture Texture4by3Normal
	{
		get
		{
			return TextureForString(texture4by3Normal);
		}
		set
		{
			texture4by3Normal = ResourceStringFromTexture(value);
		}
	}

	public Texture Texture4by3Large
	{
		get
		{
			return TextureForString(texture4by3Large);
		}
		set
		{
			texture4by3Large = ResourceStringFromTexture(value);
		}
	}

	public Texture Texture16by9Normal
	{
		get
		{
			return TextureForString(texture16by9Normal);
		}
		set
		{
			texture16by9Normal = ResourceStringFromTexture(value);
		}
	}

	public Texture Texture16by9Large
	{
		get
		{
			return TextureForString(texture16by9Large);
		}
		set
		{
			texture16by9Large = ResourceStringFromTexture(value);
		}
	}

	protected Texture TextureForString(string target)
	{
		return (target != null) ? (Resources.Load(target) as Texture) : null;
	}

	protected string ResourceStringFromTexture(Object target)
	{
		if (target == null)
		{
			return null;
		}
		return null;
	}

	public void Start()
	{
		string text = TextureForResolution();
		if (text != null && this.texture != null)
		{
			Texture texture = TextureForString(text);
			this.texture.gameObject.transform.localScale = new Vector3(texture.width, texture.height, 1f);
			this.texture.mainTexture = texture;
			this.texture.MarkAsChanged();
		}
	}

	protected string TextureForResolution()
	{
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = 0.01f;
		if (Mathf.Abs(num - 1.3333334f) < num2)
		{
			return (Screen.width <= cutoff4by3) ? texture4by3Normal : texture4by3Large;
		}
		if (Mathf.Abs(num - 1.5f) < num2)
		{
			return (Screen.width <= cutoff3by2) ? texture3by2Normal : texture3by2Large;
		}
		if (Mathf.Abs(num - 1.7777778f) < num2)
		{
			return (Screen.width <= cutoff16by9) ? texture16by9Normal : texture16by9Large;
		}
		return null;
	}
}
