using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour {

	void Awake() {

	}

	void OnTriggerEnter(Collider enter) {
		TextMesh robotText = (TextMesh) GetComponentInChildren(typeof(TextMesh));
		robotText.text = GetComponent<Text>().text.ToString ();
	}

	void OnTriggerExit(Collider enter) {
		TextMesh robotText = (TextMesh) GetComponentInChildren(typeof(TextMesh));
		robotText.text = "";
	}
	
}