using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CheckpointTextTrigger : MonoBehaviour {

	public Text message;
	bool checkActivate = false;

	// Use this for initialization
	void Awake () {
		
	}
	
	void OnTriggerEnter(Collider enter) {
		if (checkActivate == false) {
			message.text = "Checkpoint!\nPress R to return.";
			checkActivate = true;
		}
	}
	
	void OnTriggerExit(Collider enter) {
		message.text = "";
	}
}
