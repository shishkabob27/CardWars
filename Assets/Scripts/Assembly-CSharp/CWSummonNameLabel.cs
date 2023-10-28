using UnityEngine;

public class CWSummonNameLabel : MonoBehaviour
{
	private void OnEnable()
	{
		GameState instance = GameState.Instance;
		CardScript summoning = instance.GetSummoning();
		UILabel component = GetComponent<UILabel>();
		component.text = ((summoning == null) ? string.Empty : summoning.Data.Form.Name);
	}

	private void Update()
	{
	}
}
