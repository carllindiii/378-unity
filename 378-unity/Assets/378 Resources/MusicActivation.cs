using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MusicActivation : MonoBehaviour {

	private bool checkActivate = false;
	public Text musicText;

	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
		if (checkActivate) {
			if (Input.GetKey (KeyCode.E)) {
				transform.parent.GetComponent<TriggerMusic>().activateMusic();
			}
		}
	
	}

	void OnTriggerEnter(Collider enter) {
		musicText.text = "Press E to activate!";
		checkActivate = true;
	}
	
	void OnTriggerExit(Collider enter) {
		musicText.text = "";
		checkActivate = false;
	}
}
