using UnityEngine;
using System.Collections;

public class DisplayText : MonoBehaviour {
	
	void OnTriggerEnter(Collider enter) {
		Debug.Log ("ENTERED ROBOT'S REALM");
	}

	void OnTriggerExit(Collider enter) {
		Debug.Log ("LEAVING ROBOT'S REALM");
	}
	
}