using UnityEngine;

public class FxmTestSingleMain : MonoBehaviour
{
	public GameObject[] m_EffectPrefabs = new GameObject[1];

	public GUIText m_EffectGUIText;

	public int m_nIndex;

	public float m_fCreateScale = 1f;

	public int m_nCreateCount = 1;

	public float m_fRandomRange = 1f;

	private void Awake()
	{
	}

	private void OnEnable()
	{
	}

	private void Start()
	{
		Resources.UnloadUnusedAssets();
		Invoke("CreateEffect", 1f);
	}

	private void CreateEffect()
	{
		if (!(m_EffectPrefabs[m_nIndex] == null))
		{
			if (m_EffectGUIText != null)
			{
				m_EffectGUIText.text = m_EffectPrefabs[m_nIndex].name;
			}
			float num = 0f;
			if (1 < m_nCreateCount)
			{
				num = m_fRandomRange;
			}
			for (int i = 0; i < GetInstanceRoot().transform.GetChildCount(); i++)
			{
				Object.Destroy(GetInstanceRoot().transform.GetChild(i).gameObject);
			}
			for (int j = 0; j < m_nCreateCount; j++)
			{
				GameObject gameObject = (GameObject)Object.Instantiate(m_EffectPrefabs[m_nIndex], new Vector3(Random.Range(0f - num, num), 0f, Random.Range(0f - num, num)), Quaternion.identity);
				gameObject.transform.localScale = gameObject.transform.localScale * m_fCreateScale;
				NcEffectBehaviour.PreloadTexture(gameObject);
				gameObject.transform.parent = GetInstanceRoot().transform;
				SetActiveRecursively(gameObject, true);
			}
		}
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (GUI.Button(GetButtonRect(0), "Next"))
		{
			if (m_nIndex < m_EffectPrefabs.Length - 1)
			{
				m_nIndex++;
			}
			else
			{
				m_nIndex = 0;
			}
			CreateEffect();
		}
		if (GUI.Button(GetButtonRect(1), "Recreate"))
		{
			CreateEffect();
		}
	}

	public GameObject GetInstanceRoot()
	{
		return NcEffectBehaviour.GetRootInstanceEffect();
	}

	public static Rect GetButtonRect()
	{
		int num = 2;
		return new Rect(Screen.width - Screen.width / 10 * num, Screen.height - Screen.height / 10, Screen.width / 10 * num, Screen.height / 10);
	}

	public static Rect GetButtonRect(int nIndex)
	{
		return new Rect(Screen.width - Screen.width / 10 * (nIndex + 1), Screen.height - Screen.height / 10, Screen.width / 10, Screen.height / 10);
	}

	public static void SetActiveRecursively(GameObject target, bool bActive)
	{
		int num = target.transform.GetChildCount() - 1;
		while (0 <= num)
		{
			if (num < target.transform.GetChildCount())
			{
				SetActiveRecursively(target.transform.GetChild(num).gameObject, bActive);
			}
			num--;
		}
		target.SetActive(bActive);
	}
}
