using UnityEngine;

public class NcUvAnimation : NcEffectAniBehaviour
{
	public float m_fScrollSpeedX = 1f;

	public float m_fScrollSpeedY;

	public float m_fTilingX = 1f;

	public float m_fTilingY = 1f;

	public float m_fOffsetX;

	public float m_fOffsetY;

	public bool m_bUseSmoothDeltaTime;

	public bool m_bFixedTileSize;

	public bool m_bRepeat = true;

	public bool m_bAutoDestruct;

	protected Vector3 m_OriginalScale = default(Vector3);

	protected Vector2 m_OriginalTiling = default(Vector2);

	protected Vector2 m_EndOffset = default(Vector2);

	protected Vector2 m_RepeatOffset = default(Vector2);

	protected Renderer m_Renderer;

	public void SetFixedTileSize(bool bFixedTileSize)
	{
		m_bFixedTileSize = bFixedTileSize;
	}

	public override int GetAnimationState()
	{
		if (!m_bRepeat)
		{
			int num;
			if (base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && !IsEndAnimation())
			{
				num = 1;
			}
			num = 0;
		}
		return -1;
	}

	public override void ResetAnimation()
	{
		if (!base.enabled)
		{
			base.enabled = true;
		}
		Start();
	}

	private void Start()
	{
		m_Renderer = GetComponent<Renderer>();
		if (m_Renderer == null || m_Renderer.sharedMaterial == null || m_Renderer.sharedMaterial.mainTexture == null)
		{
			base.enabled = false;
			return;
		}
		GetComponent<Renderer>().material.mainTextureScale = new Vector2(m_fTilingX, m_fTilingY);
		AddRuntimeMaterial(GetComponent<Renderer>().material);
		float num = m_fOffsetX + m_fTilingX;
		m_RepeatOffset.x = num - (float)(int)num;
		if (m_RepeatOffset.x < 0f)
		{
			m_RepeatOffset.x += 1f;
		}
		num = m_fOffsetY + m_fTilingY;
		m_RepeatOffset.y = num - (float)(int)num;
		if (m_RepeatOffset.y < 0f)
		{
			m_RepeatOffset.y += 1f;
		}
		m_EndOffset.x = 1f - (m_fTilingX - (float)(int)m_fTilingX + (float)((m_fTilingX - (float)(int)m_fTilingX < 0f) ? 1 : 0));
		m_EndOffset.y = 1f - (m_fTilingY - (float)(int)m_fTilingY + (float)((m_fTilingY - (float)(int)m_fTilingY < 0f) ? 1 : 0));
		InitAnimationTimer();
	}

	private void Update()
	{
		if (m_Renderer == null || m_Renderer.sharedMaterial == null || m_Renderer.sharedMaterial.mainTexture == null)
		{
			return;
		}
		if (m_bFixedTileSize)
		{
			if (m_fScrollSpeedX != 0f && m_OriginalScale.x != 0f)
			{
				m_fTilingX = m_OriginalTiling.x * (base.transform.lossyScale.x / m_OriginalScale.x);
			}
			if (m_fScrollSpeedY != 0f && m_OriginalScale.y != 0f)
			{
				m_fTilingY = m_OriginalTiling.y * (base.transform.lossyScale.y / m_OriginalScale.y);
			}
			GetComponent<Renderer>().material.mainTextureScale = new Vector2(m_fTilingX, m_fTilingY);
		}
		if (m_bUseSmoothDeltaTime)
		{
			m_fOffsetX += m_Timer.GetSmoothDeltaTime() * m_fScrollSpeedX;
			m_fOffsetY += m_Timer.GetSmoothDeltaTime() * m_fScrollSpeedY;
		}
		else
		{
			m_fOffsetX += m_Timer.GetDeltaTime() * m_fScrollSpeedX;
			m_fOffsetY += m_Timer.GetDeltaTime() * m_fScrollSpeedY;
		}
		bool flag = false;
		if (!m_bRepeat)
		{
			m_RepeatOffset.x += m_Timer.GetDeltaTime() * m_fScrollSpeedX;
			if (m_RepeatOffset.x < 0f || 1f < m_RepeatOffset.x)
			{
				m_fOffsetX = m_EndOffset.x;
				base.enabled = false;
				flag = true;
			}
			m_RepeatOffset.y += m_Timer.GetDeltaTime() * m_fScrollSpeedY;
			if (m_RepeatOffset.y < 0f || 1f < m_RepeatOffset.y)
			{
				m_fOffsetY = m_EndOffset.y;
				base.enabled = false;
				flag = true;
			}
		}
		m_Renderer.material.mainTextureOffset = new Vector2(m_fOffsetX, m_fOffsetY);
		if (flag)
		{
			OnEndAnimation();
			if (m_bAutoDestruct)
			{
				Object.DestroyObject(base.gameObject);
			}
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fScrollSpeedX *= fSpeedRate;
		m_fScrollSpeedY *= fSpeedRate;
	}

	public override void OnUpdateToolData()
	{
		m_OriginalScale = base.transform.lossyScale;
		m_OriginalTiling.x = m_fTilingX;
		m_OriginalTiling.y = m_fTilingY;
	}
}
