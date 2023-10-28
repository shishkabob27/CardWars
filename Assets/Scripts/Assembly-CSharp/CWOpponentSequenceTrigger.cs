using UnityEngine;

public class CWOpponentSequenceTrigger : MonoBehaviour
{
	private CWOpponentActionSequencer oppActionSqcr;

	private void OnClick()
	{
		oppActionSqcr = CWOpponentActionSequencer.GetInstance();
		StartCoroutine(oppActionSqcr.StartOpponentSequence());
	}

	private void Update()
	{
	}
}
