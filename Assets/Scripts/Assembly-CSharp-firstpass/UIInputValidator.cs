using UnityEngine;

public class UIInputValidator : MonoBehaviour
{
	public enum Validation
	{
		None = 0,
		Integer = 1,
		Float = 2,
		Alphanumeric = 3,
		Username = 4,
		Name = 5,
	}

	public Validation logic;
}
