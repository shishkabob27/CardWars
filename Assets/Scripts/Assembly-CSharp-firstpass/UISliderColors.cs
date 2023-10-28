using UnityEngine;

[AddComponentMenu("NGUI/Examples/Slider Colors")]
[ExecuteInEditMode]
[RequireComponent(typeof(UISlider))]
public class UISliderColors : MonoBehaviour
{
	public UISprite sprite;

	public Color[] colors = new Color[3]
	{
		Color.red,
		Color.yellow,
		Color.green
	};

	private UISlider mSlider;

	private void Start()
	{
		mSlider = GetComponent<UISlider>();
		Update();
	}

	private void Update()
	{
		if (sprite == null || colors.Length == 0)
		{
			return;
		}
		float sliderValue = mSlider.sliderValue;
		sliderValue *= (float)(colors.Length - 1);
		int num = Mathf.FloorToInt(sliderValue);
		Color color = colors[0];
		if (num >= 0)
		{
			if (num + 1 >= colors.Length)
			{
				color = ((num >= colors.Length) ? colors[colors.Length - 1] : colors[num]);
			}
			else
			{
				float t = sliderValue - (float)num;
				color = Color.Lerp(colors[num], colors[num + 1], t);
			}
		}
		color.a = sprite.color.a;
		sprite.color = color;
	}
}
