using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MoviePlayerController : MonoBehaviour
{
	public const string PlatformMarker = "_android";

	public const string MovieExtension = "mp4";

	public string MovieName = string.Empty;

	public bool CanSkip = true;

	public bool PlayOnEnable;

	public bool PlayOnClick;

	public int MaxLifetimePlays = -1;

	[method: MethodImpl(32)]
	public event Action<MoviePlayerController> onComplete;

	private void Start()
	{
	}

	private void OnEnable()
	{
		if (PlayOnEnable)
		{
			PlayMovie();
		}
	}

	public void OnClick()
	{
		if (PlayOnClick)
		{
			PlayMovie();
		}
	}

	public void PlayMovie()
	{
		if (MaxLifetimePlays == 0)
		{
			return;
		}
		if (MaxLifetimePlays != -1)
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			int num = ((null != instance) ? instance.GetOccuranceCounter(MovieName) : 0);
			if (num >= MaxLifetimePlays)
			{
				return;
			}
		}
		string filename = GetFilename();
		if (filename == null)
		{
			TFUtils.ErrorLog("Couldn't find movie to play: " + filename);
			if (this.onComplete != null)
			{
				this.onComplete(this);
			}
		}
		else
		{
			TFUtils.DebugLog("Going to play movie: " + filename);
			StartCoroutine(PlayMovieHelper(filename));
		}
	}

	private IEnumerator PlayMovieHelper(string filename)
	{
		TFUtils.DebugLog("Starting movie: " + MovieName);
		TFUtils.PlayMovie(filename, CanSkip);
		yield return new WaitForSeconds(1f);
		if (MaxLifetimePlays != -1)
		{
			PlayerInfoScript pinfo = PlayerInfoScript.GetInstance();
			if (null != pinfo)
			{
				int newValue = pinfo.IncOccuranceCounter(MovieName);
				TFUtils.DebugLog("User has seen " + MovieName + " (" + newValue + "/" + MaxLifetimePlays + ") max times.");
				pinfo.Save();
			}
		}
		TFUtils.DebugLog("Finished movie: " + MovieName);
		if (this.onComplete != null)
		{
			this.onComplete(this);
		}
	}

	private IEnumerator PlayMovieInEditorHelper()
	{
		yield return new WaitForSeconds(1f);
		if (this.onComplete != null)
		{
			this.onComplete(this);
		}
	}

	private string GetFilename()
	{
		if (string.IsNullOrEmpty(MovieName) || string.IsNullOrEmpty("mp4"))
		{
			return null;
		}
		return MovieName + "_android.mp4";
	}
}
