using UnityEngine;

public class CWBattleEndMusicVolChange : MonoBehaviour
{
	public BattleJukeboxScript jukeBox;

	public float volume = 1f;

	private void OnClick()
	{
		if (jukeBox != null)
		{
			jukeBox.Volume = volume;
		}
	}

	private void Update()
	{
	}
}
