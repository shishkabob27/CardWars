using UnityEngine;

[RequireComponent(typeof(FingerDownDetector))]
[RequireComponent(typeof(ScreenRaycaster))]
[RequireComponent(typeof(FingerUpDetector))]
[RequireComponent(typeof(FingerHoverDetector))]
[RequireComponent(typeof(FingerMotionDetector))]
public class FingerEventsSamplePart1 : SampleBase
{
	public GameObject fingerDownObject;

	public GameObject fingerStationaryObject;

	public GameObject fingerHoverObject;

	public GameObject fingerUpObject;

	public float chargeDelay = 0.5f;

	public float chargeTime = 5f;

	public float minSationaryParticleEmissionCount = 5f;

	public float maxSationaryParticleEmissionCount = 50f;

	public Material highlightMaterial;

	private int stationaryFingerIndex = -1;

	private Material originalStationaryMaterial;

	private Material originalHoverMaterial;

	private ParticleEmitter stationaryParticleEmitter;

	private void OnFingerDown(FingerDownEvent e)
	{
		if (e.Selection == fingerDownObject)
		{
			SpawnParticles(fingerDownObject);
		}
	}

	private void OnFingerUp(FingerUpEvent e)
	{
		if (e.Selection == fingerUpObject)
		{
			SpawnParticles(fingerUpObject);
		}
		FingerGestures.Finger finger = e.Finger;
	}

	private void OnFingerHover(FingerHoverEvent e)
	{
		if (e.Selection == fingerHoverObject)
		{
			if (e.Phase == FingerHoverPhase.Enter)
			{
				base.UI.StatusText = "Finger entered " + fingerHoverObject.name;
				originalHoverMaterial = fingerHoverObject.GetComponent<Renderer>().sharedMaterial;
				fingerHoverObject.GetComponent<Renderer>().sharedMaterial = highlightMaterial;
			}
			else if (e.Phase == FingerHoverPhase.Exit)
			{
				base.UI.StatusText = "Finger left " + fingerHoverObject.name;
				fingerHoverObject.GetComponent<Renderer>().sharedMaterial = originalHoverMaterial;
			}
		}
	}

	private void OnFingerStationary(FingerMotionEvent e)
	{
		if (e.Phase == FingerMotionPhase.Started)
		{
			if (stationaryFingerIndex == -1)
			{
				GameObject selection = e.Selection;
				if (selection == fingerStationaryObject)
				{
					base.UI.StatusText = "Begin stationary on finger " + e.Finger.Index;
					stationaryFingerIndex = e.Finger.Index;
					originalStationaryMaterial = selection.GetComponent<Renderer>().sharedMaterial;
					selection.GetComponent<Renderer>().sharedMaterial = highlightMaterial;
				}
			}
		}
		else if (e.Phase == FingerMotionPhase.Updated)
		{
			if (!(e.ElapsedTime < chargeDelay) && e.Selection == fingerStationaryObject)
			{
				float num = Mathf.Clamp01((e.ElapsedTime - chargeDelay) / chargeTime);
				float num2 = Mathf.Lerp(minSationaryParticleEmissionCount, maxSationaryParticleEmissionCount, num);
				stationaryParticleEmitter.minEmission = num2;
				stationaryParticleEmitter.maxEmission = num2;
				stationaryParticleEmitter.emit = true;
				base.UI.StatusText = "Charge: " + (100f * num).ToString("N1") + "%";
			}
		}
		else if (e.Phase == FingerMotionPhase.Ended && e.Finger.Index == stationaryFingerIndex)
		{
			float elapsedTime = e.ElapsedTime;
			base.UI.StatusText = string.Concat("Stationary ended on finger ", e.Finger, " - ", elapsedTime.ToString("N1"), " seconds elapsed");
			StopStationaryParticleEmitter();
			fingerStationaryObject.GetComponent<Renderer>().sharedMaterial = originalStationaryMaterial;
			stationaryFingerIndex = -1;
		}
	}

	protected override string GetHelpText()
	{
		return "This sample lets you visualize and understand the OnFingerDown, OnFingerStationary and OnFingerUp events.\r\n\r\nINSTRUCTIONS:\r\n- Press, hold and release the red and blue spheres\r\n- Press & hold the green sphere without moving for a few seconds\r\n- Move your finger over and out of the cyan OnFingerHover sphere";
	}

	protected override void Start()
	{
		base.Start();
		if ((bool)fingerStationaryObject)
		{
			stationaryParticleEmitter = fingerStationaryObject.GetComponentInChildren<ParticleEmitter>();
		}
	}

	private void StopStationaryParticleEmitter()
	{
		stationaryParticleEmitter.emit = false;
		base.UI.StatusText = string.Empty;
	}

	private void SpawnParticles(GameObject obj)
	{
		ParticleEmitter componentInChildren = obj.GetComponentInChildren<ParticleEmitter>();
		if ((bool)componentInChildren)
		{
			componentInChildren.Emit();
		}
	}
}
