using UnityEngine;

public class DebugPauseGamePause : MonoBehaviour
{
	public UISprite sprite;

	public UILabel sliderVal;

	private int clickCount;

	private void OnClick()
	{
		clickCount++;
		if (clickCount % 2 == 1)
		{
			Time.timeScale = 0f;
			sprite.spriteName = "Playback_Play";
		}
		else
		{
			Time.timeScale = 1f;
			sprite.spriteName = "Playback_Pause";
		}
	}

	private void OnSliderChange(float val)
	{
		Time.timeScale = val;
		sliderVal.text = val.ToString();
	}

	private void Update()
	{
	}
}
