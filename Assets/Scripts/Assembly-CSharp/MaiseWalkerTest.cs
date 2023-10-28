using UnityEngine;

public class MaiseWalkerTest : MonoBehaviour
{
	public Renderer Mouth;

	public Texture Mouth_MPandB;

	public Texture Mouth_AltTH;

	public Texture Mouth_OpenSmall;

	public Texture Mouth_AandI;

	public Texture Mouth_O;

	public Texture Mouth_Consonants;

	public Texture Mouth_LandTH;

	public Texture Mouth_AltConsonants;

	public Texture Mouth_WandQ;

	public Texture Mouth_OpenSmall2;

	public bool AudioPlayed;

	public float Timer;

	private void Update()
	{
		Timer += Time.deltaTime;
		if (Timer > 1.2f && !AudioPlayed)
		{
			AudioPlayed = true;
			GetComponent<AudioSource>().Play();
		}
		if (Timer > 1.4333f)
		{
			Mouth.material.mainTexture = Mouth_AltTH;
		}
		if (Timer > 1.4667f)
		{
			Mouth.material.mainTexture = Mouth_OpenSmall;
		}
		if (Timer > 1.6333f)
		{
			Mouth.material.mainTexture = Mouth_AandI;
		}
		if ((double)Timer > 1.7)
		{
			Mouth.material.mainTexture = Mouth_MPandB;
		}
		if (Timer > 1.7667f)
		{
			Mouth.material.mainTexture = Mouth_O;
		}
		if (Timer > 1.9667f)
		{
			Mouth.material.mainTexture = Mouth_Consonants;
		}
		if (Timer > 2.0333f)
		{
			Mouth.material.mainTexture = Mouth_AandI;
		}
		if (Timer > 2.1333f)
		{
			Mouth.material.mainTexture = Mouth_LandTH;
		}
		if (Timer > 2.2f)
		{
			Mouth.material.mainTexture = Mouth_MPandB;
		}
		if (Timer > 2.2667f)
		{
			Mouth.material.mainTexture = Mouth_AandI;
		}
		if (Timer > 2.4667f)
		{
			Mouth.material.mainTexture = Mouth_Consonants;
		}
		if (Timer > 2.5333f)
		{
			Mouth.material.mainTexture = Mouth_AltConsonants;
		}
		if (Timer > 2.6f)
		{
			Mouth.material.mainTexture = Mouth_WandQ;
		}
		if (Timer > 2.6333f)
		{
			Mouth.material.mainTexture = Mouth_O;
		}
		if (Timer > 2.6667f)
		{
			Mouth.material.mainTexture = Mouth_AandI;
		}
		if (Timer > 2.8667f)
		{
			Mouth.material.mainTexture = Mouth_Consonants;
		}
		if (Timer > 2.9f)
		{
			Mouth.material.mainTexture = Mouth_AandI;
		}
		if (Timer > 3.1333f)
		{
			Mouth.material.mainTexture = Mouth_OpenSmall2;
		}
		if (Timer > 3.3f)
		{
			Mouth.material.mainTexture = Mouth_MPandB;
		}
	}
}
