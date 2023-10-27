using UnityEngine;

public class FBSettings : ScriptableObject
{
	[SerializeField]
	private int selectedAppIndex;
	[SerializeField]
	private string[] appIds;
	[SerializeField]
	private string[] appLabels;
	[SerializeField]
	private bool cookie;
	[SerializeField]
	private bool logging;
	[SerializeField]
	private bool status;
	[SerializeField]
	private bool xfbml;
	[SerializeField]
	private bool frictionlessRequests;
	[SerializeField]
	private string iosURLSuffix;
}
