using UnityEngine;

public class LodUiTexture : MonoBehaviour
{
	[SerializeField]
	public UITexture texture;
	[SerializeField]
	public int cutoff3by2;
	[SerializeField]
	public int cutoff4by3;
	[SerializeField]
	public int cutoff16by9;
	[SerializeField]
	protected string texture3by2Normal;
	[SerializeField]
	protected string texture3by2Large;
	[SerializeField]
	protected string texture4by3Normal;
	[SerializeField]
	protected string texture4by3Large;
	[SerializeField]
	protected string texture16by9Normal;
	[SerializeField]
	protected string texture16by9Large;
}
