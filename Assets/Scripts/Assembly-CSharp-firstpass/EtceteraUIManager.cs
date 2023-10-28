using System.IO;
using Prime31;
using UnityEngine;

public class EtceteraUIManager : MonoBehaviourGUI
{
	public GameObject testPlane;

	private void Start()
	{
		EtceteraAndroid.initTTS();
	}

	private void OnEnable()
	{
		EtceteraAndroidManager.albumChooserSucceededEvent += imageLoaded;
		EtceteraAndroidManager.photoChooserSucceededEvent += imageLoaded;
	}

	private void OnDisable()
	{
		EtceteraAndroidManager.albumChooserSucceededEvent -= imageLoaded;
		EtceteraAndroidManager.photoChooserSucceededEvent -= imageLoaded;
	}

	private string saveScreenshotToSDCard()
	{
		Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		texture2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0, false);
		byte[] bytes = texture2D.EncodeToPNG();
		Object.Destroy(texture2D);
		string text = Path.Combine(Application.persistentDataPath, "myImage.png");
		File.WriteAllBytes(text, bytes);
		return text;
	}

	private void OnGUI()
	{
		beginColumn();
		if (GUILayout.Button("Show Toast"))
		{
			EtceteraAndroid.showToast("Hi.  Something just happened in the game and I want to tell you but not interrupt you", true);
		}
		if (GUILayout.Button("Play Video"))
		{
			EtceteraAndroid.playMovie("http://www.daily3gp.com/vids/747.3gp", 16711680u, false, EtceteraAndroid.ScalingMode.AspectFit, true);
		}
		if (GUILayout.Button("Show Alert"))
		{
			EtceteraAndroid.showAlert("Alert Title Here", "Something just happened.  Do you want to have a snack?", "Yes", "Not Now");
		}
		if (GUILayout.Button("Single Field Prompt"))
		{
			EtceteraAndroid.showAlertPrompt("Enter Digits", "I'll call you if you give me your number", "phone number", "867-5309", "Send", "Not a Chance");
		}
		if (GUILayout.Button("Two Field Prompt"))
		{
			EtceteraAndroid.showAlertPromptWithTwoFields("Need Info", "Enter your credentials:", "username", "harry_potter", "password", string.Empty, "OK", "Cancel");
		}
		if (GUILayout.Button("Show Progress Dialog"))
		{
			EtceteraAndroid.showProgressDialog("Progress is happening", "it will be over in just a second...");
			Invoke("hideProgress", 1f);
		}
		if (GUILayout.Button("Text to Speech Speak"))
		{
			EtceteraAndroid.setPitch(Random.Range(0, 5));
			EtceteraAndroid.setSpeechRate(Random.Range(0.5f, 1.5f));
			EtceteraAndroid.speak("Howdy. Im a robot voice");
		}
		if (GUILayout.Button("Prompt for Video"))
		{
			EtceteraAndroid.promptToTakeVideo("fancyVideo");
		}
		endColumn(true);
		if (GUILayout.Button("Show Web View"))
		{
			EtceteraAndroid.showWebView("http://prime31.com");
		}
		if (GUILayout.Button("Email Composer"))
		{
			string attachmentFilePath = saveScreenshotToSDCard();
			EtceteraAndroid.showEmailComposer("noone@nothing.com", "Message subject", "click <a href='http://somelink.com'>here</a> for a present", true, attachmentFilePath);
		}
		if (GUILayout.Button("SMS Composer"))
		{
			EtceteraAndroid.showSMSComposer("I did something really cool in this game!");
		}
		if (GUILayout.Button("Prompt to Take Photo"))
		{
			EtceteraAndroid.promptToTakePhoto("photo.jpg");
		}
		if (GUILayout.Button("Prompt for Album Image"))
		{
			EtceteraAndroid.promptForPictureFromAlbum("albumImage.jpg");
		}
		if (GUILayout.Button("Save Image to Gallery"))
		{
			string pathToPhoto = saveScreenshotToSDCard();
			bool flag = EtceteraAndroid.saveImageToGallery(pathToPhoto, "My image from Unity");
		}
		if (GUILayout.Button("Ask For Review"))
		{
			EtceteraAndroid.resetAskForReview();
			EtceteraAndroid.askForReviewNow("Please rate my app!", "It will really make me happy if you do...");
		}
		endColumn();
		if (bottomRightButton("Next Scene"))
		{
			Application.LoadLevel("EtceteraTestSceneTwo");
		}
	}

	private void hideProgress()
	{
		EtceteraAndroid.hideProgressDialog();
	}

	public void imageLoaded(string imagePath)
	{
		EtceteraAndroid.scaleImageAtPath(imagePath, 0.1f);
		testPlane.GetComponent<Renderer>().material.mainTexture = EtceteraAndroid.textureFromFileAtPath(imagePath);
	}
}
