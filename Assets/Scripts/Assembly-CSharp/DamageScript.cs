using UnityEngine;

public class DamageScript : MonoBehaviour
{
	public UILabel Label;

	public float Momentum;

	public Vector3 StartPosition;

	private void Start()
	{
		Momentum = -11f;
		Label = GetComponent<UILabel>();
		StartPosition = base.transform.localPosition;
	}

	public void UpdateLabel(int Damage)
	{
		base.transform.localPosition = new Vector3(StartPosition.x, StartPosition.y, 0f);
		Label.text = Damage.ToString();
		Momentum = 10f;
	}

	private void Update()
	{
		if (Momentum > -20f)
		{
			Momentum -= Time.deltaTime * 20f;
			Label.color = new Color(1f, 0f, 0f, 1f + Momentum * 0.1f);
			if (StartPosition.x > 0f)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x + Time.deltaTime * 100f, base.transform.localPosition.y + Momentum, base.transform.localPosition.z);
			}
			else
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x - Time.deltaTime * 100f, base.transform.localPosition.y + Momentum, base.transform.localPosition.z);
			}
		}
	}
}
