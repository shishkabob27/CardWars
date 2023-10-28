using UnityEngine;

public class NcTilingTexture : NcEffectBehaviour
{
	public float m_fTilingX = 2f;

	public float m_fTilingY = 2f;

	public float m_fOffsetX;

	public float m_fOffsetY;

	public bool m_bFixedTileSize;

	protected Vector3 m_OriginalScale = default(Vector3);

	protected Vector2 m_OriginalTiling = default(Vector2);

	private void Start()
	{
		if (GetComponent<Renderer>() != null && GetComponent<Renderer>().material != null)
		{
			GetComponent<Renderer>().material.mainTextureScale = new Vector2(m_fTilingX, m_fTilingY);
			GetComponent<Renderer>().material.mainTextureOffset = new Vector2(m_fOffsetX, m_fOffsetY);
			AddRuntimeMaterial(GetComponent<Renderer>().material);
		}
	}

	private void Update()
	{
		if (m_bFixedTileSize)
		{
			if (m_OriginalScale.x != 0f)
			{
				m_fTilingX = m_OriginalTiling.x * (base.transform.lossyScale.x / m_OriginalScale.x);
			}
			if (m_OriginalScale.y != 0f)
			{
				m_fTilingY = m_OriginalTiling.y * (base.transform.lossyScale.y / m_OriginalScale.y);
			}
			GetComponent<Renderer>().material.mainTextureScale = new Vector2(m_fTilingX, m_fTilingY);
		}
	}

	public override void OnUpdateToolData()
	{
		m_OriginalScale = base.transform.lossyScale;
		m_OriginalTiling.x = m_fTilingX;
		m_OriginalTiling.y = m_fTilingY;
	}
}
