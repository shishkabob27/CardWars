using UnityEngine;

public class MoveTest : MonoBehaviour
{
	private void Start()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("path", iTweenPath.GetPath("BoxIn"), "time", 5));
	}

	private void Update()
	{
	}
}
