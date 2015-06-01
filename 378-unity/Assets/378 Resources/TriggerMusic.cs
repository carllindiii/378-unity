using UnityEngine;
using System.Collections;

public class TriggerMusic : MonoBehaviour {

	//public AudioClip BackgroundMusic;
	private AudioSource source;
	public bool playing;
	
	void Awake () {
		source = GetComponent<AudioSource>();
		playing = false;
	}
	
	
	void OnTriggerEnter (Collider coll)
	{
		if (playing == false) {
			source.Play (0);
			playing = true;
		}
	}
}
