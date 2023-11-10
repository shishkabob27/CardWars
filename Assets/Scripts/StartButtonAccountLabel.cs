using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonAccountLabel : MonoBehaviour {

	// Use this for initialization
	void Start () {

		var PlayerName = PlayerInfoScript.GetInstance().GetPlayerCode();

		if (PlayerName == "ua" || PlayerName == string.Empty)
		{
			PlayerName = "Guest";
		}
		GetComponent<UILabel>().text = "Logged in as " + PlayerName;

	}
	
}
