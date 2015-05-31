using UnityEngine;
using System.Collections;

public class DisplayText : MonoBehaviour {

	void Awake() {

	}

	void OnTriggerEnter(Collider enter) {
		TextMesh robotText = (TextMesh) GetComponentInChildren(typeof(TextMesh));
		robotText.text = "Hello Human! You must reach the top of the terrain\nand claim this planet as your own! Good Luck :)";
	}

	void OnTriggerExit(Collider enter) {
		TextMesh robotText = (TextMesh) GetComponentInChildren(typeof(TextMesh));
		robotText.text = "";
	}
	
}